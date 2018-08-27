using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.StreamReader sErr;
            System.IO.StreamReader sOut;
            String tempErr, tempOut;
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = @"NET";
            myProcess.StartInfo.Arguments = @"USE Y:\192.168.0.150\SFTP sftp2018 /user:\SFTP\"; //password is 123456, username is Administrator
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardError = true;
            myProcess.StartInfo.RedirectStandardOutput = true; // 導出 StandardOutput
            try
            {
                myProcess.Start();
                myProcess.WaitForExit(10000);

                if (!myProcess.HasExited)
                {
                    myProcess.Kill();
                    Console.Write("執行失敗!!");
                }
                else
                {
                    sErr = myProcess.StandardError;
                    tempErr = sErr.ReadToEnd();
                    sErr.Close();

                    sOut = myProcess.StandardOutput;
                    tempOut = sOut.ReadToEnd();
                    sOut.Close();

                    if (myProcess.ExitCode == 0) //連線磁碟機建立成功
                    {
                        ////Response.Write("執行成功" + "<BR>" + tempOut.ToString()); // 把執行結果也印出來
                        //System.IO.File.Copy(@"D:\abc.xls", @"Y:\abc.xls", true);
                    }
                    else if (myProcess.ExitCode == 2) // 忽略連線磁碟機已存在
                    {
                        var savePath = Directory.GetFiles(@"");
                        //System.IO.File.Copy(@"D:\abc.xls", @"Y:\abc.xls", true);
                    }
                    else
                    {
                        Console.Write(tempErr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                myProcess.Close();
            }
            finally
            {
                myProcess.Close();
                Console.ReadLine();
            }
        }
    }
}
