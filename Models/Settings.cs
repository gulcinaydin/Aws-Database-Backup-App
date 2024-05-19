namespace DatabaseBackupWebApp.Models;

public class Settings
{
    public string AccessKey { get; set; }
    public string AccessSecret { get; set; }
    public string BucketName { get; set; }
    public string Ec2Host { get; set; }
    public string Ec2Username { get; set; }
    public string PrivateKeyPath { get; set; }
    public string DatabaseName { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }
}