using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.DataValidator
{
    public class RowValidator : AbstractValidator<DataRow>
    {
        public RowValidator(List<AbstractValidator<DataRow>> validationRules)
        {
            foreach(var validationRule in validationRules)
            {
                RuleFor(dataRow => dataRow).SetValidator(validationRule);
            }
        }
    }
}
