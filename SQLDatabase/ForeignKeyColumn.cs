using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public class ForeignKeyColumn : DataBaseColumn
	{
		#region members
		#endregion

		#region ctors
		public ForeignKeyColumn(string name, ColumnType type, string referenceTable, string referenceColumn)
			:base(name, type, false)
		{
			this.ReferenceTable = referenceTable;
			this.ReferenceColumn = referenceColumn;
		}
		#endregion

		#region properties
		public string ReferenceTable { get; set; }
		public string ReferenceColumn { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}