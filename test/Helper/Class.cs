using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace test.Helper
{
    public static class Class
    {
        //图像灰度化：http://www.cnblogs.com/gdjlc/archive/2013/03/05/2943801.html
        //将彩色图像转化成为灰度图像的过程成为图像的灰度化处理。
        //彩色图像中的每个像素的颜色有R、G、B三个分量决定，而每个分量有255中值可取，
        //这样一个像素点可以有1600多万（255*255*255）的颜色的变化范围。
        //而灰度图像是R、G、B三个分量相同的一种特殊的彩色图像，其一个像素点的变化范围为255种，
        //所以在数字图像处理种一般先将各种格式的图像转变成灰度图像以使后续的图像的计算量变得少一些。
        //灰度图像的描述与彩色图像一样仍然反映了整幅图像的整体和局部的色度和亮度等级的分布和特征。
        //图像的灰度化处理可用两种方法来实现。
        //第一种方法使求出每个像素点的R、G、B三个分量的平均值，然后将这个平均值赋予给这个像素的三个分量。
        //第二种方法是根据YUV的颜色空间中，Y的分量的物理意义是点的亮度，由该值反映亮度等级，
        //根据RGB和YUV颜色空间的变化关系可建立亮度Y与R、G、B三个颜色分量的对应：Y=0.3R+0.59G+0.11B，以这个亮度值表达图像的灰度值。


        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }


        //灰度反转：
        //把每个像素点的R、G、B三个分量的值0的设为255，255的设为0。
        /// <summary>
        /// 图像灰度反转
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayReverse(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    Color newColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }


        //灰度图像二值化：
        //在进行了灰度化处理之后，图像中的每个象素只有一个值，那就是象素的灰度值。它的大小决定了象素的亮暗程度。
        //为了更加便利的开展下面的图像处理操作，还需要对已经得到的灰度图像做一个二值化处理。
        //图像的二值化就是把图像中的象素根据一定的标准分化成两种颜色。在系统中是根据象素的灰度值处理成黑白两种颜色。
        //和灰度化相似的，图像的二值化也有很多成熟的算法。它可以采用自适应阀值法，也可以采用给定阀值法。
        /// <summary>
        /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }


        /// <summary>
        /// 图像二值化2
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp2(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            for (int y = 0; y < h; y++)
            {
                byte[] scan = new byte[(w + 7) / 8];
                for (int x = 0; x < w; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            bmp.UnlockBits(data);
            return bmp;
        }
    }

    public class ValidateCodeUtils
    {
        public static Bitmap CreateImage(int length, out string validateCode)
        {
            validateCode = string.Empty;
            //颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Purple, Color.Red, Color.Blue, Color.Brown, Color.Navy };
            //字体列表，用于验证码 
            string[] font = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < length; i++)
            {
                validateCode += character[rnd.Next(character.Length)];
            }
            Bitmap bmp = new Bitmap(length * 20 + 20, 40);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            ////画噪线 
            //for (int i = 0; i < 10; i++)
            //{
            //    int x1 = rnd.Next(20) * rnd.Next(5);
            //    int y1 = rnd.Next(8) * rnd.Next(5);
            //    int x2 = rnd.Next(20) * rnd.Next(5);
            //    int y2 = rnd.Next(8) * rnd.Next(5);
            //    Color clr = color[rnd.Next(color.Length)];
            //    g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            //}
            //画验证码字符串 
            for (int i = 0; i < validateCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, 18);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(validateCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 20 + 8, (float)8);
            }
            ////画噪点 
            //for (int i = 0; i < 30; i++)
            //{
            //    int x = rnd.Next(bmp.Width);
            //    int y = rnd.Next(bmp.Height);
            //    Color clr = color[rnd.Next(color.Length)];
            //    bmp.SetPixel(x, y, clr);
            //}
            try
            {
                return bmp;
            }
            finally
            {
                //显式释放资源 
                g.Dispose();
            }
        }
    }
}