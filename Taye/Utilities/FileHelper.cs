using System.Security.Cryptography;
using Taye.Common;

namespace Taye.Utilities
{
    public class FileHelper
    {
        public async Task<string> UploadFile(Stream s, string name, string path)
        {
            EnsurePathExists(path);
            var fileFullName = Path.Combine(Constants.MediaFullPath, name);
            using var fs = File.OpenWrite(fileFullName);
            await s.CopyToAsync(fs);
            return fileFullName;
        }

        /// <summary>
        /// 所有文件都放到wwwroot下，所以首先保证wwwroot下的前缀path是存在的
        /// </summary>
        /// <param name="pathPrefix"></param>
        void EnsurePathExists(string pathPrefix)
        {
            Directory.CreateDirectory(Path.Combine(Constants.FileRootPath, pathPrefix));
        }

        public static async Task<string> CalculateMD5Async(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) // true means use IO async operations
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, 4096);
                        if (bytesRead > 0)
                        {
                            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                        }
                    } while (bytesRead > 0);

                    md5.TransformFinalBlock(buffer, 0, 0);
                    return BitConverter.ToString(md5.Hash).Replace("-", "").ToUpperInvariant();
                }
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="fileFullName">重命名前文件全路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="newFileName">新文件名</param>
        public static void Rename(string fileFullName, string fileName, string newFileName)
        {
            if (!File.Exists(Path.Combine(fileFullName)))
            {
                return;
            }

            File.Move(fileFullName, fileFullName.Replace(fileName, newFileName));
        }

        public static bool FileExists(string directory, string fileMd5)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            return Directory.GetFiles(directory, fileMd5).Any();
        }

        public static void RemoveFile(string fileFullName)
        {
            try
            {
                File.Delete(fileFullName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
