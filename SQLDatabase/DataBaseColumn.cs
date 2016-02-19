using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public enum ColumnType
	{
		VARCHAR,
		INTEGER,
		DATE
	}
	public class DataBaseColumn
	{
		#region members
		#endregion

		#region ctors
		public DataBaseColumn(string name, ColumnType type)
		{
			Name = name;
			Type = type;
			IsPrimaryKey = false;
		}

		public DataBaseColumn(string name, ColumnType type, bool isPrimaryKey)
		{
			Name = name;
			Type = type;
			IsPrimaryKey = isPrimaryKey;
		}
		#endregion

		#region properties
		public string Name { get; set; }
		public ColumnType Type { get; set; }
		public bool IsPrimaryKey { get; set; }
		public DataBaseTable Table { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}