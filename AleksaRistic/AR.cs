using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace AleksaRistic
{
    public static class AR
    {
        /// <summary>
        /// Merge multiple tables into one based on primary column
        /// </summary>
        /// <param name="Tables"></param>
        /// <param name="PrimaryColumn"></param>
        public static DataTable Merge(this DataTable main, DataTable Table, string PrimaryColumn)
        {
            List<string> Cols = new List<string>();
            bool multipleSameCols = false;

            /// Checking to see if there are multiple columns with same name!
            /// 
            foreach(DataColumn c1 in main.Columns)
            {
                Cols.Add(c1.ColumnName.ToUpper());
            }
            foreach(DataColumn c in Table.Columns)
            {
                if (Cols.Contains(c.ColumnName.ToUpper()))
                {
                    if (multipleSameCols)
                        throw new Exception("Cannot merge tables on primary column when there are multiple columns with same name!");

                    multipleSameCols = true;
                }
                Cols.Add(c.ColumnName.ToUpper());
            }


            DataTable dt = main.Copy();
            dt.Merge(Table);
            
            List<Tuple<int, object>> tup = new List<Tuple<int, object>>();
            bool tempSkip = false;
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                foreach(Tuple<int, object> temp in tup)
                {
                    if(temp.Item2.ToString() == dt.Rows[i][PrimaryColumn.ToUpper()].ToString())
                    {
                        foreach(DataColumn ccc in Table.Columns)
                        {
                            if (ccc.ColumnName.ToUpper() == PrimaryColumn.ToUpper())
                                continue;

                            dt.Rows[temp.Item1][ccc.ColumnName.ToUpper()] = dt.Rows[i][ccc.ColumnName.ToUpper()];
                        }
                        dt.Rows.RemoveAt(i);
                        tempSkip = true;
                    }
                }
                if(!tempSkip)
                    tup.Add(new Tuple<int, object>(i, dt.Rows[i][PrimaryColumn.ToUpper()]));
                tempSkip = false;
            }

            return dt;
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            if (table.Rows.Count < 1)
                return table;

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}