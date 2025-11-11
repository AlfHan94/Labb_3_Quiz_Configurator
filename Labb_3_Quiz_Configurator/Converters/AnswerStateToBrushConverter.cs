using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Labb_3_Quiz_Configurator.Converters
{

    public class AnswerStateToBrushConverter : IMultiValueConverter
    {
        private static readonly Brush DefaultBrush = (Brush)new BrushConverter().ConvertFromString("#E3E3E3");
        private static readonly Brush Green = Brushes.LightGreen;
        private static readonly Brush Red = Brushes.LightCoral;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is not [bool showFeedback, string buttonContent, string correct, string selected])
                return DefaultBrush;

            if (!showFeedback)
                return DefaultBrush;

            if (!string.IsNullOrEmpty(correct) && buttonContent == correct)
                return Green;

            if (!string.IsNullOrEmpty(selected) && buttonContent == selected && selected != correct)
                return Red;

            return DefaultBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
