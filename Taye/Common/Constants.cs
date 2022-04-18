namespace Taye.Common
{
    public static class Constants
    {
        public readonly static string FileRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public readonly static string MediaDirectory = "media";

        public readonly static string MediaFullPath = Path.Combine(FileRootPath, MediaDirectory);

        public readonly static string ThumbPrefix = "Thumb_";
    }
}
