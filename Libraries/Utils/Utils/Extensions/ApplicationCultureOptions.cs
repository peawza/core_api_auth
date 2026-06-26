using System.Globalization;

namespace Utils.Extensions
{
    public class ApplicationCultureOptions
    {
        public Dictionary<string, CultureFormatInfo> Cultures { get; set; } = new Dictionary<string, CultureFormatInfo>();

        public CultureFormatInfo Get(string name)
        {
            var options = new CultureFormatInfo();
            if (!this.Cultures.TryGetValue(name, out options))
            {
                options = new CultureFormatInfo();
            }
            return options;
        }

        public CultureInfo GetCultureInfo(string name)
        {
            var options = new CultureFormatInfo();
            if (!this.Cultures.TryGetValue(name, out options))
            {
                options = new CultureFormatInfo();
            }
            return options.ToCultureInfo(name);
        }

        public CultureInfo[] GetCultures(params string[] supportedCultures)
        {
            var cultures = new List<CultureInfo>();
            supportedCultures = supportedCultures ?? this.Cultures.Keys.ToArray();
            foreach (var key in supportedCultures)
            {
                var culture = CultureInfo.GetCultureInfo(key).Clone() as CultureInfo;
                if (this.Cultures.ContainsKey(key))
                {
                    culture.DateTimeFormat.ShortDatePattern = this.Cultures[key].ShortDatePattern ?? culture.DateTimeFormat.ShortDatePattern;
                    culture.DateTimeFormat.ShortTimePattern = this.Cultures[key].ShortTimePattern ?? culture.DateTimeFormat.ShortTimePattern;
                    culture.DateTimeFormat.LongDatePattern = this.Cultures[key].LongDatePattern ?? culture.DateTimeFormat.LongDatePattern;
                    culture.DateTimeFormat.LongTimePattern = this.Cultures[key].LongTimePattern ?? culture.DateTimeFormat.LongTimePattern;
                    culture.DateTimeFormat.FullDateTimePattern = this.Cultures[key].FullDateTimePattern ?? culture.DateTimeFormat.FullDateTimePattern;
                }
                cultures.Add(culture);
            }
            return cultures.ToArray();
        }
    }

    public class CultureFormatInfo
    {
        public string ShortDatePattern { get; set; }
        public string ShortTimePattern { get; set; }
        public string LongDatePattern { get; set; }
        public string LongTimePattern { get; set; }
        public string FullDateTimePattern { get; set; }

        public CultureInfo ToCultureInfo(string culture)
        {
            var c = CultureInfo.GetCultureInfo(culture).Clone() as CultureInfo;
            c.DateTimeFormat.ShortDatePattern = this.ShortDatePattern;
            c.DateTimeFormat.ShortTimePattern = this.ShortTimePattern;
            c.DateTimeFormat.LongDatePattern = this.LongDatePattern;
            c.DateTimeFormat.LongTimePattern = this.LongTimePattern;
            c.DateTimeFormat.FullDateTimePattern = this.FullDateTimePattern;
            return c;
        }
    }
}
