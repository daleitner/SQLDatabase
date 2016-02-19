using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase.Interfaces
{
	public interface IDataBaseRepository
	{
		void CreateDataBaseFile(string dataBaseFileName);
		void ChangeDataBase(string dataBaseFileName);
		void LoadDataBaseFile(string dataBaseFileName);
        bool DataBaseFileExists(string dataBaseFileName);
        void OpenConnection();
        void CloseConnection();
        void ExecuteCommand(string command);
		List<List<string>> ExecuteQuery(string query);
	}
}
