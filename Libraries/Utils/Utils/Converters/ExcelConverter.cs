namespace Utils.Converters
{
    using System;
    using System.Globalization;

    public static class ExcelConverter
    {
        public static bool TryParseExcelDate(object? cellValue, out DateOnly dateOnly)
        {
            dateOnly = default;

            if (cellValue is DateTime dt)
            {
                dateOnly = DateOnly.FromDateTime(dt);
                return true;
            }

            if (cellValue is double oaDate)
            {
                try
                {
                    dateOnly = DateOnly.FromDateTime(DateTime.FromOADate(oaDate));
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            string? text = cellValue?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string[] formats = {
            "M/d/yyyy h:mm:ss tt", // US style with time
            "M/d/yyyy",
            "MM/dd/yyyy",
            "dd/MM/yyyy",
            "d/M/yyyy",
            "yyyy-MM-dd",
            "d MMM yyyy",
            "dd MMM yyyy",
            "yyyyMMdd",
            "dd-MM-yyyy",
            "dd.MM.yyyy",
            "dd MMMM yyyy",
            "dddd, dd MMMM yyyy"
        };

            if (DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsed))
            {
                dateOnly = DateOnly.FromDateTime(parsed);
                return true;
            }

            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed))
            {
                dateOnly = DateOnly.FromDateTime(parsed);
                return true;
            }

            return false;
        }

        public static bool TryParseExcelTime(object? cellValue, out TimeSpan timeSpan)
        {
            timeSpan = default;

            if (cellValue is TimeSpan ts)
            {
                timeSpan = ts;
                return true;
            }

            if (cellValue is DateTime dt)
            {
                timeSpan = dt.TimeOfDay;
                return true;
            }

            if (cellValue is double oaDate)
            {
                try
                {
                    timeSpan = DateTime.FromOADate(oaDate).TimeOfDay;
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            string? text = cellValue?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return false;

       
            string[] formats = {
                "hh\\:mm", "hh\\:mm\\:ss",
                "H:mm", "HH:mm", "H:mm:ss", "HH:mm:ss",
                "h:mm tt", "hh:mm tt", "h:mm:ss tt", "hh:mm:ss tt"
            };

            if (TimeSpan.TryParseExact(text, formats, CultureInfo.InvariantCulture, out TimeSpan parsedTime))
            {
                timeSpan = parsedTime;
                return true;
            }


            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dtParsed))
            {
                timeSpan = dtParsed.TimeOfDay;
                return true;
            }

         
            if (TimeSpan.TryParse(text, out parsedTime))
            {
                timeSpan = parsedTime;
                return true;
            }

            return false;
        }


    }

}
