using System;

namespace Colosoft.Presentation.Converters
{
    public class BooleanToYesOrNoConverter : GenericConverter<BooleanToYesOrNoConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                if ((bool)value)
                {
                    return "Sim";
                }
                else
                {
                    return "Não";
                }
            }
            catch
            {
                return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value?.ToString() == "Sim")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
