using ClosedXML.Excel;
using DM.DataValidator.Common.Helper;
using DM.DataValidator.Validators;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.DataValidator
{
    public class Processor
    {
        private readonly DataSet SuccessDataSet = new DataSet();
        private readonly DataSet FailedDataSet = new DataSet();

        private string ValidatorType { get; }
        private string FileName { get; set; }
        private DataSet WorkBooks { get; set; }


        public Processor(string fileName, string validatorType)
        {
            FileName = fileName;
            ValidatorType = validatorType;
        }

        private List<AbstractValidator<DataRow>> GetValidationRules(DataColumnCollection columns)
        {
            List<AbstractValidator<DataRow>> validationRules = new List<AbstractValidator<DataRow>>();
            foreach (DataColumn dataColumn in columns)
            {
                if (ValidatorType.Equals("DealValidator", StringComparison.InvariantCultureIgnoreCase))
                {
                    validationRules.Add(new DealValidator(dataColumn.ColumnName));
                }
                else
                {
                    ConsoleHelper.Error($"Error ! Wrong {ValidatorType} validator type passed.");
                    throw new ArgumentException($"Error ! Wrong {ValidatorType} validator type passed.");
                }
            }
            return validationRules;
        }

        public void LoadData()
        {
            ConsoleHelper.Information($"File {FileName} Load Started...");
            if (!File.Exists(FileName))
            {
                ConsoleHelper.Error($"Error ! File {FileName} Not Found.");
                throw new FileNotFoundException($"File {FileName} Not Found.");
            }
            WorkBooks = Task.Run(() => ClosedXMLHelper.Load(FileName)).Result;
            ConsoleHelper.Information($"File {FileName} Load Completed.");
        }

        public void Validate()
        {
            var success = 0;
            var failed = 0;
            var total = 0;
            ConsoleHelper.Information($"Validation Started...");
            foreach (DataTable dataTable in WorkBooks.Tables)
            {
                List<AbstractValidator<DataRow>> validationRules = new List<AbstractValidator<DataRow>>();
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    validationRules.Add(new DealValidator(dataColumn.ColumnName));
                }

                DataTable successResult = dataTable.Clone();
                successResult.TableName = dataTable.TableName + "_Success";
                DataTable failedResult = dataTable.Clone();
                failedResult.TableName = dataTable.TableName + "_Failed";
                failedResult.Columns.Add("ErrorMessage"); // Need to move correct place

                RowValidator validationRule = new RowValidator(validationRules);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var result = validationRule.Validate(dataRow);
                    if (result.IsValid)
                    {
                        successResult.ImportRow(dataRow);
                    }
                    else
                    {
                        var tempDataRow = failedResult.NewRow();
                        tempDataRow.ItemArray = dataRow.ItemArray;
                        tempDataRow["ErrorMessage"] = string.Join(";", result.Errors.Select(error => error.ErrorMessage).ToArray());
                        failedResult.Rows.Add(tempDataRow);
                    }
                }
                SuccessDataSet.Tables.Add(successResult);
                FailedDataSet.Tables.Add(failedResult);
                success += successResult.Rows.Count;
                failed += failedResult.Rows.Count;
                total += dataTable.Rows.Count;
            }
            ConsoleHelper.Information($"Validation Completed");

            // Output
            ConsoleHelper.Information($"=================");
            ConsoleHelper.Information($"Validation Result");
            ConsoleHelper.Information($"=================");
            ConsoleHelper.Success($"Passed: {success} of {total}");
            ConsoleHelper.Error($"Passed: {failed} of {total}");
            ConsoleHelper.Information($"=================");
        }

        public void SaveResult()
        {
            string rootPath = Path.Combine(Directory.GetParent(FileName).FullName, "Output");
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.AddWorksheet(SuccessDataSet);
                    workbook.SaveAs(Path.Combine(rootPath, $"Success_{DateTime.Now.ToString("yyyymmddhhMMssfff")}.xlsx"));
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Error ! {ex.Message}");
                throw ex;
            }
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.AddWorksheet(FailedDataSet);
                    workbook.SaveAs(Path.Combine(rootPath, $"Failed_{DateTime.Now.ToString("yyyymmddhhMMssfff")}.xlsx"));
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Error ! {ex.Message}");
                throw ex;
            }
        }
    }
}
