namespace Taye.Common
{
    public static class Constants
    {
        public readonly static string FileRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public readonly static string MediaDirectory = "media";

        public readonly static string MediaFullPath = Path.Combine(FileRootPath, MediaDirectory);

        public readonly static string ThumbPrefix = "Thumb_";

        public readonly static string ImageFileSufix = ".jpg";

        public readonly static string VideoFileSufix = ".mp4";
    }
}
