using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tesseract;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string ocrTtxt = "";
            //chi_sim是中文库
            const string language = "chi_sim";
            //Nuget安装的Tessract版本为3.20，tessdata的版本必须与其匹配，另外路径最后必须以"\"或者"/"结尾
            string TessractData = AppDomain.CurrentDomain.BaseDirectory+ @"tessdata\";
            TesseractEngine test = new TesseractEngine(TessractData, language);
            //创建一个图片对象
            Bitmap tmpVal = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"Content\捕获.PNG");
            //灰度化，可以提高识别率
            var tmpImage = Helper.Class.ToGray(tmpVal);
            //Page tmpPage = test.Process(tmpImage, pageSegMode: test.DefaultPageSegMode);
            Page tmpPage = test.Process(tmpImage);
            ocrTtxt = tmpPage.GetText();
            return View();
        }
    }
}