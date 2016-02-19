using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public class ElementUpdate
	{
		#region members
		#endregion

		#region ctors
		public ElementUpdate(DataBaseTable table, Dictionary<DataBaseColumn, object> columnValues)
			:this(table, columnValues, null)
		{
		}

		public ElementUpdate(DataBaseTable table, Dictionary<DataBaseColumn, object> columnValues, Condition condition)
		{
			this.Table = table;
			this.ColumnValues = columnValues;
			this.Condition = condition;
		}
		#endregion

		#region properties
		public DataBaseTable Table { get; set; }
		public Dictionary<DataBaseColumn, object> ColumnValues { get; set; }
		public Condition Condition { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}