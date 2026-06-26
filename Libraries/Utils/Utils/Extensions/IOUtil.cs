using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utils.Extensions
{
    public class IOUtil
    {
        public static DateTime GetCurrentDateTime
        {
            get
            {
                DateTime date = DateTime.Now;
                return GetDateTime(date);
            }
        }
        public static DateTime GetDateTime(DateTime date)
        {

            try
            {
                string timeZoneId = Utils.Constants.COMMON.TIME_ZONE;
                if (timeZoneId == null)
                    timeZoneId = "SE Asia Standard Time";

                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, TimeZoneInfo.Local.Id, timeZoneId);
            }
            catch
            {

            }

            return date;
        }
    }
}