namespace DatabaseBackupWebApp.Models;

public class ResponseModel
{
    public string? Message { get; set; }
    public int Status { get; set; }
    public string? FileName { get; set; }
}