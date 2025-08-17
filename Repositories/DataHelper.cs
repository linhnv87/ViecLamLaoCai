using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories;

public static class DataHelper
{
    public static List<T> ConvertToList<T>(List<Dictionary<string, object>> resultSet) where T : class, new()
    {
        var list = new List<T>();
        var properties = typeof(T).GetProperties();

        foreach (var row in resultSet)
        {
            var instance = new T();
            foreach (var property in properties)
            {
                if (row.ContainsKey(property.Name) && row[property.Name] != DBNull.Value)
                {
                    property.SetValue(instance, row[property.Name]);
                }
            }
            list.Add(instance);
        }

        return list;
    }
}
