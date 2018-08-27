using System;
using System.IO;

namespace LF_XML_To_Excel
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemLog sl = new SystemLog();
            string[] savePath=null;
            try
            {
                bool btestflag = false;
                if (btestflag)
                    savePath = Directory.GetFiles(@"Z:\部門空間\14.資訊部\SFTP\LiFung", "*.XML");
                else
                    savePath = Directory.GetFiles(@"\\192.168.0.150\sftp\LiFung", "*.XML");
                string strSource = @"\\192.168.0.156\great giant\部門空間\EDI_File\Source\";
                if (savePath.Length > 0)
                {
                    for (int file數量 = 0; file數量 < savePath.Length; file數量++)
                    {
                        int iflg = 0;
                        轉出EXCEL 轉出 = new 轉出EXCEL();
                        if (轉出.ERP格式(savePath[file數量]) == true && 轉出.匯入格式(savePath[file數量]) == true)
                        {
                            string destinationFile = strSource + savePath[file數量].ToString().Substring(savePath[file數量].ToString().Length - 37, 37);
                            if (File.Exists(destinationFile))
                            {
                                int icheck = 1;
                                while (File.Exists(strSource + icheck.ToString("000") + savePath[file數量].ToString().Substring(savePath[file數量].ToString().Length - 37, 37)))
                                {
                                    icheck++;
                                }
                                destinationFile = strSource + icheck.ToString("000") + savePath[file數量].ToString().Substring(savePath[file數量].ToString().Length - 37, 37);
                            }
                            if (!btestflag)
                                File.Move(savePath[file數量], destinationFile);
                        }
                        else
                        {
                            iflg = 1;
                        }
                        using (var conn = new GGFEntities())
                        {
                            using (var transaction = conn.Database.BeginTransaction())
                            {
                                try
                                {

                                    XML匯出LOG XMLLog = new XML匯出LOG
                                    {
                                        檔案名稱 = savePath[file數量].ToString().Substring(savePath[file數量].ToString().Length - 37, 37),
                                        匯入狀態 = iflg,
                                        Style = (!string.IsNullOrEmpty(轉出.StrStyle)) ? 轉出.StrStyle : "Style:沒有資料"
                                    };
                                    conn.XML匯出LOG.Add(XMLLog);
                                    conn.SaveChanges();
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    sl.ErrorLog(ex, "匯入LOG失敗");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sl.ErrorLog(ex, "找不到資料夾");
            }
            
        }
    }
}
