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
    internal class PasswordBoxPair
    {
        public PasswordBox? Password { get; set; }
        public PasswordBox? ConfirmPassword { get; set; }
    }

    public class PasswordsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            PasswordBoxPair pair = new PasswordBoxPair();
            for (int i = 0; i < 2; i++)
            {

                if (values[i] is PasswordBox pbox)
                {
                    switch (i)
                    {
                        case 0:
                            pair.Password = pbox;
                            break;
                        case 1:
                            pair.ConfirmPassword = pbox;
                            break;
                        default:
                            break;
                    }
                }
            }

            return pair;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
