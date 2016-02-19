using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDatabase;
using SQLDatabase.Interfaces;
using Moq;
using System.Collections.Generic;

namespace SQLDatabaseUnitTests
{
    [TestClass]
    public class DataBaseConnectionTests
    {
        private Mock<IDataBaseRepository> repository = null;

        [TestInitialize]
        public void Setup()
        {
            this.repository = new Mock<IDataBaseRepository>();
        }

        [TestMethod]
        public void OpenNewDataBaseConnection()
        {
            DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
            this.repository.Verify(x => x.CreateDataBaseFile("test.db"));
            this.repository.Verify(x => x.OpenConnection());
        }

        [TestMethod]
        public void CreateATableWithoutForeignKeys()
        {
            DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
            DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
            DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
            DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
            DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 }, null);
            dbcon.CreateTable(table);
            this.repository.Verify(x => x.ExecuteCommand("CREATE TABLE Person (id INTEGER, name VARCHAR(32), geb DATE, PRIMARY KEY(id));"));
        }

        [TestMethod]
        public void CreateATableWithForeignKeys()
        {
            DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
            DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
            ForeignKeyColumn col2 = new ForeignKeyColumn("name", ColumnType.VARCHAR, "xyz", "abc");
            DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
            DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col3 }, new List<ForeignKeyColumn>(){col2});
            dbcon.CreateTable(table);
            this.repository.Verify(x => x.ExecuteCommand("CREATE TABLE Person (id INTEGER, geb DATE, name VARCHAR(32), PRIMARY KEY(id), FOREIGN KEY(name) REFERENCES xyz(abc));"));
        }

		[TestMethod]
		public void CreateATableWithForeignKeysWithoutColumns()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			ForeignKeyColumn col1 = new ForeignKeyColumn("id", ColumnType.INTEGER, "xyz", "abc");
			ForeignKeyColumn col2 = new ForeignKeyColumn("name", ColumnType.VARCHAR, "xyz", "abc");
			DataBaseTable table = new DataBaseTable("Person",new List<DataBaseColumn>(), new List<ForeignKeyColumn>() { col1, col2 });
			dbcon.CreateTable(table);
			this.repository.Verify(x => x.ExecuteCommand("CREATE TABLE Person (id INTEGER, name VARCHAR(32), FOREIGN KEY(id) REFERENCES xyz(abc), FOREIGN KEY(name) REFERENCES xyz(abc));"));
		}

        [TestMethod]
        public void InsertAnElement()
        {
            DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
            DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
            DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
            DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
            DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 }, null);
            Dictionary<DataBaseColumn, object> columnValues = new Dictionary<DataBaseColumn,object>();
            columnValues.Add(col1, 1);
            columnValues.Add(col2, "hallo");
            columnValues.Add(col3, new DateTime(1991, 11, 29));
            ElementInsert insert = new ElementInsert(table, columnValues);
            dbcon.InsertElement(insert);
            this.repository.Verify(x => x.ExecuteCommand("INSERT INTO Person (id, name, geb) VALUES (1, \"hallo\", \"1991-11-29\");"));
        }

		[TestMethod]
		public void SelectStar()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
            DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			DataBaseQuery query = new DataBaseQuery(table);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person;"));
		}

		[TestMethod]
		public void SelectColumns()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			DataBaseQuery query = new DataBaseQuery(new List<DataBaseColumn>(){col1, col2}, table, null);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT Person.id, Person.name FROM Person;"));
		}

		[TestMethod]
		public void SelectDistinctColumns()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			DataBaseQuery query = new DataBaseQuery(new List<DataBaseColumn>() { col1, col2 }, table, null, null, null, true);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT DISTINCT Person.id, Person.name FROM Person;"));
		}

		[TestMethod]
		public void SelectStarWithConditionInteger()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition cond = new Condition().Add(new PropertyExpression(col1, CompareEnum.Equals, "1"));
			DataBaseQuery query = new DataBaseQuery(table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person WHERE Person.id=1;"));
		}

		[TestMethod]
		public void SelectStarWithConditionString()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition cond = new Condition().Add(new PropertyExpression(col2, CompareEnum.Equals, "Daniel"));
			DataBaseQuery query = new DataBaseQuery(table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person WHERE Person.name=\"Daniel\";"));
		}

		[TestMethod]
		public void SelectStarWithConditionDate()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition cond = new Condition().Add(new PropertyExpression(col3, CompareEnum.SmallerOrEqual, new DateTime(2015,1,1)));
			DataBaseQuery query = new DataBaseQuery(table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person WHERE Person.geb<=\"2015-01-01\";"));
		}

		[TestMethod]
		public void SelectStarWithConditionAnd()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition cond = new Condition()
				.Add(new LogicalExpression(LogicalEnum.AND)
					.Add(new PropertyExpression(col1, CompareEnum.Equals, "1"))
					.Add(new PropertyExpression(col2, CompareEnum.Equals, "Daniel")));
			DataBaseQuery query = new DataBaseQuery(table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person WHERE (Person.id=1 AND Person.name=\"Daniel\");"));
		}

		[TestMethod]
		public void SelectColumnsWithConditionOr()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition cond = new Condition()
				.Add(new LogicalExpression(LogicalEnum.OR)
					.Add(new PropertyExpression(col3, CompareEnum.SmallerOrEqual, new DateTime(2015, 1, 1)))
					.Add(new PropertyExpression(col3, CompareEnum.GreaterOrEqual, new DateTime(2015, 2, 1))));
			DataBaseQuery query = new DataBaseQuery(new List<DataBaseColumn>(){col2}, table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT Person.name FROM Person WHERE (Person.geb<=\"2015-01-01\" OR Person.geb>=\"2015-02-01\");"));
		}

		[TestMethod]
		public void SelectStarWithNestedConditions()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition cond = new Condition()
				.Add(new LogicalExpression(LogicalEnum.OR)
					.Add(new LogicalExpression(LogicalEnum.AND)
						.Add(new PropertyExpression(col3, CompareEnum.GreaterOrEqual, new DateTime(2015, 1, 1)))
						.Add(new PropertyExpression(col3, CompareEnum.SmallerOrEqual, new DateTime(2015, 2, 1))))
					.Add(new LogicalExpression(LogicalEnum.AND)
						.Add(new LogicalExpression(LogicalEnum.OR)
							.Add(new PropertyExpression(col2, CompareEnum.Equals, "Daniel"))
							.Add(new PropertyExpression(col1, CompareEnum.Equals, "1")))
						.Add(new PropertyExpression(col3, CompareEnum.Equals, new DateTime(1991, 11, 29)))));
			DataBaseQuery query = new DataBaseQuery(new List<DataBaseColumn>() { col2 }, table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT Person.name FROM Person WHERE ((Person.geb>=\"2015-01-01\" AND Person.geb<=\"2015-02-01\") OR ((Person.name=\"Daniel\" OR Person.id=1) AND Person.geb=\"1991-11-29\"));"));
		}

		[TestMethod]
		public void SelectStarWithSorting()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			DataBaseQuery query = new DataBaseQuery(null, table, null, new Dictionary<DataBaseColumn, SortEnum>(){{col2, SortEnum.ASC}});
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person ORDER BY Person.name;"));
		}

		[TestMethod]
		public void SelectStarWithSortingDesc()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			DataBaseQuery query = new DataBaseQuery(null, table, null, new Dictionary<DataBaseColumn, SortEnum>() { { col2, SortEnum.ASC }, {col1, SortEnum.DESC} });
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person ORDER BY Person.name, Person.id DESC;"));
		}

		[TestMethod]
		public void SelectStarWithSubQuery()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 });
			Condition subcond = new Condition().Add(new PropertyExpression(col1, CompareEnum.Equals, "1"));
			DataBaseQuery subQuery = new DataBaseQuery(new List<DataBaseColumn>() { col2 }, table, subcond);
			Condition cond = new Condition().Add(new PropertyExpression(col2, CompareEnum.In, subQuery));
			DataBaseQuery query = new DataBaseQuery(null, table, cond);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person WHERE Person.name IN (SELECT Person.name FROM Person WHERE Person.id=1);"));
		}

		[TestMethod]
		public void SelectStarWithInnerJoin()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col4 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col5 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseTable table2 = new DataBaseTable("Car", new List<DataBaseColumn>() { col4, col5 });
			ForeignKeyColumn col3 = new ForeignKeyColumn("cid", ColumnType.INTEGER, table2.Name, col4.Name);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2 }, new List<ForeignKeyColumn>() { col3 });
			JoinTable jtable = new JoinTable(table2, col3, col4);
			DataBaseQuery query = new DataBaseQuery(null, table, new Join(JoinEnum.Inner, jtable), null);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person INNER JOIN Car ON Person.cid=Car.id;"));
		}

		[TestMethod]
		public void SelectStarWithInnerJoins()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col4 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col5 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseTable table2 = new DataBaseTable("Car", new List<DataBaseColumn>() { col4, col5 });
			ForeignKeyColumn col3 = new ForeignKeyColumn("cid", ColumnType.INTEGER, table2.Name, col4.Name);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2 }, new List<ForeignKeyColumn>() { col3 });
			JoinTable jtable = new JoinTable(table2, col3, col4);
			JoinTable jtable2 = new JoinTable(table2, col2, col5);
			DataBaseQuery query = new DataBaseQuery(null, table,new List<Join>(){ new Join(JoinEnum.Inner, jtable), new Join(JoinEnum.Inner, jtable2)}, null);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person INNER JOIN Car ON Person.cid=Car.id INNER JOIN Car ON Person.name=Car.name;"));
		}

		[TestMethod]
		public void SelectStarWithInnerJoins2()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("pid", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("cid", ColumnType.INTEGER, true);
			DataBaseColumn col4 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col5 = new DataBaseColumn("rid", ColumnType.INTEGER, true);
			DataBaseColumn col6 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseTable table2 = new DataBaseTable("Car", new List<DataBaseColumn>() { col3, col4 });
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2 });
			DataBaseTable table3 = new DataBaseTable("Reifen", new List<DataBaseColumn>() { col5, col6 });
			JoinTable jtable = new JoinTable(table2, col1, col3);
			JoinTable jtable2 = new JoinTable(table3, col3, col5);
			DataBaseQuery query = new DataBaseQuery(null, table, new List<Join>() { new Join(JoinEnum.Inner, jtable), new Join(JoinEnum.Inner, jtable2) }, null);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person INNER JOIN Car ON Person.pid=Car.cid INNER JOIN Reifen ON Car.cid=Reifen.rid;"));
		}

		[TestMethod]
		public void SelectStarWithInnerJoinAndCondition()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col4 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col5 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseTable table2 = new DataBaseTable("Car", new List<DataBaseColumn>() { col4, col5 });
			ForeignKeyColumn col3 = new ForeignKeyColumn("cid", ColumnType.INTEGER, table2.Name, col4.Name);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2 }, new List<ForeignKeyColumn>() { col3 });
			JoinTable jtable = new JoinTable(table2, col3, col4);
			Condition con = new Condition()
				.Add(new LogicalExpression(LogicalEnum.AND)
					.Add(new PropertyExpression(col2, CompareEnum.Equals, "Hallo"))
					.Add(new PropertyExpression(col5, CompareEnum.Equals, "Audi")));
			DataBaseQuery query = new DataBaseQuery(null, table, new Join(JoinEnum.Inner, jtable), con);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person INNER JOIN Car ON Person.cid=Car.id WHERE (Person.name=\"Hallo\" AND Car.name=\"Audi\");"));
		}

		[TestMethod]
		public void DeleteAnElement()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 }, null);
			Condition condition = new Condition()
				.Add(new PropertyExpression(col1, CompareEnum.Equals, "1"));
			ElementDelete delete = new ElementDelete(table, condition);
			dbcon.DeleteElement(delete);
			this.repository.Verify(x => x.ExecuteCommand("DELETE FROM Person WHERE id=1;"));
		}

		[TestMethod]
		public void DeleteAllElements()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 }, null);
			ElementDelete delete = new ElementDelete(table);
			dbcon.DeleteElement(delete);
			this.repository.Verify(x => x.ExecuteCommand("DELETE FROM Person;"));
		}

		[TestMethod]
		public void UpdateAnElement()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 }, null);
			Condition condition = new Condition()
				.Add(new PropertyExpression(col1, CompareEnum.Equals, "1"));
			ElementUpdate update = new ElementUpdate(table, new Dictionary<DataBaseColumn, object>() { {col2, "Hallo"} }, condition);
			dbcon.UpdateElement(update);
			this.repository.Verify(x => x.ExecuteCommand("UPDATE Person SET name=\"Hallo\" WHERE id=1;"));
		}

		[TestMethod]
		public void UpdateAllElements()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col3 = new DataBaseColumn("geb", ColumnType.DATE);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2, col3 }, null);
			Condition condition = new Condition()
				.Add(new PropertyExpression(col1, CompareEnum.Equals, "1"));
			ElementUpdate update = new ElementUpdate(table, new Dictionary<DataBaseColumn, object>() { { col2, "Hallo" } }, condition);
			dbcon.UpdateElement(update);
			this.repository.Verify(x => x.ExecuteCommand("UPDATE Person SET name=\"Hallo\" WHERE id=1;"));
		}

		[TestMethod]
		public void SelectStarWithInnerJoinAndConditionAndAlias()
		{
			DataBaseConnection dbcon = new DataBaseConnection(this.repository.Object, "test");
			DataBaseColumn col1 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col2 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseColumn col4 = new DataBaseColumn("id", ColumnType.INTEGER, true);
			DataBaseColumn col5 = new DataBaseColumn("name", ColumnType.VARCHAR);
			DataBaseTable table2 = new DataBaseTable("Car", new List<DataBaseColumn>() { col4, col5 }, null, "C");
			ForeignKeyColumn col3 = new ForeignKeyColumn("cid", ColumnType.INTEGER, table2.Name, col4.Name);
			DataBaseTable table = new DataBaseTable("Person", new List<DataBaseColumn>() { col1, col2 }, new List<ForeignKeyColumn>() { col3 }, "P");
			JoinTable jtable = new JoinTable(table2, col3, col4);
			Condition con = new Condition()
				.Add(new LogicalExpression(LogicalEnum.AND)
					.Add(new PropertyExpression(col2, CompareEnum.Equals, "Hallo"))
					.Add(new PropertyExpression(col5, CompareEnum.Equals, "Audi")));
			DataBaseQuery query = new DataBaseQuery(null, table, new Join(JoinEnum.Inner, jtable), con);
			dbcon.ExecuteQuery(query);
			this.repository.Verify(x => x.ExecuteQuery("SELECT * FROM Person P INNER JOIN Car C ON P.cid=C.id WHERE (P.name=\"Hallo\" AND C.name=\"Audi\");"));
		}
    }
}
