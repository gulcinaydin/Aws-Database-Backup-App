using DatabaseBackupWebApp.Models;
using Renci.SshNet;

namespace DatabaseBackupWebApp.Services;

public interface IAwsService
{
    public ResponseModel ExecuteCommand(SshClient client, string command);

    public ResponseModel AwsCreateBackupAndUpload();
    
}