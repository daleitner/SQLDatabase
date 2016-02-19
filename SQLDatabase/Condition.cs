using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
    public class Condition
    {
        public Condition()
        {
        }

		public LogicalExpression Expression { get; set; }

		public Condition Add(LogicalExpression expression)
		{
			this.Expression = expression;
			return this;
		}

		public Condition Add(PropertyExpression propertyExpression)
		{
			this.Expression = new LogicalExpression(LogicalEnum.AND).Add(propertyExpression);
			return this;
		}

		//public override string ToString()
		//{
		//	return Expression.ToString();
		//}
    }
}
