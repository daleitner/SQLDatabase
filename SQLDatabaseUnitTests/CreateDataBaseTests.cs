using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDatabase;
using Moq;
using SQLDatabase.Interfaces;
using Finisar.SQLite;

namespace SQLDatabaseUnitTests
{
	[TestClass]
	public class CreateDataBaseTests
	{
        private Mock<IDataBaseRepository> repository = null;

		[TestInitialize]
		public void Setup()
		{
            repository = new Mock<IDataBaseRepository>();
		}

		[TestMethod]
		public void CreateDataBase()
		{
			DataBase db = DataBase.GetInstance(repository.Object);
			bool success = db.CreateDataBase("test1");
            repository.Verify(x => x.CreateDataBaseFile("test1.db"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void TryCreateExistingDataBase()
		{
            repository.Setup(x => x.DataBaseFileExists(It.IsAny<string>())).Returns(true);
			DataBase db = DataBase.GetInstance(repository.Object);
			bool success = db.CreateDataBase("test2");
			Assert.IsFalse(success);
		}

		[TestMethod]
		public void TryCreateLoadedDataBase()
		{
			DataBase db = DataBase.GetInstance(repository.Object);
			db.CreateDataBase("test3");
			bool success = db.CreateDataBase("test3");
            repository.Verify(x => x.CreateDataBaseFile("test3.db"), Times.Once());
			Assert.IsFalse(success);
		}

		[TestMethod]
		public void ChangeDataBase()
		{
			DataBase db = DataBase.GetInstance(repository.Object);
			db.CreateDataBase("test4.1");
			db.CreateDataBase("test4");
            repository.Verify(x => x.CreateDataBaseFile(It.IsAny<string>()), Times.Exactly(2));
			bool success = db.ChangeDataBase("test4.1");
            repository.Verify(x => x.ChangeDataBase("test4.1.db"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void ChangeToFileExistingDataBase()
		{
            repository.Setup(x => x.DataBaseFileExists(It.IsAny<string>())).Returns(true);
			DataBase db = DataBase.GetInstance(repository.Object);
			db.CreateDataBase("test5");
			bool success = db.ChangeDataBase("test5.1");
            repository.Verify(x => x.ChangeDataBase("test5.1.db"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void TryChangeToUsedDataBase()
		{
			DataBase db = DataBase.GetInstance(repository.Object);
			db.CreateDataBase("test6");
			bool success = db.ChangeDataBase("test6");
            repository.Verify(x => x.ChangeDataBase(It.IsAny<string>()), Times.Never());
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void LoadExistingDataBase()
		{
            repository.Setup(x => x.DataBaseFileExists("test7.db")).Returns(false);
            repository.Setup(x => x.DataBaseFileExists("test7.1.db")).Returns(true);
			DataBase db = DataBase.GetInstance(repository.Object);
			db.CreateDataBase("test7");
			bool success = db.LoadDataBase("test7.1");
            repository.Verify(x => x.LoadDataBaseFile("test7.1.db"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void LoadLoadedDataBase()
		{
			DataBase db = DataBase.GetInstance(repository.Object);
			db.CreateDataBase("test8");
			db.CreateDataBase("test8.1");
			bool success = db.LoadDataBase("test8");
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void TryLoadNotExistingDataBase()
		{
            repository.Setup(x => x.DataBaseFileExists("test9.db")).Returns(false);
			DataBase db = DataBase.GetInstance(repository.Object);
			bool success = db.LoadDataBase("test9");
			Assert.IsFalse(success);
		}

		[TestMethod]
		public void CreateDataBaseIfNotExists()
		{
			DataBase db = DataBase.GetInstance(repository.Object);
			bool success = db.CreateDataBaseIfNotExists("test10");
            repository.Verify(x => x.CreateDataBaseFile("test10.db"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void LoadDataBaseIfExists()
		{
            repository.Setup(x => x.DataBaseFileExists(It.IsAny<string>())).Returns(true);
			DataBase db = DataBase.GetInstance(repository.Object);
			bool success = db.CreateDataBaseIfNotExists("test11");
            repository.Verify(x => x.LoadDataBaseFile("test11.db"));
			Assert.IsTrue(success);
		}
	}
}
