using System;

namespace TurAgenstvo.Helpers
{
    public static class DateHelper
    {
        // Для преобразования DateTime -> DateOnly (при сохранении)
        public static DateOnly ToDateOnly(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        public static DateOnly? ToDateOnly(this DateTime? dateTime)
        {
            if (dateTime == null)
                return null;
            return DateOnly.FromDateTime(dateTime.Value);
        }

        // Для преобразования DateOnly -> DateTime (для отображения)
        public static DateTime ToDateTime(this DateOnly dateOnly)
        {
            return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day);
        }

        public static DateTime? ToDateTime(this DateOnly? dateOnly)
        {
            if (dateOnly == null)
                return null;
            return new DateTime(dateOnly.Value.Year, dateOnly.Value.Month, dateOnly.Value.Day);
        }
    }
}