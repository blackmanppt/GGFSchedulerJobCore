using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF_XML_To_Excel
{
    class SystemLog
    {
        public void ErrorLog(Exception ex, string 程式名稱)
        {
            using (var conn = new GGFEntities())
            {
                using (var transaction = conn.Database.BeginTransaction())
                {
                    ERROR_LOG error = new ERROR_LOG
                    {
                        ERROR_LOG1 = ex.ToString(),
                        ERROR_PROGRAM = "LF XML to Excel 程式，程式名稱：" + 程式名稱,
                        CREATED_BY = "Program",
                        CREATEDATE = DateTime.Now
                    };
                    conn.ERROR_LOG.Add(error);
                    conn.SaveChanges();
                    transaction.Commit();
                }
            }
        }
    }
}
