using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Utils.Extensions
{
    public static class ConverterExtensions
    {
        public static string ConvertToString(object? obj, string? format = null)
        {
            string result = string.Empty;
            try
            {
                if (obj == null)
                    return result;

                object _obj = obj;
                string _format = "{0}";
                IFormatProvider? _provider = null;

                Type type = obj.GetType();

                if (type.IsEnum)
                {
                    MemberInfo[]? members = type.GetMember(_obj.ToString() ?? string.Empty);
                    if (members.Length > 0)
                    {
                        object[]? attrs = members[0].GetCustomAttributes(true);
                        foreach (object attr in attrs)
                        {
                            if (attr is DescriptionAttribute descriptionAttr)
                            {
                                _obj = descriptionAttr.Description;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    string? dformat = format;
                    if (string.IsNullOrEmpty(dformat))
                    {
                        dformat = type switch
                        {
                            { } when type == typeof(int) || type == typeof(int?) => "N0",
                            { } when type == typeof(double) || type == typeof(double?) ||
                                     type == typeof(decimal) || type == typeof(decimal?) => "N2",
                            { } when type == typeof(DateTime) || type == typeof(DateTime?) => "dd/MM/yyyy",
                            _ => dformat
                        };
                    }
                    _format = "{0:" + dformat + "}";

                    if (type == typeof(DateTime) || type == typeof(DateTime?))
                    {
                        _provider = new CultureInfo("en-US");
                    }
                }

                result = _provider != null
                    ? string.Format(_provider, _format, _obj)
                    : string.Format(_format, _obj);
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        public static string ConvertToXml_Store<T>(IEnumerable<T>? lst, params string[] fields) where T : class
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<rows></rows>");

                if (lst != null)
                {
                    Type cType = typeof(T);
                    PropertyInfo[] props = cType.GetProperties();

                    // If fields are specified, filter the properties
                    if (fields.Length > 0)
                    {
                        List<PropertyInfo> pLst = new List<PropertyInfo>();
                        foreach (string f in fields)
                        {
                            PropertyInfo? prop = cType.GetProperty(f);
                            if (prop != null)
                                pLst.Add(prop);
                        }
                        props = pLst.ToArray();
                    }

                    foreach (T obj in lst)
                    {
                        // Safely create and append the node
                        XmlNode? node = doc.CreateNode(XmlNodeType.Element, "row", "");
                        if (node == null || doc.ChildNodes.Count == 0) continue;

                        doc.ChildNodes[0].AppendChild(node);

                        foreach (PropertyInfo prop in props)
                        {
                            XmlNode? iNode = doc.CreateNode(XmlNodeType.Element, prop.Name, "");
                            if (iNode == null) continue;

                            object? value = prop.GetValue(obj, null);
                            if (value != null)
                            {
                                string val = prop.PropertyType switch
                                {
                                    { } when prop.PropertyType == typeof(bool) => ((bool)value) ? "1" : "0",
                                    { } when prop.PropertyType == typeof(bool?) => ((bool?)value)?.ToString() == "True" ? "1" : "0",
                                    { } when prop.PropertyType == typeof(DateTime) =>
                                        ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
                                    { } when prop.PropertyType == typeof(DateTime?) =>
                                        ((DateTime?)value)?.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture) ?? "",
                                    _ => value.ToString() ?? string.Empty
                                };

                                iNode.InnerText = val;
                            }
                            else
                            {
                                iNode.InnerText = string.Empty;
                            }

                            node.AppendChild(iNode);
                        }
                    }
                }

                // Use StringWriter and XmlTextWriter safely
                using StringWriter sw = new StringWriter();
                using XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);

                return sw.ToString();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error while converting to XML: {ex.Message}");
                throw; // Re-throw the exception to ensure it's not swallowed
            }
        }

        public static DataTable ConvertDoToDataTable<T>(T? objDo)
        {
            DataTable dtOut = new DataTable();
            if (objDo != null)
            {
                PropertyInfo[] pSourceInfo = objDo.GetType().GetProperties();
                foreach (PropertyInfo pInfo in pSourceInfo)
                {
                    string? strPropertyType = GetPropertyType(pInfo);
                    if (strPropertyType != null)
                    {
                        DataColumn col = new DataColumn(pInfo.Name, Type.GetType(strPropertyType) ?? typeof(string));
                        dtOut.Columns.Add(col);
                    }
                }

                DataRow row = dtOut.NewRow();
                foreach (DataColumn column in dtOut.Columns)
                {
                    PropertyInfo? pDestInfo = objDo.GetType().GetProperty(column.ColumnName);
                    object? objVal = pDestInfo?.GetValue(objDo, null);
                    row[column.ColumnName] = objVal ?? DBNull.Value;
                }
                dtOut.Rows.Add(row);
                dtOut.AcceptChanges();
            }
            return dtOut;
        }

        public static DataTable ConvertDoListToDataTable<T>(List<T>? objDoList)
        {
            DataTable dtOut = new DataTable();
            if (objDoList != null && objDoList.Count > 0)
            {
                T objSource = objDoList[0];
                PropertyInfo[] pSourceInfo = objSource.GetType().GetProperties();
                foreach (PropertyInfo pInfo in pSourceInfo)
                {
                    string? strPropertyType = GetPropertyType(pInfo);
                    if (strPropertyType != null)
                    {
                        DataColumn col = new DataColumn(pInfo.Name, Type.GetType(strPropertyType) ?? typeof(string));
                        dtOut.Columns.Add(col);
                    }
                }

                foreach (T obj in objDoList)
                {
                    DataRow row = dtOut.NewRow();
                    foreach (DataColumn column in dtOut.Columns)
                    {
                        PropertyInfo? pDestInfo = obj.GetType().GetProperty(column.ColumnName);
                        object? objVal = pDestInfo?.GetValue(obj, null);
                        row[column.ColumnName] = objVal ?? DBNull.Value;
                    }
                    dtOut.Rows.Add(row);
                }
                dtOut.AcceptChanges();
            }
            return dtOut;
        }

        public static DateTime GetCurrentDateTimeTH => GetDateTimeTH(DateTime.Now);

        public static DateTime GetDateTimeTH(DateTime date)
        {
            try
            {
                const string timeZoneId = "SE Asia Standard Time";
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, TimeZoneInfo.Local.Id, timeZoneId);
            }
            catch
            {
                // If conversion fails, return the original date
                return date;
            }
        }

        private static string? GetPropertyType(PropertyInfo pInfo)
        {
            if (pInfo.PropertyType.IsGenericType && pInfo.PropertyType.Name.Contains("Nullable"))
            {
                Type? nullableType = Nullable.GetUnderlyingType(pInfo.PropertyType);
                return nullableType?.FullName;
            }
            else if (!pInfo.PropertyType.IsGenericType)
            {
                return pInfo.PropertyType.FullName;
            }

            return null;
        }
    }
}
