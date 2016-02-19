using Finisar.SQLite;
using SQLDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public class DataBaseRepository : IDataBaseRepository
    {
        #region members
        private SQLiteConnection sqlConnection;
        private SQLiteCommand sqlCommand;
        #endregion

        #region ctors
        public DataBaseRepository()
		{
		}
        #endregion

        #region public methods
        public void CreateDataBaseFile(string dataBaseFileName)
		{
			this.sqlConnection = new SQLiteConnection("Data Source=" + dataBaseFileName + ";New=True");
			this.sqlConnection.Open();
		}

		public void ChangeDataBase(string dataBaseFileName)
		{
			this.sqlConnection.ChangeDatabase(dataBaseFileName);
			this.sqlConnection.Open();
		}

		public void LoadDataBaseFile(string dataBaseFileName)
		{
			this.sqlConnection = new SQLiteConnection("Data Source=" + dataBaseFileName + ";");
			this.sqlConnection.Open();
		}

        public bool DataBaseFileExists(string dataBaseFileName)
        {
            return File.Exists(dataBaseFileName);
        }

        public void OpenConnection()
        {
            this.sqlConnection.Open();
        }

        public void CloseConnection()
        {
            this.sqlConnection.Close();
        }

        public void ExecuteCommand(string command)
        {
            string help = command;
            this.sqlCommand = this.sqlConnection.CreateCommand();
            this.sqlCommand.CommandText = help;
            this.sqlCommand.ExecuteNonQuery();
        }

		public List<List<string>> ExecuteQuery(string query)
		{
			List<List<string>> ret = new List<List<string>>();
			SQLiteDataAdapter DB;
			DataSet DS = new DataSet();
				string clearquery = query;
				DB = new SQLiteDataAdapter(clearquery, this.sqlConnection);
				DS.Reset();
				DB.Fill(DS);
			if (DS.Tables.Count > 0)
			{
				if (DS.Tables[0].Rows.Count > 0)
				{
					for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
					{
						List<string> help = new List<string>();
						string logstring = "";
						for (int j = 0; j < DS.Tables[0].Rows[i].ItemArray.Length; j++)
						{
							string h = DS.Tables[0].Rows[i].ItemArray[j].ToString();
							help.Add(h);
							logstring += h + " ";
						}
						ret.Add(help);
					}
				}
			}
			return ret;
		}
        #endregion

        #region private methods
        #endregion
	}
}
