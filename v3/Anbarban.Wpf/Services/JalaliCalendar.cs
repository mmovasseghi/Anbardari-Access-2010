using System;
using System.Globalization;

namespace Anbarban.Services
{
    /// <summary>تاریخ شمسی با تقویم رسمی .NET (PersianCalendar).</summary>
    public static class JalaliCalendar
    {
        private static readonly PersianCalendar Pc = new();

        public static string[] MonthNames { get; } =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        public static string[] WeekDayShort { get; } = { "ش", "ی", "د", "س", "چ", "پ", "ج" };

        public static DateTime Today => DateTime.Today;

        public static void ToJalali(DateTime g, out int jy, out int jm, out int jd)
        {
            jy = Pc.GetYear(g);
            jm = Pc.GetMonth(g);
            jd = Pc.GetDayOfMonth(g);
        }

        public static string Format(DateTime? g)
        {
            if (g == null) return "";
            ToJalali(g.Value.Date, out var jy, out var jm, out var jd);
            return $"{jy:0000}/{jm:00}/{jd:00}";
        }

        public static bool TryParse(string? text, out DateTime gregorian)
        {
            gregorian = default;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var parts = text.Trim().Replace('-', '/').Replace('\\', '/').Split('/');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0].Trim(), out var jy) ||
                !int.TryParse(parts[1].Trim(), out var jm) ||
                !int.TryParse(parts[2].Trim(), out var jd))
                return false;
            return TryJalaliToGregorian(jy, jm, jd, out gregorian);
        }

        public static bool TryJalaliToGregorian(int jy, int jm, int jd, out DateTime gregorian)
        {
            gregorian = default;
            try
            {
                if (jy < 1300 || jy > 1499 || jm < 1 || jm > 12 || jd < 1) return false;
                var max = Pc.GetDaysInMonth(jy, jm);
                if (jd > max) return false;
                gregorian = Pc.ToDateTime(jy, jm, jd, 0, 0, 0, 0).Date;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int GetDaysInMonth(int jy, int jm) => Pc.GetDaysInMonth(jy, jm);

        /// <summary>ستون ۰=شنبه … ۶=جمعه (برای گرید تقویم).</summary>
        public static int WeekColumnIndex(DateTime gregorian)
        {
            var dow = Pc.GetDayOfWeek(gregorian);
            return ((int)dow + 1) % 7;
        }

        public static int WeekColumnIndex(int jy, int jm, int jd) =>
            WeekColumnIndex(Pc.ToDateTime(jy, jm, jd, 0, 0, 0, 0));
    }
}
