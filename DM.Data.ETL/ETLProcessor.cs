using ALE.ETLBox.DataFlow;
using DM.DataValidator.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Data.ETL
{
    public class ExcelData
    {
        [ExcelColumn(0)]
        public string Col1 { get; set; }
        [ExcelColumn(1)]
        public string Col2 { get; set; }
        [ExcelColumn(2)]
        public string Col3 { get; set; }
        [ExcelColumn(4)]
        public string Col4 { get; set; }
        [ExcelColumn(5)]
        public string Col5 { get; set; }
    }

    public class ETLProcessor
    {
        private ExcelSource<ExcelData> _excelSource { get; set; }
        private string _fileName { get; set; }
        private RowTransformation<ExcelData> _rowTransformation { get; set; }

        public ETLProcessor(string fileName)
        {
            _fileName = fileName;
        }

        public void Extract()
        {
            ConsoleHelper.Information($"File {_fileName} Load Started...");
            if (!File.Exists(_fileName))
            {
                ConsoleHelper.Error($"Error ! File {_fileName} Not Found.");
                throw new FileNotFoundException($"File {_fileName} Not Found.");
            }
            _excelSource = new ExcelSource<ExcelData>(_fileName)
            {
                SheetName = "Sheet1"
            };
        }

        public void Transform()
        {
            _rowTransformation = new RowTransformation<ExcelData>(row =>
            {
                row.Col1 = "A" + row.Col1;
                row.Col2 = "B" + row.Col2;
                row.Col3 = "C" + row.Col3;
                row.Col4 = "D" + row.Col4;
                row.Col5 = "E" + row.Col5;
                return row;
            });
            _excelSource.LinkTo(_rowTransformation);
        }

        public void Load()
        {
            MemoryDestination<ExcelData> dest = new MemoryDestination<ExcelData>();
            _rowTransformation.LinkTo(dest);
            _excelSource.Execute();
            dest.Wait();
        }
    }
}
