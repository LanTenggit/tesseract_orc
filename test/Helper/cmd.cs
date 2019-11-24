using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Helper
{
    public class cmd
    {
        public void cmdexe()
        {
            string languageName = "languageName";  //语言名称
            string fontType = "fontType";  //字体名称
            string cmd = ""; //cmd命令

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息

            cmd = string.Format(@"cd C:\Users\Administrator\Desktop\训练");
            p.StandardInput.WriteLine(cmd + "&exit");

            //生成font_properties
            cmd = string.Format("echo {0} 0 0 0 0 0 >font_properties", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            //一：生成训练文件
            cmd = string.Format("tesseract {0}.{1}.exp0.tif {0}.{1}.exp0 -l eng -psm 7 nobatch box.train", languageName,fontType);
            p.StandardInput.WriteLine(cmd+"&exit");

            //二：生成字符集文件
            cmd = string.Format("unicharset_extractor {0}.{1}.exp0.box", languageName, fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            //三：生成shape文件
            cmd = string.Format("shapeclustering -F font_properties -U unicharset -O {0}.unicharset {0}.{1}.exp0.tr", languageName, fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            //四：生成聚集字符特征文件
            cmd = string.Format("mftraining -F font_properties -U unicharset -O {0}.unicharset {0}.{1}.exp0.tr", languageName, fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            //五：生成字符正常化特征文件
            cmd = string.Format("cntraining {0}.{1}.exp0.tr", languageName, fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            //六：更名
            cmd = string.Format("rename normproto {0}.normproto", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            cmd = string.Format("rename inttemp {0}.inttemp", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            cmd = string.Format("rename pffmtable {0}.pffmtable ", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            cmd = string.Format("rename unicharset {0}.unicharset", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            cmd = string.Format("rename shapetable {0}.shapetable", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");

            //七：合并训练文件
            cmd = string.Format("combine_tessdata {0}.", fontType);
            p.StandardInput.WriteLine(cmd + "&exit");
            



            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();

            //StreamReader reader = p.StandardOutput;
            //string line=reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    str += line + "  ";
            //    line = reader.ReadLine();
            //}

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();



        }
    }
}