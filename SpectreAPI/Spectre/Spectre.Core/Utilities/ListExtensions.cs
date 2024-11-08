﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Utilities
{
    public static class ListExtensions
    {
        public static DataTable ToDataTable<T>(this List<T> iList)
        {
            DataTable dataTable = new DataTable();
            if (iList != null)
            {
                PropertyDescriptorCollection propertyDescriptorCollection =
                    TypeDescriptor.GetProperties(typeof(T));
                for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                {
                    PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                    Type type = propertyDescriptor.PropertyType;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = Nullable.GetUnderlyingType(type);


                    dataTable.Columns.Add(propertyDescriptor.Name, type);
                }
                object[] values = new object[propertyDescriptorCollection.Count];
                foreach (T iListItem in iList)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                    }
                    dataTable.Rows.Add(values);
                }
            }
            return dataTable;
        }

        public static void DisposeList<T>(this IList<T> list) where T : IDisposable
        {
            for (int index = 0; index < list.Count; index++)
            {
                list[index]?.Dispose();
            }
            list.Clear();
        }

    }
}
