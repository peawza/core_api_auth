using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using Utils.Extensions;

namespace Utils.SqlServer
{
    public static class SqlParameterHelper
    {

        public static string CallStoredProcedure(string name, SqlParameter[]? sqlparameter)
        {
            int size = sqlparameter?.Length ?? 0;
            var parameterString = string.Join(", ", Enumerable.Range(0, size).Select(i => $"{{{i}}}"));
            var sqlQueryStoredProcedure = $"EXEC {name} {parameterString}";
            return sqlQueryStoredProcedure;
        }
        public static SqlParameter Create(string name, DateTime? value)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.DateTime) { SqlValue = value.Value } : new SqlParameter(name, SqlDbType.DateTime) { SqlValue = DBNull.Value };
        }

        public static SqlParameter Create(string name, DateOnly? value)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.Date) { SqlValue = value.Value } : new SqlParameter(name, SqlDbType.Date) { SqlValue = DBNull.Value };
        }

        public static SqlParameter Create(string name, string value, bool encode, int size)
        {
            var dbType = encode ? SqlDbType.NVarChar : SqlDbType.VarChar;
            return value == null ? new SqlParameter(name, dbType, size) { SqlValue = DBNull.Value } : new SqlParameter(name, dbType, size) { SqlValue = value };
        }

        public static SqlParameter Create(string name, Guid value)
        {
            return new SqlParameter(name, SqlDbType.UniqueIdentifier) { SqlValue = value };
        }

        public static SqlParameter Create(string name, int value)
        {
            return new SqlParameter(name, SqlDbType.Int) { SqlValue = value };
        }

        public static SqlParameter Create(string name, int? value)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.Int) { SqlValue = value } : new SqlParameter(name, SqlDbType.Int) { SqlValue = DBNull.Value };
        }

        public static SqlParameter Create(string name, long value)
        {
            return new SqlParameter(name, SqlDbType.BigInt) { SqlValue = value };
        }
        public static SqlParameter Create(string name, double value)
        {
            return new SqlParameter(name, SqlDbType.Float) { SqlValue = value };
        }

        public static SqlParameter Create(string name, decimal value)
        {
            return new SqlParameter(name, SqlDbType.Decimal) { SqlValue = value };
        }

        public static SqlParameter Create(string name, byte[] value)
        {
            return new SqlParameter(name, SqlDbType.Binary) { SqlValue = value };
        }

        public static SqlParameter Create(string name, decimal? value)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.Decimal) { SqlValue = value } : new SqlParameter(name, SqlDbType.Decimal) { SqlValue = DBNull.Value };
        }

        public static SqlParameter Create(string name, decimal? value, byte precision, byte scale)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.Decimal) { Precision = precision, Scale = scale, SqlValue = value.Value }
            : new SqlParameter(name, SqlDbType.Decimal) { Precision = precision, Scale = scale, SqlValue = DBNull.Value };
        }

        public static SqlParameter Create(string name, TimeSpan? value)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.Time) { SqlValue = value.Value }
            : new SqlParameter(name, SqlDbType.Time) { SqlValue = DBNull.Value };
        }

        public static SqlParameter CreateXmlString<T>(string name, IEnumerable<T> data)
            where T : class
        {
            string xml = ConverterExtensions.ConvertToXml_Store(data);
            return string.IsNullOrEmpty(xml) ? new SqlParameter(name, SqlDbType.Xml) { SqlValue = DBNull.Value }
            : new SqlParameter(name, SqlDbType.Xml) { SqlValue = xml };
        }

        public static SqlParameter Create(string name, bool? value)
        {
            return value.HasValue ? new SqlParameter(name, SqlDbType.Bit) { SqlValue = value.Value }
            : new SqlParameter(name, SqlDbType.Bit) { SqlValue = DBNull.Value };
        }

        public static SqlParameter CreateOutput(string name, SqlDbType dbType = SqlDbType.Int)
        {
            var p = new SqlParameter(name, dbType);
            p.Direction = ParameterDirection.Output;
            return p;
        }
        public static SqlParameter CreateIntegerOutput(string name, int? value)
        {
            //var p = new SqlParameter(name,SqlDbType.Int) { SqlValue = value } : new SqlParameter(name, SqlDbType.Int) { SqlValue = DBNull.Value }; ;
            var p = value.HasValue ? new SqlParameter(name, SqlDbType.Int) { SqlValue = value } : new SqlParameter(name, SqlDbType.Int) { SqlValue = DBNull.Value };
            p.Direction = ParameterDirection.Output;
            return p;
        }
        public static SqlParameter CreateStringOutput(string name, int size, bool encode)
        {
            var p = new SqlParameter(name, encode ? SqlDbType.NVarChar : SqlDbType.VarChar, size);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        public static SqlParameter CreateString(string name, string value, bool encode = true, int size = 50)
        {
            return Create(name, value, encode, size);
        }

        public static SqlParameter CreateUser(string name, string value)
        {
            return Create(name, value, true, 20);
        }

        public static SqlParameter Create(string name, string[] values, int size = 4000)
        {
            if (values == null || values.Length == 0)
            {
                return new SqlParameter(name, SqlDbType.VarChar) { SqlValue = DBNull.Value };
            }
            return Create(name, String.Join(",", values), true, size);
        }

        public static SqlParameter Create(string name, Guid[] values, int size = 8000)
        {
            if (values == null || values.Length == 0)
            {
                return new SqlParameter(name, SqlDbType.VarChar) { SqlValue = DBNull.Value };
            }

            var v = String.Join(",", values);
            if (v.Length > size)
            {
                throw new Exception($"Invalid parameter value size. Current Length: {v.Length}, Max Size: {size}.");
            }
            return Create(name, v, false, size);
        }


        public static SqlParameter Create(string name, string? value)
        {
            var dbType = SqlDbType.NVarChar;
            if (value == "")
            {
                value = null;
            }
            return value == null ? new SqlParameter(name, dbType, -1) { SqlValue = DBNull.Value } : new SqlParameter(name, dbType, -1) { SqlValue = value };

        }



        public static List<T> ConvertDataTableToList<T>(DataTable dt) where T : class, new()
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var properties = typeof(T).GetProperties();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        try
                        {
                            var value = row[pro.Name];
                            if (value != DBNull.Value)
                            {
                                // Handle DateTime conversion
                                if (pro.PropertyType == typeof(DateTime) || pro.PropertyType == typeof(DateTime?))
                                {
                                    pro.SetValue(objT, Convert.ChangeType(value, pro.PropertyType));
                                }
                                // Handle DateOnly conversion
                                else if (pro.PropertyType == typeof(DateOnly) || pro.PropertyType == typeof(DateOnly?))
                                {
                                    DateTime dateValue;
                                    if (DateTime.TryParse(value.ToString(), out dateValue))
                                    {
                                        var dateOnlyValue = DateOnly.FromDateTime(dateValue);
                                        pro.SetValue(objT, dateOnlyValue);
                                    }
                                }
                                // Handle decimal conversion
                                else if (pro.PropertyType == typeof(decimal) || pro.PropertyType == typeof(decimal?))
                                {
                                    if (decimal.TryParse(value.ToString(), out decimal decimalValue))
                                    {
                                        pro.SetValue(objT, decimalValue);
                                    }
                                }
                                // Handle int conversion
                                else if (pro.PropertyType == typeof(int) || pro.PropertyType == typeof(int?))
                                {
                                    if (int.TryParse(value.ToString(), out int intValue))
                                    {
                                        pro.SetValue(objT, intValue);
                                    }
                                }
                                // Handle long conversion
                                else if (pro.PropertyType == typeof(long) || pro.PropertyType == typeof(long?))
                                {
                                    if (long.TryParse(value.ToString(), out long longValue))
                                    {
                                        pro.SetValue(objT, longValue);
                                    }
                                }
                                // Handle float conversion
                                else if (pro.PropertyType == typeof(float) || pro.PropertyType == typeof(float?))
                                {
                                    if (float.TryParse(value.ToString(), out float floatValue))
                                    {
                                        pro.SetValue(objT, floatValue);
                                    }
                                }
                                else
                                {
                                    pro.SetValue(objT, Convert.ChangeType(value, pro.PropertyType));
                                }
                            }
                            else if (pro.PropertyType == typeof(DateTime?) || pro.PropertyType == typeof(DateOnly?) ||
                                     pro.PropertyType == typeof(decimal?) || pro.PropertyType == typeof(int?) ||
                                     pro.PropertyType == typeof(long?) || pro.PropertyType == typeof(float?))
                            {
                                // Handle DBNull for nullable types
                                pro.SetValue(objT, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the exception or handle accordingly
                            // Example: Console.WriteLine($"Error converting DataTable to model: {ex.Message}");
                        }
                    }
                }
                return objT;
            }).ToList();
        }

        public static async Task<T?> ConvertDataTableToModel<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return await Task.Run(() =>
            {
                try
                {
                    var row = dt.Rows[0];
                    var model = new T();

                    foreach (var prop in properties)
                    {
                        if (columnNames.Contains(prop.Name))
                        {
                            var value = row[prop.Name];
                            if (value != DBNull.Value)
                            {
                                if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                                {
                                    if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
                                    {
                                        prop.SetValue(model, dateValue);
                                    }
                                }
                                else if (prop.PropertyType == typeof(DateOnly) || prop.PropertyType == typeof(DateOnly?))
                                {
                                    if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
                                    {
                                        var dateOnlyValue = DateOnly.FromDateTime(dateValue);
                                        prop.SetValue(model, dateOnlyValue);
                                    }
                                }
                                else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                                {
                                    if (decimal.TryParse(value.ToString(), out decimal decimalValue))
                                    {
                                        prop.SetValue(model, decimalValue);
                                    }
                                }
                                else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                                {
                                    if (int.TryParse(value.ToString(), out int intValue))
                                    {
                                        prop.SetValue(model, intValue);
                                    }
                                }
                                else if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                                {
                                    if (long.TryParse(value.ToString(), out long longValue))
                                    {
                                        prop.SetValue(model, longValue);
                                    }
                                }
                                else if (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(float?))
                                {
                                    if (float.TryParse(value.ToString(), out float floatValue))
                                    {
                                        prop.SetValue(model, floatValue);
                                    }
                                }
                                else
                                {
                                    prop.SetValue(model, Convert.ChangeType(value, prop.PropertyType));
                                }
                            }
                            else if (prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateOnly?))
                            {
                                prop.SetValue(model, null);
                            }
                        }
                    }

                    return model;
                }
                catch (Exception ex)
                {
                    // Log the exception or handle accordingly
                    // Example: Console.WriteLine($"Error converting DataTable to model: {ex.Message}");
                    return null;
                }
            });
        }
    }
}
