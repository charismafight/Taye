using System.Drawing;
using System.Text;

namespace Taye.Utilities
{
    public class ImageHelper
    {
        /// <summary>
        /// (推荐)生成缩略图，图片流输入，输出图片流
        /// </summary>
        /// <param name="dWidth">要生成的宽度</param>
        /// <param name="dHeight">要生成的高度</param>
        /// <param name="flag">生成图片质量，1-100</param>
        /// <param name="inputStream">输入图片流</param>
        /// <param name="outStream">输出图片流</param>
        public static void CompressImgByte(int dWidth, int dHeight, Stream inputStream, Stream outStream, int flag = 80, string arg = null)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromStream(inputStream);
            var prop = iSource.PropertyItems.FirstOrDefault(x => x.Id == 274);
            if (prop != null)
            {
                //读取旋转方向
                byte[] buffed = prop.Value;
                StringBuilder sbv = new StringBuilder();
                foreach (var byteValue in buffed)
                {
                    sbv.Append(byteValue.ToString("x2"));
                }
                string value2 = sbv.ToString();
                //Console.WriteLine($"图片id  {arg}  图片274里面的值>" + value2);//linux测试
                //windows获取到旋转的值为0600，linux旋转值为0006
                if (value2.Equals("0006") || value2.Equals("0600"))
                {
                    //未做任何操作，此缩略图会自动逆时针旋转90度
                    //下面操作纠正旋转，让其顺时针旋转90度，让图片回正
                    iSource.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
            }

            System.Drawing.Imaging.ImageFormat tFormat = iSource.RawFormat;
            //按比例缩放            
            if (dWidth > 0 && iSource.Width > dWidth && iSource.Width > iSource.Height)
            {
                dHeight = dWidth * iSource.Height / iSource.Width;
            }
            else if (dWidth > 0 && dHeight == 0 && iSource.Width > dWidth)
            {
                dHeight = dWidth * iSource.Height / iSource.Width;
            }
            else if (dWidth == 0 && dHeight > 0 && iSource.Width > iSource.Height)
            {
                dWidth = dHeight * iSource.Width / iSource.Height;
            }
            else if (dHeight > iSource.Height || dWidth > iSource.Width)
            {
                dWidth = iSource.Width;
                dHeight = iSource.Height;
            }
            else if (dHeight > 0 && iSource.Width < iSource.Height)
            {
                dWidth = dHeight * iSource.Width / iSource.Height;
            }
            else
            {
                dWidth = iSource.Width;
                dHeight = iSource.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            //ob.SetResolution(72,72);
            Graphics g = Graphics.FromImage(ob);
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle(0, 0, dWidth, dHeight), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量  
            System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            System.Drawing.Imaging.EncoderParameter eParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.ImageCodecInfo jpegICIinfo = arrayICI.FirstOrDefault(x => x.FormatDescription.Equals("JPEG"));
                if (jpegICIinfo != null)
                {
                    //ob.Save("d://" + DateTime.Now.Ticks + ".jpg", jpegICIinfo, ep);
                    ob.Save(outStream, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(outStream, tFormat);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Console.WriteLine(msg);
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }

        /// <summary>
        /// (推荐)生成缩略图，图片流输入，输出图片流
        /// </summary>
        /// <param name="dWidth">要生成的宽度</param>
        /// <param name="dHeight">要生成的高度</param>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="outFilePath">输出保存文件路径</param>
        /// <param name="flag">生成的图片质量，0-100，默认80</param>
        public static void CompressImgFile(int dWidth, int dHeight, string filePath, string outFilePath, int flag = 80)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(filePath);
            //检测图片是否有旋转
            foreach (var item in iSource.PropertyItems)
            {
                //System.Diagnostics.Debug.WriteLine(item.Id);
                if (item.Id == 274)
                {
                    //读取旋转方向
                    byte[] buffed = item.Value;
                    StringBuilder sbv = new StringBuilder();
                    foreach (var byteValue in buffed)
                    {
                        sbv.Append(byteValue.ToString("x2"));
                    }
                    //string value2 = string.Join("", buffed);
                    string value2 = sbv.ToString();
                    System.Diagnostics.Debug.WriteLine("方向=" + value2);
                    //windows获取到旋转的值为0600，linux旋转值为0006
                    if (value2.Equals("0006") || value2.Equals("0600"))
                    {
                        //未做任何操作，此缩略图会自动逆时针旋转90度
                        //下面操作纠正旋转，让其顺时针旋转90度，让图片回正
                        iSource.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    break;
                }
            }

            System.Drawing.Imaging.ImageFormat tFormat = iSource.RawFormat;
            //按比例缩放            
            if (dWidth > 0 && iSource.Width > dWidth && iSource.Width > iSource.Height)
            {
                dHeight = dWidth * iSource.Height / iSource.Width;
            }
            else if (dWidth > 0 && dHeight == 0 && iSource.Width > dWidth)
            {
                dHeight = dWidth * iSource.Height / iSource.Width;
            }
            else if (dWidth == 0 && dHeight > 0 && iSource.Width > iSource.Height)
            {
                dWidth = dHeight * iSource.Width / iSource.Height;
            }
            else if (dHeight > iSource.Height || dWidth > iSource.Width)
            {
                dWidth = iSource.Width;
                dHeight = iSource.Height;
            }
            else if (dHeight > 0 && iSource.Width < iSource.Height)
            {
                dWidth = dHeight * iSource.Width / iSource.Height;
            }
            else
            {
                dWidth = iSource.Width;
                dHeight = iSource.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            //ob.SetResolution(72,72);
            Graphics g = Graphics.FromImage(ob);
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle(0, 0, dWidth, dHeight), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量  
            System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            System.Drawing.Imaging.EncoderParameter eParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.ImageCodecInfo jpegICIinfo = null;
                // string imgExtend = tFormat.ToString().ToUpper();
                string imgExtend = "JPEG";
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals(imgExtend))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(outFilePath, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    ob.Save(outFilePath, tFormat);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Console.WriteLine(msg);
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }

    }
}