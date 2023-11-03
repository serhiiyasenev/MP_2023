namespace BusinessLayer.Models
{
    public class FileInfoModel
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTime? UploadTime { get; set; }
    }
}
