using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace NPOITest
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDatatableToExcel();

            Excel帶格式 test1 = new Excel帶格式();
            test1.DataFormatsXlsx();
        }

        private static void TestDatatableToExcel()
        {
            DataTable dt = new DataTable();
            using (SqlConnection Conn = new SqlConnection("Data Source=192.168.0.131;Initial Catalog=GGF;User ID=sa;Password=1qaz2wsx"))
            {
                SqlDataAdapter myAdapter = new SqlDataAdapter("select top 10 * from [View採購單] where 三角出 <>'Y' ", Conn);
                myAdapter.Fill(dt);    //---- 這時候執行SQL指令。取出資料，放進 DataSet。

                DatatableToExcel xx = new DatatableToExcel();
                xx.ExcelWithNPOI(dt, "");
            }
        }
    }
}
