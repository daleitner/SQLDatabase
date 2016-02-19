using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public enum CompareEnum
	{
		Equals,
		NotEquals,
		GreaterThen,
		SmallerThen,
		GreaterOrEqual,
		SmallerOrEqual,
		In,
		Not_In
	}

	public class PropertyExpression : IExpression
	{
		#region members
		#endregion

		#region ctors
		public PropertyExpression(DataBaseColumn column, CompareEnum comparison, string value)
		{
			Column = column;
			Comparison = comparison;
			Value = value;
		}

		public PropertyExpression(DataBaseColumn column, CompareEnum comparison, DateTime date)
			:this(column, comparison, date.ToString("yyyy-MM-dd"))
		{
		}

		public PropertyExpression(DataBaseColumn column, CompareEnum comparison, DataBaseQuery subQuery)
		{
			Column = column;
			Comparison = comparison;
			Value = subQuery;
		}

		#endregion

		#region properties
		public DataBaseColumn Column { get; set; }
		public CompareEnum Comparison { get; set; }
		public object Value { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}