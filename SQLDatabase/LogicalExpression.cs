using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public enum LogicalEnum
	{
		AND,
		OR
	}

	public class LogicalExpression : IExpression
	{
		#region ctors
		public LogicalExpression(LogicalEnum type)
		{
			this.LogicalType = type;
			this.Expressions = new List<PropertyExpression>();
			this.LogicExpressions = new List<LogicalExpression>();
		}
		#endregion

		#region properties
		public LogicalEnum LogicalType { get; set; }
		public List<LogicalExpression> LogicExpressions { get; set; }
		public List<PropertyExpression> Expressions { get; set; }
		#endregion

		#region public methods
		public LogicalExpression Add(PropertyExpression propertyExpression)
		{
			Expressions.Add(propertyExpression);
			return this;
		}

		public LogicalExpression Add(LogicalExpression logicalExpression)
		{
			LogicExpressions.Add(logicalExpression);
			return this;
		}

		//public override string ToString()
		//{
		//	string s = "(";
		//	for (int i = 0; i < this.Expressions.Count; i++ )
		//	{
		//		s += this.Expressions[i].ToString();
		//		if(i < this.Expressions.Count-1)
		//			s += " " + this.LogicalType.ToString() + " ";
		//	}
		//	if (LogicExpression != null)
		//	{
		//		if(s.Length > 1)
		//			s += " " + this.LogicalType.ToString() + " ";
		//		s += LogicExpression.ToString();
		//	}
		//	s += ")";
		//	return s;
		//}
		#endregion
	}
}
