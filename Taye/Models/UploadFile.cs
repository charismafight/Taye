using Taye.Enums;

namespace Taye.Models
{
    public class UploadFile : BaseModel
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public long Size { get; set; }

        public string MD5 { get; set; }

        public DateTime UploadTime { get; set; }

        public FileType FileType { get; set; }

        public string? ThumbnailFilePath { get; set; }
    }
}
