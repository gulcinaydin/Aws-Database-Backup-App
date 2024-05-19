using System.Diagnostics;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DatabaseBackupWebApp.Models;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace DatabaseBackupWebApp.Services;

public class AwsService : IAwsService
{
    private readonly Settings _settings;

    public AwsService(IOptions<Settings> settings)
    {
        _settings = settings.Value;
    }

    public ResponseModel ExecuteCommand(SshClient client, string command)
    {
        using (var cmd = client.CreateCommand(command))
        {
            var result = cmd.Execute();
            if (cmd.ExitStatus != 0)
            {
                return new ResponseModel() { Message = $"Komut çalıştırılırken hata oluştu: {cmd.Error}", Status = 0 };
            }

            return new ResponseModel() { Status = 1 };
        }
    }

    public ResponseModel AwsCreateBackupAndUpload()
    {
        string ec2Host = _settings.Ec2Host;
        string ec2Username = _settings.Ec2Username;
        string privateKeyPath = @$"{_settings.PrivateKeyPath}";

        string databaseName = _settings.DatabaseName;
        string backupFileName = $"/tmp/{databaseName}_backup.sql";
        string compressedBackupFileName = $"/tmp/{databaseName}_backup.sql.gz";
        string s3BucketName = _settings.BucketName;
        string s3KeyName = $"backups/{databaseName}_backup_{DateTime.Now:yyyyMMdd_HH-mm-ss}.gz";

        try
        {
            using (var client = new SshClient(ec2Host, ec2Username, new PrivateKeyFile(privateKeyPath)))
            {
                client.Connect();

                // Veritabanı yedeğini oluşturma
                string backupCommand = $"mysqldump -u {_settings.User} -p'{_settings.Pass}' {databaseName} > {backupFileName}";
                var backupResponse = ExecuteCommand(client, backupCommand);

                if (backupResponse.Status == 0)
                {
                    backupResponse.Message =
                        backupResponse.Message?.Insert(0, "Yedekleme oluşturulurken bir hata oluştu. ");
                    return backupResponse;
                }

                // Dosyayı sıkıştır
                string gzipCommand = $"gzip -f {backupFileName}";
                var gzipResponse = ExecuteCommand(client, gzipCommand);
                if (gzipResponse.Status == 0)
                {
                    gzipResponse.Message =
                        gzipResponse.Message?.Insert(0, "Sıkıştırılırken bir hata oluştu ");
                    return gzipResponse;
                }

                // S3'e upload etme
                string s3UploadCommand = $"aws s3 cp {compressedBackupFileName} s3://{s3BucketName}/{s3KeyName}";
                var uploadResponse = ExecuteCommand(client, s3UploadCommand);
                if (uploadResponse.Status == 0)
                {
                    uploadResponse.Message =
                        uploadResponse.Message?.Insert(0, "S3'e upload edilirken bir hata oluştu. ");
                    return uploadResponse;
                }
                
                client.Disconnect();

                var response = new ResponseModel()
                    { Message = "Yedekleme yaratıldı ve başarıyla S3'e upload edildi.", Status = 1, FileName = s3KeyName};
                return response;
            }
        }
        catch (Exception ex)
        {
            return new ResponseModel() { Message = $"Bir hata oluştu: {ex.Message}", Status = 0 };
        }
    }

}