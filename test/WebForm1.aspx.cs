using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tesseract;
using Page = Tesseract.Page;

namespace test
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ocrTtxt = "";
            //chi_sim是中文库
            const string language = "chi_sim";
            //Nuget安装的Tessract版本为3.20，tessdata的版本必须与其匹配，另外路径最后必须以"\"或者"/"结尾
            string TessractData = AppDomain.CurrentDomain.BaseDirectory + @"tessdata\";
            TesseractEngine test = new TesseractEngine(TessractData, language);
            //创建一个图片对象
            Bitmap tmpVal = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"Content\捕获.PNG");
            //灰度化，可以提高识别率
            var tmpImage = Helper.Class.ToGray(tmpVal);
            //Page tmpPage = test.Process(tmpImage, pageSegMode: test.DefaultPageSegMode);
            Page tmpPage = test.Process(tmpImage);
            ocrTtxt = tmpPage.GetText();
        }
    }
}