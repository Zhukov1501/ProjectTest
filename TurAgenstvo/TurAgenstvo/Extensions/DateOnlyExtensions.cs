using System;

namespace TurAgenstvo.Extensions
{
    public static class DateOnlyExtensions
    {
        // Преобразование DateOnly в DateTime
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

        // Преобразование DateTime в DateOnly
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
    }
}