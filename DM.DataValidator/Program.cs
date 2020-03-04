using DM.Data.ETL;
using DM.DataValidator.Common.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.DataValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                ConsoleHelper.Error($"Invalid Arguments........");
                ConsoleHelper.Information($"E.g DM.DataValidator.exe Data.xlsx DealValidator");
            }

            string file = args[0];
            string validatorType = args[1];

            ETLProcessor eTLProcessor = new ETLProcessor(file);
            eTLProcessor.Extract();
            eTLProcessor.Transform();
            eTLProcessor.Load();

            //Processor processor = new Processor(file, validatorType);
            //processor.LoadData();
            //processor.Validate();
            //processor.SaveResult();
        }
    }
}
