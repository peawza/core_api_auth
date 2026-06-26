namespace Utils.Helper
{
    public class LanguageHelper
    {
        public static string GetDisplayName(object item, string nameColumn, string languageCode)
        {
            string propertyName = $"{nameColumn}_{languageCode.ToUpper()}";

            var value = item.GetType().GetProperty(propertyName)?.GetValue(item)?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                var defaultValue = item.GetType().GetProperty($"{nameColumn}_EN")?.GetValue(item)?.ToString();
                return defaultValue ?? string.Empty; // คืนค่า Default หรือ string.Empty หากไม่มี Default
            }
            return value;
        }
    }
}
