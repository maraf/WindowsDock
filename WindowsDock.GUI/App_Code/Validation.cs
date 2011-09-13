using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using DesktopCore;

namespace WindowsDock.GUI
{
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int num;
            if (Int32.TryParse(value.ToString(), out num) && num >= 0)
                return new ValidationResult(true, null);

            return new ValidationResult(false, Resource.Get("Validation.NotPositiveNumber"));
        }
    }
}
