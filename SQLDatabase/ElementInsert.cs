using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public class ElementInsert
	{
		#region members
		#endregion

		#region ctors
		public ElementInsert()
		{
			Table = null;
			ColumnValues = null;
		}

		public ElementInsert(DataBaseTable table, Dictionary<DataBaseColumn, object> columnValues)
		{
			Table = table;
			ColumnValues = columnValues;
		}
		#endregion

		#region properties
		public DataBaseTable Table { get; set; }
		public Dictionary<DataBaseColumn, object> ColumnValues { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}