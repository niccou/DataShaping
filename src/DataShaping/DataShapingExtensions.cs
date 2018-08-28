using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataShaping
{
    public static class DataShapingExtensions
    {
        public static IDictionary<string, object> ShapeData<T>(this T dataToshape, string fields)
        {
            var result = new Dictionary<string, object>();
            if (dataToshape == null)
            {
                return result;
            }

            var propertyInfoList = GetPropertyInfos<T>(fields);

            result.FillDictionary(dataToshape, propertyInfoList);

            return result;
        }

        public static IDictionary<string, object> ShapeDataList<T>(this IEnumerable<T> dataToshape, string fields, string listName = "values")
        {
            var result = new Dictionary<string, object>();
            if (dataToshape == null)
            {
                return result;
            }

            var propertyInfoList = GetPropertyInfos<T>(fields);

            var list = dataToshape.Select(line =>
            {
                var data = new Dictionary<string, object>();
                data.FillDictionary(line, propertyInfoList);
                return data;
            }).Where(d => d.Keys.Any()).ToList();

            result[listName] = list;

            return result;
        }

        private static void FillDictionary<T>(this IDictionary<string, object> dictionary, T source, IEnumerable<PropertyInfo> fields)
        {
            foreach (var propertyInfo in fields)
            {
                var propertyValue = propertyInfo.GetValue(source);

                dictionary.Add(propertyInfo.Name, propertyValue);
            }
        }

        private static IEnumerable<PropertyInfo> GetPropertyInfos<T>(string fields)
        {
            var propertyInfoList = new List<PropertyInfo>();

            if (!string.IsNullOrWhiteSpace(fields))
            {
                return ExtractSelectedPropertiesInfo<T>(fields, propertyInfoList);
            }

            var propertyInfos = typeof(T).GetRuntimeProperties();
            propertyInfoList.AddRange(propertyInfos);
            return propertyInfoList;
        }

        private static IEnumerable<PropertyInfo> ExtractSelectedPropertiesInfo<T>(string fields, List<PropertyInfo> propertyInfoList)
        {
            var fieldsAfterSplit = fields.Split(',');

            foreach (var propertyName in fieldsAfterSplit.Select(f => f.Trim()))
            {
                var propertyInfo = typeof(T).GetRuntimeProperty(propertyName);

                if (propertyInfo == null)
                {
                    continue;
                }

                propertyInfoList.Add(propertyInfo);
            }

            return propertyInfoList;
        }
    }
}
