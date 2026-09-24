using System;

namespace Anbarban.Services
{
    /// <summary>حالت تست خودکار — پیام‌های موفقیت مزاحم را کم می‌کند.</summary>
    public static class UiTestMode
    {
        public static bool Active =>
            string.Equals(Environment.GetEnvironmentVariable("ANBARBAN_UI_TEST"), "1", StringComparison.Ordinal);

        public static bool SuppressSuccessPopups => Active;
    }
}
