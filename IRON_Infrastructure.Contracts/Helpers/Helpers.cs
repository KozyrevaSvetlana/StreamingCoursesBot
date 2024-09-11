using NPOI.SS.UserModel;
using System.Reflection;

namespace StreamingCourses_Contracts.Helpers
{
    public static class Helpers
    {
        public static T? DeserializeObject<T>(List<ICell> cells) where T : new()
        {
            var model = new T();
            var type = typeof(T);
            var baseProperties = type.BaseType?.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var derivedProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var properties = baseProperties?.Union(derivedProperties).DistinctBy(x => x.Name).ToArray();

            for (int i = 0; i < properties.Length && i < cells.Count; i++)
            {
                var value = cells[i];
                var property = properties[i];
                if (property.CanWrite)
                {
                    object? convertedValue = GetCellValueByPropertyType(property, value);
                    if (convertedValue != null)
                    {
                        property.SetValue(model, convertedValue);
                    }
                }
            }
            return model;
        }

        private static object? GetCellValueByPropertyType(PropertyInfo property, ICell value)
        {
            switch (value.CellType)
            {
                case CellType.Unknown:
                case CellType.Formula:
                case CellType.Blank:
                case CellType.Error:
                    return null;
                case CellType.Numeric:
                    if (property.PropertyType.Name == "DateTime")
                        return value.DateCellValue;
                    if (property.PropertyType.Name == "Double")
                        return value.NumericCellValue;
                    if (property.PropertyType.Name == "Int32")
                        return (int)value.NumericCellValue;
                    return null;
                case CellType.String:
                    var result = property.PropertyType.Name == "String" ? value.StringCellValue : null;
                    return result?.Trim() ?? string.Empty;
                case CellType.Boolean:
                    return property.PropertyType.Name == "Boolean" ? value.BooleanCellValue : null;
                default:
                    return null;
            }
        }
    }
}
