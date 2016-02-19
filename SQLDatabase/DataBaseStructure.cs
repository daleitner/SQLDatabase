using FileIO.XMLReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDatabase
{
	public class DataBaseStructure
	{
		#region members
		#endregion

		#region ctors
		public DataBaseStructure(string dataBaseTableFile)
		{
			Tables = new List<DataBaseTable>();
			List<Node> nodes = XMLReader.ReadXMLFile(dataBaseTableFile);
			List<Node> tables = null;
			foreach (Node n in nodes)
			{
				if(n.Name == "Tables")
					tables = n.Childs;
			}
			if (tables != null)
			{
				foreach(Node tableNode in tables)
				{
					List<DataBaseColumn> columns = new List<DataBaseColumn>();
					List<ForeignKeyColumn> foreignKeys = new List<ForeignKeyColumn>();
					foreach (Node columnNode in tableNode.Childs)
					{
						if (columnNode.Name == "Column")
						{
							string name = columnNode.Attributes["name"];
							ColumnType type = (ColumnType) Enum.Parse(typeof(ColumnType), columnNode.Attributes["type"]);
							//if (type == "VARCHAR")
							//	type += "(32)";
							bool isPrimaryKey = false;
							if (columnNode.Attributes.Keys.Contains("primarykey") && columnNode.Attributes["primarykey"] == "true")
								isPrimaryKey = true;
							if (columnNode.Attributes.Keys.Contains("foreignkey") && columnNode.Attributes.Keys.Contains("reference"))
							{
								string referenceTable = columnNode.Attributes["reference"];
								string referenceColumn = columnNode.Attributes["foreignkey"];
								foreignKeys.Add(new ForeignKeyColumn(name, type, referenceTable, referenceColumn));
							}
							else
							{
								columns.Add(new DataBaseColumn(name, type, isPrimaryKey));
							}
						}
					}
					Tables.Add(new DataBaseTable(tableNode.Name, columns, foreignKeys));
				}
			}
		}
		#endregion

		#region properties
		public List<DataBaseTable> Tables { get; set; }
		#endregion

		#region private methods
		#endregion

		#region public methods
		#endregion
 }
}