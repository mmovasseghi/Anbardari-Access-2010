using System;
namespace Anbarban.Services
{
    /// <summary>تبدیل شمسی/میلادی — همان منطق v2 برای اپراتور.</summary>
    public static class JalaliCalendar
    {
        public static string Format(DateTime? g)
        {
            if (g == null) return "";
            GregorianToJalali(g.Value.Year, g.Value.Month, g.Value.Day, out var jy, out var jm, out var jd);
            return $"{jy:0000}/{jm:00}/{jd:00}";
        }

        public static bool TryParse(string? text, out DateTime gregorian)
        {
            gregorian = default;
            if (string.IsNullOrWhiteSpace(text)) return false;
            var parts = text.Trim().Replace('-', '/').Split('/');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out var jy) || !int.TryParse(parts[1], out var jm) || !int.TryParse(parts[2], out var jd))
                return false;
            if (jy < 1300 || jy > 1500 || jm < 1 || jm > 12 || jd < 1 || jd > 31) return false;
            JalaliToGregorian(jy, jm, jd, out var gy, out var gm, out var gd);
            gregorian = new DateTime(gy, gm, gd);
            return true;
        }

        private static void GregorianToJalali(int gy, int gm, int gd, out int jy, out int jm, out int jd)
        {
            int[] g_d_m = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };
            int gy2 = gy > 1600 ? gy - 1600 : gy - 621;
            int days = 365 * gy2 + (gy2 + 3) / 4 - (gy2 + 99) / 100 + (gy2 + 399) / 400 - 80 + gd;
            days += gm > 2 ? g_d_m[gm - 1] + 1 : g_d_m[gm - 1];
            jy = -979 + 33 * (days / 12053);
            days %= 12053;
            jy += 4 * (days / 1461);
            days %= 1461;
            if (days > 365) { jy += (days - 1) / 365; days = (days - 1) % 365; }
            if (days < 186) { jm = 1 + days / 31; jd = 1 + days % 31; }
            else { jm = 7 + (days - 186) / 30; jd = 1 + (days - 186) % 30; }
        }

        private static void JalaliToGregorian(int jy, int jm, int jd, out int gy, out int gm, out int gd)
        {
            jy -= 979; jm -= 1; jd -= 1;
            int days = 365 * jy + (jy / 33) * 8 + ((jy % 33) + 3) / 4;
            days += jm < 7 ? jm * 31 : (jm - 7) * 30 + 186;
            days += jd + 79;
            gy = 1600 + 400 * (days / 146097);
            days %= 146097;
            bool leap = true;
            if (days >= 36525)
            {
                days--;
                gy += 100 * (days / 36524);
                days %= 36524;
                if (days >= 365) days++; else leap = false;
            }
            gy += 4 * (days / 1461);
            days %= 1461;
            if (days >= 366) { leap = false; days--; gy += days / 365; days %= 365; }
            int[] sal_a = { 0, 31, leap ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            gm = 0;
            while (gm < 12 && days >= sal_a[gm + 1]) { days -= sal_a[gm + 1]; gm++; }
            gm++;
            gd = days + 1;
        }
    }
}
