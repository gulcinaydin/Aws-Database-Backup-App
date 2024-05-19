using System.Diagnostics;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using DatabaseBackupWebApp.Models;
using DatabaseBackupWebApp.Services;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace DatabaseBackupWebApp.Controllers;
 
public class HomeController : Controller
{
    private readonly IAwsService _awsService;
    private readonly Settings _settings;


    public HomeController(IAwsService awsService, IOptions<Settings> settings)
    {
        _awsService = awsService;
        _settings = settings.Value;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]
    public IActionResult BackupDatabase()
    {
        var result = _awsService.AwsCreateBackupAndUpload();
        ViewBag.Message = result.Message;
        ViewBag.Status = result.Status;
        ViewBag.FileName = result.FileName;
        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DownloadS3(string keyName)
    {
        if (keyName != null)
        {
            string accessKey = _settings.AccessKey;
            string accessSecret = _settings.AccessSecret;
            var region = RegionEndpoint.EUNorth1;

            var credentials = new BasicAWSCredentials(accessKey, accessSecret);
            var s3Client = new AmazonS3Client(credentials, region);

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = keyName
            };

            using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await responseStream.CopyToAsync(memoryStream);
                byte[] data = memoryStream.ToArray();

                return File(data, response.Headers["Content-Type"], keyName);
            }
        }

        return View("Index");
    }
}