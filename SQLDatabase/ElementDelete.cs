using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public class ElementDelete
	{
		#region members
		#endregion

		#region ctors
		public ElementDelete(DataBaseTable table)
			:this(table, null)
		{
		}

		public ElementDelete(DataBaseTable table, Condition condition)
		{
			this.Table = table;
			this.Condition = condition;
		}
		#endregion

		#region properties
		public DataBaseTable Table { get; set; }
		public Condition Condition { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
	}
}
