using Base;
using FileIO.FileWorker;
using FileIO.XMLReader;
using Finisar.SQLite;
using SQLDatabase.Exceptions;
using SQLDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public class DataBase
    {
        #region members
        private static DataBase dataBase;
		private FileWorker fileWorker;
		private IDataBaseRepository repository;
		private List<string> dataBases;
		private string selectedDataBase;
		private static bool isTest = true;
        #endregion

        #region ctors
        private DataBase(IDataBaseRepository dataBaseRepository)
		{
			this.repository = dataBaseRepository;
			this.fileWorker = FileWorker.GetInstance("LOG_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt", false);
			this.dataBases = new List<string>();
		}
        #endregion

        #region static methods
        public static DataBase GetInstance()
		{
			isTest = false;
            if (dataBase == null)
                dataBase = new DataBase(new DataBaseRepository());
            return dataBase;
		}

		public static DataBase GetInstance(IDataBaseRepository dataBaseRepository)
		{
			if (dataBase == null)
                dataBase = new DataBase(dataBaseRepository);
            dataBase.repository = dataBaseRepository; //für testen; sonst werden immer die gleichen setups vom mock verwendet
			return dataBase;
		}
        #endregion

        #region public methods
        public bool CreateDataBase(string dataBaseName)
		{
			LogInfo("CreateDataBase", "dataBaseName=<" + dataBaseName + ">");
            if (!this.dataBases.Contains(dataBaseName))
            {
                if (!this.repository.DataBaseFileExists(dataBaseName + ".db"))
                {
                    try
                    {
                        this.repository.CreateDataBaseFile(dataBaseName + ".db");
                        this.dataBases.Add(dataBaseName);
                        this.selectedDataBase = dataBaseName;
                    }
                    catch (SQLiteException sqlex)
                    {
						LogInfo("CreateDataBase", sqlex.Message + "\r\n" + sqlex.StackTrace);
                        throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
                    }
                    return true;
                }
            }
			return false;
		}

		public bool ChangeDataBase(string dataBaseName)
		{
			LogInfo("ChangeDataBase", "dataBaseName=<" + dataBaseName + ">");
			if (this.selectedDataBase == dataBaseName)
				return true;

			if (this.dataBases.Contains(dataBaseName) || this.repository.DataBaseFileExists(dataBaseName + ".db"))
			{
				try
				{
					this.repository.ChangeDataBase(dataBaseName + ".db");
				}
				catch (SQLiteException sqlex)
				{
					LogInfo("ChangeDataBase", sqlex.Message + "\r\n" + sqlex.StackTrace);
					throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
				}

				if(!this.dataBases.Contains(dataBaseName))
					this.dataBases.Add(dataBaseName);

				this.selectedDataBase = dataBaseName;
				return true;
			}
			return false;
		}

        public bool LoadDataBase(string dataBaseName)
        {
			LogInfo("LoadDataBase", "dataBaseName=<" + dataBaseName + ">");
            if (this.selectedDataBase == dataBaseName)
                return true;

            if (this.dataBases.Contains(dataBaseName))
            {
                this.selectedDataBase = dataBaseName;
                return true;
            }

			if (this.repository.DataBaseFileExists(dataBaseName + ".db"))
			{
				try
				{
					this.repository.LoadDataBaseFile(dataBaseName + ".db");
					this.dataBases.Add(dataBaseName);
					this.selectedDataBase = dataBaseName;
				}
				catch (SQLiteException sqlex)
				{
					LogInfo("LoadDataBase", sqlex.Message + "\r\n" + sqlex.StackTrace);
					throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
				}
				return true;
			}
			return false;
        }

		public bool CreateDataBaseIfNotExists(string dataBaseName)
		{
			LogInfo("CreateDataBaseIfNotExists", "dataBaseName=<" + dataBaseName + ">");
			if (!LoadDataBase(dataBaseName))
			{
				return CreateDataBase(dataBaseName);
			}
			return true;
		}

        public void OpenConnection()
        {
			LogInfo("OpenConnection", "");
            try
            {
                this.repository.OpenConnection();
            }
            catch (SQLiteException sqlex)
            {
				LogInfo("OpenConnection", sqlex.Message + "\r\n" + sqlex.StackTrace);
                throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
            }
        }

        public void CloseConnection()
        {
			LogInfo("CloseConnection", "");
            try
            {
                this.repository.CloseConnection();
            }
            catch (SQLiteException sqlex)
            {
				LogInfo("CloseConnection", sqlex.Message + "\r\n" + sqlex.StackTrace);
                throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
            }
        }

        public void ExecuteCommand(string command)
        {
			LogInfo("ExecuteCommand", "command=<" + command + ">");
            try
            {
                this.repository.ExecuteCommand(command);
            }
            catch (SQLiteException sqlex)
            {
				LogInfo("ExecuteCommand", sqlex.Message + "\r\n" + sqlex.StackTrace);
                throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
            }
        }

		public List<List<string>> ExecuteQuery(string query)
		{
			LogInfo("ExecuteQuery", "query=<" + query + ">");
			List<List<string>> ret = null;
			try
			{
				ret = this.repository.ExecuteQuery(query);
			}
			catch (SQLiteException sqlex)
			{
				LogInfo("ExecuteQuery", sqlex.Message + "\r\n" + sqlex.StackTrace);
				throw new Exception(sqlex.Message, sqlex) as SQLDatabaseException;
			}
			return ret;
		}
        #endregion

        #region private methods
		private void LogInfo(String Function, String Message)
		{
			if(!isTest)
				this.fileWorker.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\t" + Function + "\t" + Message);
		}
        #endregion

        /*
		#region members
		private static Dictionary<string, DataBase> dataBases = null;
		private FileWorker writer = null;
		private DataBaseConfig config = null;
		private SQLiteConnection sqlConnection;
		private SQLiteCommand sqlCommand;
		#endregion

		#region ctor
		private DataBase(string configFile, string tableFile)
		{
			this.config = new DataBaseConfig(configFile, tableFile);
			bool newdb = false;
			this.writer = FileWorker.GetInstance(this.config.LogFile, false);
			try
			{
				this.sqlConnection = new SQLiteConnection("Data Source=" + this.config.DataBaseFile + ";");
                this.sqlConnection.Open();
				LogInfo("DataBase", "Opened DataBase " + this.config.DataBaseFile);
			}
			catch (SQLiteException sqle)
			{
				string error = sqle.Message;
				LogInfo("DataBase", error);
				if (error.Contains(this.config.DataBaseFile + "' does not exist. Use ConnectionString parameter New=True to create new file."))
				{
					LogInfo("DataBase", "Creating new Database: " + this.config.DataBaseFile);
                    this.sqlConnection = new SQLiteConnection("Data Source=" + this.config.DataBaseFile + ";New=True");
                    this.sqlConnection.Open();
					LogInfo("DataBase", "Opened DataBase " + this.config.DataBaseFile);
					newdb = true;
				}
			}
			finally
			{
				if (newdb)
				{
					CreateTables();
				}
				CloseConnection();
			}
		}
		#endregion

		#region static methods
		public static DataBase GetInstance()
		{
			if (dataBases != null)
			{
				return dataBases.First().Value;
			}
			throw new Exception("Database Dicitonary was not initialized!");
		}

		public static DataBase GetInstance(string key)
		{
			if (dataBases != null)
			{
				return dataBases[key];
			}
			throw new Exception("Database Dicitonary was not initialized!");
		}

		public static void Initialize(string configFile, string tableFile)
		{
			if (dataBases == null)
				dataBases = new Dictionary<string, DataBase>();
			if (!dataBases.Keys.Contains(configFile))
				dataBases.Add(configFile, new DataBase(configFile, tableFile));
		}
		#endregion

		#region sqlite
		private void OpenConnection()
		{
			try
			{
				LogInfo("OpenConnection", "Database:" + this.config.DataBaseFile);
                this.sqlConnection = new SQLiteConnection("Data Source=" + this.config.DataBaseFile + ";");
                this.sqlConnection.Open();
				LogInfo("OpenConnection", "Open " + this.config.DataBaseFile + " successful");
			}
			catch (SQLiteException sqle)
			{
				string error = sqle.Message;
				LogInfo("OpenConnection", error);
				if (error.Contains(this.config.DataBaseFile + "' does not exist. Use ConnectionString parameter New=True to create new file."))
				{
					System.Console.WriteLine("Creating new Database: " + this.config.DataBaseFile);
                    this.sqlConnection = new SQLiteConnection("Data Source=" + this.config.DataBaseFile + ";New=True");
                    this.sqlConnection.Open();
				}
			}
		}

		private void CloseConnection()
		{
			LogInfo("CloseConnection", "");
            this.sqlConnection.Close();
		}

		private void ExecuteQuery(string txtQuery, bool isConnected)
		{
			try
			{
				if(!isConnected)
					OpenConnection();
				string help = ResetUmlaute(txtQuery);
				LogInfo("ExecuteQuery", "Execute " + help);
                this.sqlCommand = this.sqlConnection.CreateCommand();
				this.sqlCommand.CommandText = help;
				this.sqlCommand.ExecuteNonQuery();
				
				if(!isConnected)
					CloseConnection();
			}
			catch (SQLiteException sqle)
			{
				LogInfo("ExecuteQuery", sqle.Message);
			}
			catch (Exception e)
			{
				LogInfo("ExecuteQuery", e.Message);
				LogInfo("ExecuteQuery", e.StackTrace);
			}
		}

		public List<List<string>> GetExecutedQuery(string txtQuery, bool isConnected)
		{
			List<List<string>> ret = new List<List<string>>();
			SQLiteDataAdapter DB;
			DataSet DS = new DataSet();
			try
			{
				if(!isConnected)
					OpenConnection();
				string help = ResetUmlaute(txtQuery);
				LogInfo("GetExecutedQuery", "Execute <" + help + ">");
				DB = new SQLiteDataAdapter(help, this.sqlConnection);
				DS.Reset();
				DB.Fill(DS);
				if(!isConnected)
					CloseConnection();
			}
			catch (SQLiteException sqle)
			{
				LogInfo("GetExecutedQuery", sqle.Message);
				return new List<List<string>>() { new List<string>() { "Error", sqle.Message } };
			}
			catch (Exception e)
			{
				LogInfo("GetExecutedQuery", e.Message);
				LogInfo("GetExecutedQuery", e.StackTrace);
				return new List<List<string>>() { new List<string>() { "Error", e.Message } };
			}
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
							string h = SetUmlaute(DS.Tables[0].Rows[i].ItemArray[j].ToString());
							help.Add(h);
							logstring += h + " ";
						}
						ret.Add(help);
						LogInfo("GetExecutedQuery", "Result " + i + ": " + logstring);
					}
				}
				else
				{
					LogInfo("GetExecutedQuery", "Result: No Entries found!");
				}
			}
			return ret;
		}
		#endregion

		#region select
		public ObservableCollection<T> Select<T>()
		{
			Type t = typeof(T);
			LogInfo("Select", "type=<" + t.Name + ">");
			ObservableCollection<T> ret = new ObservableCollection<T>();
			string query = "SELECT * FROM " + this.config.Mapping[t.Name] + ";";
			List<List<string>> dt = GetExecutedQuery(query, false);
			foreach (List<string> l in dt)
			{
				object instance = Activator.CreateInstance(t, new object[] {l});
				ret.Add((T)instance);
			}
			return ret;
		}

		public ObservableCollection<T> Select<T>(Query query)
		{
			Type t = typeof(T);
			LogInfo("Select", "type=<" + t.Name + ">, query=<" + query.ToString() + ">");
			ObservableCollection<T> ret = new ObservableCollection<T>();
			string stmt = "SELECT * FROM " + this.config.Mapping[t.Name];
			stmt += " WHERE " + query.ToString() + ";";
			List<List<string>> dt = GetExecutedQuery(stmt, false);
			foreach (List<string> l in dt)
			{
				object instance = Activator.CreateInstance(t, new object[] { l });
				ret.Add((T)instance);
			}
			return ret;
		}

		public void Delete(ModelBase p)
		{
			LogInfo("DeletePlayer", "p=<" + p.ToString() + ">");
			Type t = p.GetType();
			string deleteStmt = "DELETE FROM " + this.config.Mapping[t.Name] + " WHERE Id=\"" + p.GetId() + "\";";
			ExecuteQuery(deleteStmt, false);
		}

        //public ObservableCollection<Player> GetAvailablePlayersForTeam(Team team)
        //{
        //    LogInfo("GetAvailablePlayersForTeam", "Team=<" + team.ToString() + ">");
        //    ObservableCollection<Player> ret = new ObservableCollection<Player>();
        //    string query = "SELECT * FROM Players WHERE PID NOT IN(SELECT DISTINCT PID FROM PlaysIn);";
        //    List<List<string>> dt = GetExecutedQuery(query, false);
        //    foreach (List<string> l in dt)
        //    {
        //        ret.Add(new Player(l));
        //    }
        //    return ret;
        //}

        //public ObservableCollection<Team> GetTeams(Player p)
        //{
        //    LogInfo("GetTeams", "Player=<" + p.ToString() + ">");
        //    ObservableCollection<Team> ret = new ObservableCollection<Team>();
        //    string querystring = "SELECT * FROM Teams t INNER JOIN PlaysIn p ON t.TID=p.TID WHERE p.PID=\"" + p.GetId() + "\";";
        //    List<List<string>> dt = GetExecutedQuery(querystring, false);
        //    foreach (List<string> l in dt)
        //    {
        //        ret.Add(new Team(l));
        //    }
        //    return ret;
        //}

        //public Verein GetVerein(Team t)
        //{
        //    LogInfo("GetVerein", "Team=<" + t.ToString() + ">");
        //    Verein ret = null;
        //    string querystring = "SELECT * FROM Vereine WHERE VID IN (SELECT VID FROM Teams WHERE TID=\"" + t.GetId() + "\");";
        //    List<List<string>> dt = GetExecutedQuery(querystring, false);
        //    if (dt.Count > 0)
        //        ret = new Verein(dt[0]);
        //    return ret;
        //}


        //public ObservableCollection<Player> GetPlayers(Team t)
        //{
        //    LogInfo("GetPlayers", "Team=<" + t.ToString() + ">");
        //    ObservableCollection<Player> ret = new ObservableCollection<Player>();
        //    string query = "SELECT * FROM Players p INNER JOIN PlaysIn i ON p.PID=i.PID WHERE i.TID=\"" + t.GetId() + "\";";
        //    List<List<string>> dt = GetExecutedQuery(query, false);
        //    foreach (List<string> l in dt)
        //    {
        //        ret.Add(new Player(l));
        //    }
        //    return ret;
        //}

        //public Lokal GetLokal(Verein v)
        //{
        //    LogInfo("GetLokal", "Verein=<" + v.ToString() + ">");
        //    Lokal ret = null;
        //    string querystring = "SELECT * FROM Lokale WHERE LID IN(SELECT LID FROM Vereine WHERE VID=\"" + v.GetId() + "\");";
        //    List<List<string>> dt = GetExecutedQuery(querystring, false);
        //    if (dt.Count > 0)
        //        ret = new Lokal(dt[0]);
        //    return ret;
        //}

        //public Lokal GetLokal(Team t)
        //{
        //    LogInfo("GetLokal", "Team=<" + t.ToString() + ">");
        //    Lokal ret = null;
        //    string querystring = "SELECT * FROM Lokale WHERE LID IN(SELECT LID FROM Teams WHERE TID=\"" + t.GetId() + "\");";
        //    List<List<string>> dt = GetExecutedQuery(querystring, false);
        //    if (dt.Count > 0)
        //        ret = new Lokal(dt[0]);
        //    return ret;
        //}

        //public ObservableCollection<Team> GetTeams(Division d)
        //{
        //    LogInfo("GetTeams", "division=<" + d.ToString() + ">");
        //    ObservableCollection<Team> ret = new ObservableCollection<Team>();
        //    string query = "SELECT t.tid, t.name, t.imagename FROM Teams t INNER JOIN RegisteredIn r ON r.tid = t.tid INNER JOIN Divisionen d ON d.did = r.did WHERE r.did=\"" + d.GetId() + "\";";
        //    List<List<string>> dt = GetExecutedQuery(query, false);
        //    foreach (List<string> l in dt)
        //    {
        //        Team t = new Team(l);
        //        ret.Add(t);
        //    }
        //    return ret;
        //}

        //private ObservableCollection<ScoreSheet> GetScoreSheets(Division d, bool isConnectionOpen)
        //{
        //    LogInfo("GetScoreSheets", "Division=<" + d.ToString() + ">");
        //    ObservableCollection<ScoreSheet> ret = new ObservableCollection<ScoreSheet>();
        //    string query = "SELECT s.scid, s.round, s.battleindex, s.datum, s.theim, s.tgast, s.hset, s.gset, s.hleg, s.gleg, s.did, ";
        //    query += "t.tid, t.name, t.imagename, t.vid, t.lid, g.tid, g.name, g.imagename, g.vid, g.lid ";
        //    query += "FROM ScoreSheets s INNER JOIN Teams t ON s.theim = t.tid INNER JOIN Teams g ON s.tgast = g.tid WHERE s.DID=\"" + d.GetId() + "\";";
        //    List<List<string>> dt = GetExecutedQuery(query, isConnectionOpen);
        //    foreach (List<string> l in dt)
        //    {
        //        ScoreSheet s = new ScoreSheet(l);
        //        s.HeimMannschaft = new Team(l.GetRange(11, 5));
        //        s.GastMannschaft = new Team(l.GetRange(16, 5));
        //        s.Division = d;
        //        ret.Add(s);
        //    }
        //    return ret;
        //}


        //#region insert
        //public void InsertPlayer(Player p)
        //{
        //    LogInfo("InsertPlayer", "Player=<" + p.ToString() + ">");
        //    string insertstring = "INSERT INTO Players VALUES(\"" + p.GetId() + "\", \"" + p.VorName + "\", \"" + p.NachName + "\", \"" + p.Geb.ToString("yyyy-MM-dd") + "\", " + p.Scores.ToString() + ", \"" + p.ImageName + "\");";
        //    OpenConnection();
        //    ExecuteQuery(insertstring, true);
        //    CloseConnection();
        //}

        //public void InsertTeam(Team team)
        //{
        //    LogInfo("InsertTeam", "Team=<" + team.ToString() + ">");
        //    string insertstring = "INSERT INTO Teams VALUES(\"" + team.GetId() + "\", \"" + team.Name + "\", \"" + team.ImageName + "\", \"" + (team.Verein != null ? team.Verein.GetId() : "") + "\");";
        //    OpenConnection();
        //    ExecuteQuery(insertstring, true);
        //    InsertPlaysIn(team);
        //    CloseConnection();
        //}

        //public void InsertLokal(Lokal l)
        //{
        //    LogInfo("InsertLokal", "Lokal=<" + l.ToString() + ">");
        //    string insertstring = "INSERT INTO Lokale VALUES(\"" + l.GetId() + "\", \"" + l.Name + "\", \"" + l.Adresse + "\", \"" + l.Telefon + "\", \"" + l.HomePage + "\", \"" + l.ImageName + "\");";
        //    OpenConnection();
        //    ExecuteQuery(insertstring, true);
        //    CloseConnection();
        //}

        //public void InsertVerein(Verein v)
        //{
        //    LogInfo("InsertVerein", "Verein=<" + v.ToString() + ">");
        //    int lid = 0;
        //    string insertstring = "INSERT INTO Vereine VALUES(\"" + v.GetId() + "\", \"" + v.Name + "\", \"" + v.ImageName + "\", \"" + lid.ToString() + "\");";
        //    OpenConnection();
        //    ExecuteQuery(insertstring, true);
        //    CloseConnection();
        //}

        //public void InsertSaison(Saison s)
        //{
        //    LogInfo("InsertSaison", "Saison=<" + s.ToString() + ">");
        //    string insertstring = "INSERT INTO Saisonen VALUES(\"" + s.GetId() + "\", \"" + s.Start.ToString("yyyy-MM-dd") + "\", \"" + s.End.ToString("yyyy-MM-dd") + "\", \"" + s.ImageName + "\", \"" + s.Liga.GetId() + "\");";
        //    OpenConnection();
        //    ExecuteQuery(insertstring, true);
        //    foreach (Division d in s.Divisionen)
        //    {
        //        InsertDivision(d);
        //    }
        //    CloseConnection();
        //}

        //private void InsertDivision(Division d)
        //{
        //    LogInfo("InsertDivision", "d=<" + d.ToString() + ">");
        //    string insertstring = "INSERT INTO Divisionen VALUES(\"" + d.GetId() + "\", \"" + d.Name + "\", \"" + d.Saison.GetId() + "\");";
        //    ExecuteQuery(insertstring, true);
        //    InsertRegisteredIn(d, d.Mannschaften);
        //    foreach (ScoreSheet s in d.ScoreSheets)
        //    {
        //        InsertScoreSheet(s);
        //    }
        //}

        //private void InsertScoreSheet(ScoreSheet s)
        //{
        //    LogInfo("InsertScoreSheet", "ScoreSheet=<" + s.ToString() + ">");
        //    string insertstring = "INSERT INTO ScoreSheets VALUES(\"" + s.GetId() + "\", \"" + s.Round + "\", \"" + s.Index + "\", \"" + s.Datum.ToString("yyyy-MM-dd") + "\", \"" + s.HeimMannschaft.GetId() + "\", \"" + s.GastMannschaft.GetId() + "\", \"" + s.EndErgebnis.LegsHeim + "\", \"" + s.EndErgebnis.LegsGast + "\", \"" + s.EndErgebnis.SetsHeim + "\", \"" + s.EndErgebnis.SetsGast + "\", \"" + s.Division.GetId() + "\");";
        //    ExecuteQuery(insertstring, true);
        //    foreach(PlayerEntry player in s.HeimPlayerEntries)
        //    {
        //        InsertPlayerEntry(player, s, true);
        //    }

        //    foreach(PlayerEntry player in s.GastPlayerEntries)
        //    {
        //        InsertPlayerEntry(player, s, false);
        //    }

        //    foreach(BattleModel b in s.Spiele)
        //    {
        //        InsertSpiel(b, s);
        //    }
        //}

        //private void InsertPlayerEntry(PlayerEntry entry, ScoreSheet sc, bool isHeim)
        //{
        //    LogInfo("InsertPlayerEntry", "entry=<" + entry.ToString() + ">, scoreSheetID=<" + sc.GetId() + ">, isHeim=<" + isHeim + ">");
        //    string insertstring = "INSERT INTO PlayerEntries VALUES(\"" + entry.GetId() + "\", \"" + sc.GetId() + "\", \"" + entry.Key + "\", \"" + (entry.Spieler != null ? entry.Spieler.GetId() : "") + "\", " + entry.Scores + ", \"" + (isHeim ? 1 : 0) + "\");";
        //    ExecuteQuery(insertstring, true);
        //}

        //private void InsertPlaysIn(Team team)
        //{
        //    LogInfo("InsertPlaysIn", team.ToString());
        //    if (team.Players != null)
        //    {
        //        foreach (Player p in team.Players)
        //        {
        //            string insertstring = "INSERT INTO PlaysIn VALUES(\"" + p.GetId() + "\", \"" + team.GetId() + "\");";
        //            ExecuteQuery(insertstring, true);
        //        }
        //    }
        //    else
        //    {
        //        LogInfo("InsertPlaysIn", "Players are null!");
        //    }
        //}

        //private void InsertRegisteredIn(Division d, ObservableCollection<Team> teams)
        //{
        //    if (teams != null)
        //    {
        //        List<string> help = new List<string>();
        //        foreach (Team t in teams)
        //        {
        //            help.Add(t.ToString());
        //        }
        //        LogInfo("InsertRegisteredIn", "Division=<" + d.ToString() + ">, Teams=<" + string.Join(", ", help.ToArray()) + ">");

        //        foreach (Team t in teams)
        //        {
        //            string insertstring = "INSERT INTO RegisteredIn VALUES(\"" + d.GetId() + "\", \"" + t.GetId() + "\");";
        //            ExecuteQuery(insertstring, true);
        //        }
        //    }
        //    else
        //    {
        //        LogInfo("InsertRegisteredIn", "Mannschaften are null!");
        //    }
        //}

        //private void InsertSpiel(BattleModel spiel, ScoreSheet sc)
        //{
        //    LogInfo("InsertSpiel", "spiel=<" + spiel.ToString() + ">, scoreSheetID=<" + sc.GetId() + ">");
        //    string insertstring = "INSERT INTO Spiele VALUES(\"" + spiel.GetId() + "\", \"" + sc.GetId() + "\", " + spiel.Index + ", \"" + spiel.Spieler1OriginalKey + "\", \"\", \"" + spiel.Spieler2OriginalKey + "\", \"\", \"\", \"\", " + spiel.Result.LegsHeim + ", " + spiel.Result.LegsGast + ", " + spiel.Result.SetsHeim + ", " + spiel.Result.SetsGast + ");";
        //    ExecuteQuery(insertstring, true);
        //}
        //#endregion

        //#region update
        //public void UpdatePlayer(Player oldPlayer, Player newPlayer)
        //{
        //    LogInfo("UpdatePlayer", "oldPlayer=<" + oldPlayer.ToString() + ">, newPlayer=<" + newPlayer.ToString() + ">");
        //    string querystring = "UPDATE Players Set vname=\"" + newPlayer.VorName + "\", nname=\"" + newPlayer.NachName + "\", geb=\"" + newPlayer.Geb.ToString("yyyy-MM-dd") +
        //        "\", scores=" + newPlayer.Scores.ToString() + ", imagename=\"" + newPlayer.ImageName + "\" WHERE pid=\"" + oldPlayer.GetId() + "\";";
        //    ExecuteQuery(querystring, false);
        //}

        //public void UpdateTeam(Team oldTeam, Team newTeam)
        //{
        //    LogInfo("UpdateTeam", "oldTeam=<" + oldTeam.ToString() + ">, newTeam=<" + newTeam + ">");
        //    string querystring = "UPDATE Teams Set name=\"" + newTeam.Name + "\", imagename=\"" + newTeam.ImageName + "\", vid=\"" + newTeam.Verein.GetId() + "\" WHERE TID=\"" + oldTeam.GetId() + "\";";
        //    OpenConnection();
        //    ExecuteQuery(querystring, true);
        //    DeletePlaysIn(oldTeam);
        //    InsertPlaysIn(newTeam);
        //    CloseConnection();
        //}

        //public void UpdateLokal(Lokal oldLokal, Lokal newLokal)
        //{
        //    LogInfo("UpdateLokal", "oldLokal=<" + oldLokal.ToString() + ">, newLokal=<" + newLokal.ToString() + ">");
        //    string querystring = "UPDATE Lokale Set name=\"" + newLokal.Name + "\", adresse=\"" + newLokal.Adresse + "\", telefon=\"" + newLokal.Telefon +
        //        "\", homepage=\"" + newLokal.HomePage + "\", imagename=\"" + newLokal.ImageName + "\" WHERE lid=\"" + oldLokal.GetId() + "\";";
        //    ExecuteQuery(querystring, false);
        //}

        //public void UpdateVerein(Verein oldVerein, Verein newVerein)
        //{
        //    LogInfo("UpdateVerein", "oldVerein=<" + oldVerein.ToString() + ">, newVerein=<" + newVerein.ToString() + ">");
        //    string querystring = "UPDATE Vereine Set name=\"" + newVerein.Name + "\", imagename=\"" + newVerein.ImageName + "\" WHERE vid=\"" + oldVerein.GetId() + "\";";
        //    ExecuteQuery(querystring, false);
        //}

        //public void UpdateScoreSheet(ScoreSheet scoreSheet)
        //{
        //    LogInfo("UpdateScoreSheet", "scoreSheet=<" + scoreSheet.HeimMannschaft.ToString() + " vs " + scoreSheet.GastMannschaft.ToString() + ">, index=<" + scoreSheet.GetId() + ">");
        //    string querystring = "UPDATE ScoreSheets Set datum=\"" + scoreSheet.Datum.ToString("yyyy-MM-dd") + "\", hleg=" + scoreSheet.EndErgebnis.LegsHeim + ", gleg=" + scoreSheet.EndErgebnis.LegsGast + ", hset=" + scoreSheet.EndErgebnis.SetsHeim + ", gset=" + scoreSheet.EndErgebnis.SetsGast + " WHERE scid=\"" + scoreSheet.GetId() + "\";";
        //    OpenConnection();
        //    ExecuteQuery(querystring, true);
        //    foreach (PlayerEntry entry in scoreSheet.HeimPlayerEntries)
        //    {
        //        UpdatePlayerEntry(entry);
        //    }
        //    foreach (PlayerEntry entry in scoreSheet.GastPlayerEntries)
        //    {
        //        UpdatePlayerEntry(entry);
        //    }
        //    foreach (BattleModel spiel in scoreSheet.Spiele)
        //    {
        //        UpdateSpiel(spiel);
        //    }
        //    CloseConnection();
        //}

        //private void UpdateSpiel(BattleModel battle)
        //{
        //    LogInfo("UpdateSpiel", "battle=<" + battle.GetId() + ">");
        //    string querystring = "UPDATE Spiele Set hplayer=\"" + (battle.Spieler1 != null ? battle.Spieler1.GetId().ToString() : "") + "\"";
        //    querystring += ", gplayer=\"" + (battle.Spieler2 != null ? battle.Spieler2.GetId().ToString() : "") + "\"";
        //    querystring += ", hpartner=\"" + (battle.Spieler1Partner != null ? battle.Spieler1Partner.GetId().ToString() : "") + "\"";
        //    querystring += ", gpartner=\"" + (battle.Spieler2Partner != null ? battle.Spieler2Partner.GetId().ToString() : "") + "\"";
        //    querystring += ", hleg=" + battle.Result.LegsHeim + ", gleg=" + battle.Result.LegsGast;
        //    querystring += ", hset=" + battle.Result.SetsHeim + ", gset=" + battle.Result.SetsGast + " WHERE spid=\"" + battle.GetId() + "\";";
        //    ExecuteQuery(querystring, true);
        //}

        //private void UpdatePlayerEntry(PlayerEntry entry)
        //{
        //    LogInfo("UpdatePlayerEntry", "entry=<" + entry.GetId() + ">");
        //    string querystring = "UPDATE PlayerEntries Set pkey=\"" + entry.Key + "\"";
        //    querystring += ", pid=\"" + (entry.Spieler != null ? entry.Spieler.GetId().ToString() : "") + "\"";
        //    querystring += ", scores=" + entry.Scores + " WHERE peid=\"" + entry.GetId() + "\";";
        //    ExecuteQuery(querystring, true);
        //}
        //#endregion

        //#region delete
        //public void DeletePlayer(Player p)
        //{
        //    LogInfo("DeletePlayer", "p=<" + p.ToString() + ">");
        //    DeletePlaysIn(p);
        //    string querystring = "DELETE FROM Players WHERE PID=\"" + p.GetId() + "\";";
        //    ExecuteQuery(querystring, false);
        //}

        //public void DeleteTeam(Team team)
        //{
        //    LogInfo("DeleteTeam", "Team=<" + team.ToString() + ">");
        //    DeletePlaysIn(team);
        //    string querystring = "DELETE FROM Teams WHERE TID=\"" + team.GetId() + "\";";
        //    ExecuteQuery(querystring, false);
        //}

        //public void DeleteLokal(Lokal l)
        //{
        //    LogInfo("DeleteLokal", "l=<" + l.ToString() + ">");
        //    string querystring = "DELETE FROM Lokale WHERE LID=\"" + l.GetId() + "\";";
        //    ExecuteQuery(querystring, false);
        //}

        //public void DeleteVerein(Verein v)
        //{
        //    LogInfo("DeleteVerein", "v=<" + v.ToString() + ">");
        //    string querystring = "UPDATE Teams SET VID=\"\" WHERE VID=\"" + v.GetId() + "\";"; //delete foreign keys
        //    OpenConnection();
        //    ExecuteQuery(querystring, true);
        //    querystring = "DELETE FROM Vereine WHERE VID=\"" + v.GetId() + "\";";
        //    ExecuteQuery(querystring, true);
        //    CloseConnection();
        //}

        //private void DeletePlaysIn(Team team)
        //{
        //    LogInfo("DeletePlaysIn", "Team=<" + team + ">");
        //    string querystring = "DELETE FROM PlaysIn WHERE TID=\"" + team.GetId() + "\";";
        //    ExecuteQuery(querystring, true);
        //}

        //private void DeletePlaysIn(Player p)
        //{
        //    LogInfo("DeletePlaysIn", "Player=<" + p + ">");
        //    string querystring = "DELETE FROM PlaysIn WHERE PID=\"" + p.GetId() + "\";";
        //    ExecuteQuery(querystring, true);
        //}
        #endregion

		#region IO
		private void LogInfo(String Function, String Message)
		{
			this.writer.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\t" + Function + "\t" + Message);
		}

        //private string ReadFile(String sFile)
        //{
        //    string sContent = "";
        //    string sPath = System.IO.Path.Combine(path, sFile);
        //    if (File.Exists(sPath))
        //    {
        //        StreamReader myFile = new StreamReader(sPath, System.Text.Encoding.Default);
        //        sContent = myFile.ReadToEnd();
        //        myFile.Close();
        //    }

        //    sContent = sContent.Replace("\r", "");

        //    return sContent;
        //}
		#endregion

		#region private methods
		private void CreateTables()
		{
			LogInfo("CreateTables", "Read setup from " + this.config.SetupFile);
			List<Node> xml = XMLReader.ReadXMLFile(this.config.SetupFile);
			if (xml != null)
			{
				List<Node> tables = null;
				foreach (Node n in xml)
				{
					if (n.Name == "Tables")
					{
						tables = n.Childs;
						break;
					}
				}
				if (tables != null)
				{
					foreach (Node n in tables)
					{
						string stmt = GetStatement(n);
						if (!string.IsNullOrEmpty(stmt))
							ExecuteQuery(stmt, true);
					}
				}
			}

			string f = FileWorker.ReadFile(this.config.TestValuesFile);
			string[] commands = f.Split('\n');
			foreach (string s in commands)
			{
				if (s != "")
					ExecuteQuery(s, true);
			}
		}

		private string GetStatement(Node node)
		{
			string stmt = "";
			if (node.Name == "Table")
			{
				stmt = "CREATE TABLE " + node.Attributes["name"] + " (";
				List<string> columns = new List<string>();
				List<string> primaryKeys = new List<string>();
				List<string> foreignKeys = new List<string>();
				List<string> references = new List<string>();
				List<string> foreignKeyReferences = new List<string>();
				foreach (Node n in node.Childs)
				{
					if(n.Name == "Column")
					{
						string s = n.Attributes["name"] + " " + n.Attributes["type"];
						if (n.Attributes["type"] == "VARCHAR")
							s += "(32)";
						if (n.Attributes.Keys.Contains("autoincrement") && n.Attributes["autoincrement"] == "true")
							s += " AUTOINCREMENT";
						columns.Add(s);
						if (n.Attributes.Keys.Contains("primarykey") && n.Attributes["primarykey"] == "true")
							primaryKeys.Add(n.Attributes["name"]);
						if (n.Attributes.Keys.Contains("foreignkey") && n.Attributes.Keys.Contains("reference"))
						{
							foreignKeys.Add(n.Attributes["name"]);
							references.Add(n.Attributes["reference"]);
							foreignKeyReferences.Add(n.Attributes["foreignkey"]);
						}
					}
				}
				stmt += string.Join(", ", columns.ToArray());
				if (primaryKeys.Count > 0)
				{
					stmt += ", PRIMARY KEY(" + string.Join(", ", primaryKeys.ToArray()) + ")";
				}
				if (foreignKeys.Count > 0)
				{
					for(int i = 0; i< foreignKeys.Count; i++)
					{
						stmt += ", FOREIGN KEY(" + foreignKeys[i] + ") REFERENCES " + references[i] + "(" + foreignKeyReferences[i] + ")";
					}
				}
				stmt += ");";
			}
			return stmt;
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
		#endregion*/
    }
}