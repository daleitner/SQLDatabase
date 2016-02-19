using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public class JoinTable
	{
		#region members
		#endregion

		#region ctors

		public JoinTable(DataBaseTable table, DataBaseColumn mainColumn, DataBaseColumn joinColumn)
		{
			this.Table = table;
			this.MainColumn = mainColumn;
			this.JoinColumn = joinColumn;
		}
		#endregion

		#region properties
		public DataBaseTable Table { get; set; }
		public DataBaseColumn MainColumn { get; set; }
		public DataBaseColumn JoinColumn { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}