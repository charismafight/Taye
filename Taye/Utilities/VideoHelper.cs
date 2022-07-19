using System.Diagnostics;
using System.Reflection;

namespace Taye.Utilities
{
    public static class VideoHelper
    {
        public async static Task GetFrame(string inputVideoPath, string outputPath)
        {
            string sPath_FFMpegDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"ffmpeg\bin\");
            outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", outputPath);
            var startInfo = new ProcessStartInfo
            {
                FileName = sPath_FFMpegDir + $"ffmpeg.exe",
                Arguments = $"-i {inputVideoPath}  -vf select=eq(n\\,0) -y scale=200:-2 -vframes 1 {outputPath}",
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = sPath_FFMpegDir
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
