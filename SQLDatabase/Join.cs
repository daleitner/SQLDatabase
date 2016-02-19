using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public enum JoinEnum
	{
		Inner,
		Left,
		Right,
		Full
	}

	public class Join
	{
		#region members
		#endregion

		#region ctors

		public Join(JoinEnum joinType, JoinTable joinTable)
		{
			JoinType = joinType;
			JoinTable = joinTable;
		}
		#endregion

		#region properties
		public JoinEnum JoinType { get; set; }
		public JoinTable JoinTable { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}