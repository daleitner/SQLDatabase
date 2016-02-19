using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public enum SortEnum
	{
		ASC,
		DESC
	}
	public class DataBaseQuery
	{
		#region members
		#endregion

		#region ctors
		public DataBaseQuery(DataBaseTable table)
			:this(null, table, null, null, null, false)
		{
		}

		public DataBaseQuery(DataBaseTable table, Condition condition)
			:this(null, table, null, condition, null, false)
		{
		}

		public DataBaseQuery(List<DataBaseColumn> columns, DataBaseTable table, Condition condition)
			:this(columns, table, null, condition, null, false)
		{
		}

		public DataBaseQuery(List<DataBaseColumn> columns, DataBaseTable table, Join join, Condition condition)
			: this(columns, table, new List<Join>(){join}, condition, null, false)
		{
		}

		public DataBaseQuery(List<DataBaseColumn> columns, DataBaseTable table, List<Join> join, Condition condition)
			: this(columns, table, join, condition, null, false)
		{
		}

		public DataBaseQuery(List<DataBaseColumn> columns, DataBaseTable table, Condition condition, Dictionary<DataBaseColumn, SortEnum> sortColumns)
			:this(columns, table, null, condition, sortColumns, false)
		{
		}

		public DataBaseQuery(List<DataBaseColumn> columns, DataBaseTable table, List<Join> joins, Condition condition, Dictionary<DataBaseColumn, SortEnum> sortColumns, bool distinct)
		{
			this.Columns = columns;
			this.Table = table;
			this.Joins = joins;
			this.Condition = condition;
			this.SortColumns = sortColumns;
			this.Distinct = distinct;
		}
		#endregion

		#region properties
		public List<DataBaseColumn> Columns { get; set; }
		public Condition Condition { get; set; }
		public DataBaseTable Table { get; set; }
		public List<Join> Joins { get; set; }
		public Dictionary<DataBaseColumn, SortEnum> SortColumns { get; set; }
		public bool Distinct { get; set; }
		#endregion

		#region public methods
		#endregion

		#region private methods
		#endregion
	}
}
