using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public class DataBaseTable
	{
		#region members
		#endregion

		#region ctors
		public DataBaseTable()
		{
			this.Name = "";
			this.Columns = new Dictionary<string,DataBaseColumn>();
		}

        public DataBaseTable(string name, List<DataBaseColumn> columns)
        {
            this.Name = name;
			FillDictionary(columns, null);
			SetTableInColumns();
        }

		public DataBaseTable(string name, List<DataBaseColumn> columns, List<ForeignKeyColumn> foreignKeyColumns)
		{
			this.Name = name;
			FillDictionary(columns, foreignKeyColumns);
			SetTableInColumns();
		}

		public DataBaseTable(string name, List<DataBaseColumn> columns, List<ForeignKeyColumn> foreignKeyColumns, string alias)
		{
			this.Name = name;
			this.Alias = alias;
			FillDictionary(columns, foreignKeyColumns);
			SetTableInColumns();
		}
		#endregion

		#region properties
		public string Name { get; set; }
		public Dictionary<string, DataBaseColumn> Columns { get; set; }
		public string Alias { get; set; }
		#endregion

		#region private methods
		private void FillDictionary(List<DataBaseColumn> columns, List<ForeignKeyColumn> foreignKeyColumns)
		{
			this.Columns = new Dictionary<string, DataBaseColumn>();
			if (columns != null)
			{
				foreach (DataBaseColumn c in columns)
				{
					this.Columns.Add(c.Name, c);
				}
			}

			if (foreignKeyColumns != null)
			{
				foreach (ForeignKeyColumn c in foreignKeyColumns)
				{
					this.Columns.Add(c.Name, c);
				}
			}
		}

		private void SetTableInColumns()
		{
			foreach (DataBaseColumn c in this.Columns.Values)
			{
				c.Table = this;
			}
		}
		#endregion

		#region public methods
		public List<DataBaseColumn> GetDataColumns()
		{
			List<DataBaseColumn> ret = new List<DataBaseColumn>();
			foreach (string s in this.Columns.Keys)
			{
				if (this.Columns[s].GetType() != typeof(ForeignKeyColumn))
					ret.Add(this.Columns[s]);
			}
			return ret;
		}

		public List<ForeignKeyColumn> GetForeignKeyColumns()
		{
			List<ForeignKeyColumn> ret = new List<ForeignKeyColumn>();
			foreach (string s in this.Columns.Keys)
			{
				if (this.Columns[s].GetType() == typeof(ForeignKeyColumn))
					ret.Add(this.Columns[s] as ForeignKeyColumn);
			}
			return ret;
		}
		#endregion
 }
}