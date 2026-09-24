using System;
using System.Globalization;
using System.Windows.Data;
using Anbarban.Services;

namespace Anbarban.Converters
{
    public sealed class JalaliDisplayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value == DBNull.Value) return "";
            if (value is DateTime dt) return JalaliCalendar.Format(dt);
            if (DateTime.TryParse(value.ToString(), out var parsed)) return JalaliCalendar.Format(parsed);
            return value.ToString();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
