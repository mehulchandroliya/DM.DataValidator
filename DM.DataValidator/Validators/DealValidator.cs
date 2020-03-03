using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.DataValidator.Validators
{
    public class DealValidator : AbstractValidator<DataRow>
    {
        public DealValidator(string name)
        {
            RuleFor(dataRow => dataRow.Field<Double>(name).ToString()).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().WithMessage("Must Not Null");
            RuleFor(dataRow => dataRow.Field<Double>(name)).Cascade(CascadeMode.StopOnFirstFailure).NotEqual(0).WithMessage("Must Grater Than 0");
        }
    }
}
