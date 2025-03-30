using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFUI
{
    internal class PasswordBoxTriple
    {
        public PasswordBox? oldPassword { get; set; }
        public PasswordBox? newPassword { get; set; }
        public PasswordBox? ConfirmNewPassword { get; set; }
    }

    public class TriplePasswordsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            PasswordBoxTriple triple = new PasswordBoxTriple();
            for (int i = 0; i < 3; i++)
            {

                if (values[i] is PasswordBox pbox)
                {
                    switch (i)
                    {
                        case 0:
                            triple.oldPassword = pbox;
                            break;
                        case 1:
                            triple.newPassword = pbox;
                            break;
                        case 2:
                            triple.ConfirmNewPassword = pbox;
                            break;
                        default:
                            break;
                    }
                }
            }

            return triple;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
