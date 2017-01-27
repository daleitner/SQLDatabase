using SQLDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public class DataBaseConnection
	{
		#region members
		private DataBase dataBase = null;
		#endregion

		#region ctors
		public DataBaseConnection(string dataBase)
		{
			this.dataBase = DataBase.GetInstance();
			Initialize(dataBase);
		}

		public DataBaseConnection(string dataBase, List<DataBaseTable> dataBaseTables, List<string> testValueInsertCommands)
		{
			this.dataBase = DataBase.GetInstance();
			if (!this.dataBase.LoadDataBase(dataBase))
			{
				this.dataBase.CreateDataBase(dataBase);
				if (dataBaseTables != null)
				{
					foreach (DataBaseTable table in dataBaseTables)
					{
						CreateTable(table);
					}
				}
				if (testValueInsertCommands != null)
				{
					foreach (string s in testValueInsertCommands)
					{
						string cmd = ResetUmlaute(s);
						this.dataBase.ExecuteCommand(cmd);
					}
				}
			}
		}

		public DataBaseConnection(IDataBaseRepository rep, string dataBase)
		{
			this.dataBase = DataBase.GetInstance(rep);
			Initialize(dataBase);

		}
		#endregion

		#region dtors
		~DataBaseConnection()
		{
			this.dataBase.CloseConnection();
		}
		#endregion

		#region public methods
		public void CreateTable(DataBaseTable table)
		{
			string command = GetCreateTableCommandString(table);
			this.dataBase.ExecuteCommand(command);
		}

		public void InsertElement(ElementInsert insert)
		{
			string command = GetInsertCommandString(insert);
			command = ResetUmlaute(command);
			this.dataBase.ExecuteCommand(command);
		}

		public void DeleteElement(ElementDelete delete)
		{
			string command = GetDeleteCommand(delete);
			command = ResetUmlaute(command);
			this.dataBase.ExecuteCommand(command);
		}

		public void UpdateElement(ElementUpdate update)
		{
			string command = GetUpdateCommand(update);
			command = ResetUmlaute(command);
			this.dataBase.ExecuteCommand(command);
		}

		public List<List<string>> ExecuteQuery(DataBaseQuery query)
		{
			string queryString = GetQueryString(query);
			return ExecuteQuery(queryString);
		}

		public List<List<string>> ExecuteQuery(string query)
		{
			string queryString = ResetUmlaute(query);
			List<List<string>> ret = this.dataBase.ExecuteQuery(queryString);
			if (ret != null)
			{
				for (int i = 0; i < ret.Count; i++)
				{
					for (int j = 0; j < ret[i].Count; j++)
					{
						ret[i][j] = SetUmlaute(ret[i][j]);
					}
				}
			}
			return ret;
		}

		public void ExecuteCommand(string statement)
		{
			this.dataBase.ExecuteCommand(statement);
		}
		#endregion

		#region private methods
		private void Initialize(string dataBase)
		{
			this.dataBase.CreateDataBaseIfNotExists(dataBase);
			this.dataBase.OpenConnection();
		}

		private string GetCreateTableCommandString(DataBaseTable table)
		{
			string ret = "CREATE TABLE ";
			ret += table.Name + " (";
			List<DataBaseColumn> dataColumns = table.GetDataColumns();
			List<ForeignKeyColumn> foreignKeyColumns = table.GetForeignKeyColumns();
			for (int i = 0; i < dataColumns.Count; i++)
			{
				ret += dataColumns[i].Name + " " + dataColumns[i].Type.ToString();
				if (dataColumns[i].Type.Equals(ColumnType.VARCHAR))
					ret += "(32)";
				ret += ", ";
			}

			if (foreignKeyColumns != null)
			{
				foreach (ForeignKeyColumn col in foreignKeyColumns)
				{
					ret += col.Name + " " + col.Type.ToString();
					if (col.Type.Equals(ColumnType.VARCHAR))
						ret += "(32)";
					ret += ", ";
				}
				ret = ret.Substring(0, ret.Length - 2);
			}
			else
				ret = ret.Substring(0, ret.Length - 2);

			foreach (DataBaseColumn col in dataColumns)
			{
				if (col.IsPrimaryKey)
					ret += ", PRIMARY KEY(" + col.Name + ")";
			}

			if (foreignKeyColumns != null)
			{
				foreach (ForeignKeyColumn col in foreignKeyColumns)
				{
					ret += ", FOREIGN KEY(" + col.Name + ") ";
					ret += "REFERENCES " + col.ReferenceTable + "(" + col.ReferenceColumn + ")";
				}
			}
			ret += ");";
			return ret;
		}

		private string GetInsertCommandString(ElementInsert insert)
		{
			string ret = "INSERT INTO " + insert.Table.Name + " (";
			foreach (DataBaseColumn col in insert.ColumnValues.Keys)
			{
				ret += col.Name + ", ";
			}
			ret = ret.Substring(0, ret.Length - 2);
			ret += ") VALUES (";
			foreach (DataBaseColumn col in insert.ColumnValues.Keys)
			{
				if (col.Type == ColumnType.INTEGER)
					ret += insert.ColumnValues[col].ToString() + ", ";
				else if (col.Type == ColumnType.DATE)
				{
					DateTime date = (DateTime)insert.ColumnValues[col];
					ret += "\"" + date.ToString("yyyy-MM-dd") + "\", ";
				}
				else
					ret += "\"" + insert.ColumnValues[col].ToString() + "\", ";
			}
			ret = ret.Substring(0, ret.Length - 2);
			ret += ");";
			return ret;
		}

		private string GetDeleteCommand(ElementDelete delete)
		{
			string ret = "DELETE FROM ";
			ret += delete.Table.Name;
			if (delete.Condition != null)
			{
				ret += " WHERE ";
				ret += GetLogicalExpressionString(delete.Condition.Expression);
			}
			ret += ";";
			return ret;
		}

		private string GetUpdateCommand(ElementUpdate update)
		{
			string ret = "UPDATE ";
			ret += update.Table.Name + " SET ";
			foreach (DataBaseColumn col in update.ColumnValues.Keys)
			{
				ret += col.Name + "=";
				if (col.Type == ColumnType.INTEGER)
					ret += update.ColumnValues[col].ToString();
				else if (col.Type == ColumnType.DATE)
				{
					DateTime date = (DateTime)update.ColumnValues[col];
					ret += "\"" + date.ToString("yyyy-MM-dd") + "\"";
				}
				else
					ret += "\"" + update.ColumnValues[col].ToString() + "\"";
				ret += ", ";
			}
			ret = ret.Substring(0, ret.Length - 2);
			if (update.Condition != null)
			{
				ret += " WHERE ";
				ret += GetLogicalExpressionString(update.Condition.Expression);
			}
			ret += ";";
			return ret;
		}

		private string GetQueryString(DataBaseQuery query)
		{
			List<DataBaseTable> tables = new List<DataBaseTable>() { query.Table };
			if (query.Joins != null)
			{
				foreach (Join j in query.Joins)
				{
					tables.Add(j.JoinTable.Table);
				}
			}

			string ret = "SELECT ";
			if (query.Distinct)
				ret += "DISTINCT ";
			if (query.Columns != null)
			{
				foreach (DataBaseColumn c in query.Columns)
				{
					ret += (String.IsNullOrEmpty(c.Table.Alias) ? c.Table.Name : c.Table.Alias) + "." + c.Name + ", ";
				}
				ret = ret.Substring(0, ret.Length - 2);
			}
			else
			{
				ret += "*";
			}
			ret += " FROM ";
			ret += query.Table.Name;
			if (!String.IsNullOrEmpty(query.Table.Alias))
				ret += " " + query.Table.Alias;
			if (query.Joins != null && query.Joins.Count > 0)
			{
				foreach(Join j in query.Joins)
				{
					ret += " " + j.JoinType.ToString().ToUpper() + " JOIN " + j.JoinTable.Table.Name;
					if (!String.IsNullOrEmpty(j.JoinTable.Table.Alias))
						ret += " " + j.JoinTable.Table.Alias;
					ret += " ON ";
					DataBaseTable jtbl = j.JoinTable.MainColumn.Table;
					if(jtbl != null)
						ret += (String.IsNullOrEmpty(jtbl.Alias) ? jtbl.Name : jtbl.Alias);
					else
						ret += (String.IsNullOrEmpty(query.Table.Alias) ? query.Table.Name : query.Table.Alias);
					ret += "." + j.JoinTable.MainColumn.Name + "=";
					ret += (String.IsNullOrEmpty(j.JoinTable.Table.Alias) ? j.JoinTable.Table.Name : j.JoinTable.Table.Alias); 
					ret += "." + j.JoinTable.JoinColumn.Name;
				}
			}
			if (query.Condition != null && query.Condition.Expression != null &&
				(query.Condition.Expression.Expressions.Count > 0 || query.Condition.Expression.LogicExpressions.Count > 0))
			{
				ret += " WHERE ";
				ret += GetLogicalExpressionString(tables, query.Condition.Expression);
			}
			if (query.SortColumns != null && query.SortColumns.Count > 0)
			{
				ret += " ORDER BY ";
				foreach (DataBaseColumn col in query.SortColumns.Keys)
				{
					ret += (string.IsNullOrEmpty(query.Table.Alias) ? query.Table.Name : query.Table.Alias) + "." + col.Name;
					if (query.SortColumns[col] == SortEnum.DESC)
						ret += " " + query.SortColumns[col].ToString();
					ret += ", ";
				}
				ret = ret.Substring(0, ret.Length - 2);
			}
			ret += ";";
			return ret;
		}

		private string GetLogicalExpressionString(LogicalExpression logicalExpression)
		{
			return GetLogicalExpressionString(null, logicalExpression);
		}
		private string GetLogicalExpressionString(List<DataBaseTable> tables, LogicalExpression logicalExpression)
		{
			string ret = "";
			if (logicalExpression.LogicExpressions.Count > 0 || logicalExpression.Expressions.Count > 1)
				ret += "(";
			List<LogicalExpression> lexp = logicalExpression.LogicExpressions;
			for (int i = 0; i < lexp.Count; i++)
			{
				ret += GetLogicalExpressionString(tables, lexp[i]);
				if (i < lexp.Count - 1 || logicalExpression.Expressions.Count > 0)
					ret += " " + logicalExpression.LogicalType.ToString() + " ";
			}
			List<PropertyExpression> exp = logicalExpression.Expressions;
			for (int i = 0; i < exp.Count; i++)
			{
				if (tables != null)
				{
					foreach (DataBaseTable table in tables)
					{
						if(table.Columns.Keys.Contains(exp[i].Column.Name) && table.Columns[exp[i].Column.Name] == exp[i].Column)
							ret += (String.IsNullOrEmpty(table.Alias) ? table.Name : table.Alias) + ".";
					}
				}
				ret += exp[i].Column.Name + CompareEnumToSign(exp[i].Comparison);
				if ((exp[i].Comparison == CompareEnum.In || exp[i].Comparison == CompareEnum.Not_In) && exp[i].Value.GetType() == typeof(DataBaseQuery))
				{
					ret += "(";
					ret += GetQueryString((DataBaseQuery)exp[i].Value);
					ret = ret.Substring(0, ret.Length - 1);
					ret += ")";
				}
				else
				{
					if (exp[i].Column.Type == ColumnType.VARCHAR || exp[i].Column.Type == ColumnType.DATE)
						ret += "\"" + exp[i].Value + "\"";
					else
						ret += exp[i].Value;
				}
				if (i < exp.Count - 1)
					ret += " " + logicalExpression.LogicalType.ToString() + " ";
			}
			if (logicalExpression.LogicExpressions.Count > 0 || logicalExpression.Expressions.Count > 1)
				ret += ")";
			return ret;
		}

		private string CompareEnumToSign(CompareEnum comparison)
		{
			switch (comparison)
			{
				case CompareEnum.Equals: return "=";
				case CompareEnum.NotEquals: return "!=";
				case CompareEnum.GreaterOrEqual: return ">=";
				case CompareEnum.GreaterThen: return ">";
				case CompareEnum.SmallerOrEqual: return "<=";
				case CompareEnum.SmallerThen: return "<";
				case CompareEnum.In: return " IN ";
				case CompareEnum.Not_In: return " NOT IN ";
				default: return "";
			}
		}

		private string ResetUmlaute(string toReset)
		{
			string ret = toReset.Replace('ä', '_');
			ret = ret.Replace('ö', '!');
			ret = ret.Replace('ü', '?');
			ret = ret.Replace('Ä', '&');
			ret = ret.Replace('Ö', '%');
			ret = ret.Replace('Ü', '$');
			ret = ret.Replace('ß', '=');
			return ret;
		}

		private string SetUmlaute(string toSet)
		{
			string ret = toSet.Replace('_', 'ä');
			ret = ret.Replace('!', 'ö');
			ret = ret.Replace('?', 'ü');
			ret = ret.Replace('&', 'Ä');
			ret = ret.Replace('%', 'Ö');
			ret = ret.Replace('$', 'Ü');
			ret = ret.Replace('=', 'ß');
			return ret;
		}
		#endregion
	}
}
