using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.DataValidator.Common.Helper
{
    public class ClosedXMLHelper
    {
        public static DataSet Load(string filePath)
        {
            DataSet dataSet = new DataSet();
            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                foreach (IXLWorksheet workSheet in workBook.Worksheets)
                {
                    DataTable dataTable = workSheet.RangeUsed().AsTable().AsNativeDataTable();
                    dataTable.TableName = workSheet.Name;
                    dataSet.Tables.Add(dataTable);
                }
            }
            return dataSet;
        }
    }
}
