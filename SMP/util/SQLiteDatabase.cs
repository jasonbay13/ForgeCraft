using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;

namespace SMP
{
	public class SQLiteDatabase
	{
		private string dbConnection;
		
		public SQLiteDatabase ()
		{
			
			dbConnection = "Data Source=properties/ForgeCraft.db";
			
			if (!File.Exists("properties/ForgeCraft.db"))
			{
				SQLiteConnection.CreateFile("properties/ForgeCraft.db");
				if (!WriteDefault())
					Server.Log("Couldn't write default DataBase");
			}
			
		}
		
		public SQLiteDatabase (string file)
		{
			if (!File.Exists(file))
				SQLiteConnection.CreateFile("Data Source=" + file);
			
			dbConnection = "Data Source=" + file;
		}
		
		public DataTable GetDataTable(string sql)
	    {
	        DataTable dt = new DataTable();
	        try
	        {
	            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
	            cnn.Open();
	            SQLiteCommand mycommand = new SQLiteCommand(cnn);
	            mycommand.CommandText = sql;
	            SQLiteDataReader reader = mycommand.ExecuteReader();
	            dt.Load(reader);
	            reader.Close();
	            cnn.Close();
	        }
	        catch (Exception e)
	        {
	            throw new Exception(e.Message);
	        }
	        return dt;
	    }
	     
	    public int ExecuteNonQuery(string sql)
	    {
			//Console.WriteLine("sql nonquery: " + sql);
	        SQLiteConnection cnn = new SQLiteConnection(dbConnection);
	        cnn.Open();
	        SQLiteCommand mycommand = new SQLiteCommand(cnn);
	        mycommand.CommandText = sql;
	        int rowsUpdated = mycommand.ExecuteNonQuery();
	        cnn.Close();
			//Console.WriteLine("rowsupdated: " + rowsUpdated);
	        return rowsUpdated;
	    }
	 
	    public string ExecuteScalar(string sql)
	    {
	        SQLiteConnection cnn = new SQLiteConnection(dbConnection);
	        cnn.Open();
	        SQLiteCommand mycommand = new SQLiteCommand(cnn);
	        mycommand.CommandText = sql;
	        object value = mycommand.ExecuteScalar();
	        cnn.Close();
	        if (value != null)
	        {
	            return value.ToString();
	        }
	        return "";
	    }
	 
	    public bool Update(String tableName, Dictionary<String, String> data, String where)
	    {
	        String vals = "";
	        Boolean returnCode = true;
	        if (data.Count >= 1)
	        {
	            foreach (KeyValuePair<String, String> val in data)
	            {
	                vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
	            }
	            vals = vals.Substring(0, vals.Length - 1);
	        }
	        try
	        {
	            this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
	        }
	        catch
	        {
	            returnCode = false;
	        }
	        return returnCode;
	    }
	 
	    public bool Delete(String tableName, String where)
	    {
	        Boolean returnCode = true;
	        try
	        {
	            this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
	        }
	        catch (Exception e)
	        {
	            Server.ServerLogger.LogError(e);
	            returnCode = false;
	        }
	        return returnCode;
	    }
	 
   	    public bool Insert(String tableName, Dictionary<String, String> data)
   	    {
   	        String columns = "";
   	        String values = "";
   	        Boolean returnCode = true;
   	        foreach (KeyValuePair<String, String> val in data)
   	        {
   	            columns += String.Format(" {0},", val.Key.ToString());
   	            values += String.Format(" '{0}',", val.Value);
   	        }
   	        columns = columns.Substring(0, columns.Length - 1);
   	        values = values.Substring(0, values.Length - 1);
   	        try
   	        {
   	            this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
   	        }
   	        catch(Exception e)
   	        {
	            Server.ServerLogger.LogError(e);
   	            returnCode = false;
   	        }
   	        return returnCode;
   	    }
		
   	    public bool ClearDB()
   	    {
   	        DataTable tables;
   	        try
   	        {
   	            tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
   	            foreach (DataRow table in tables.Rows)
   	            {
   	                this.ClearTable(table["NAME"].ToString());
   	            }
   	            return true;
	        }
	        catch
	        {
	            return false;
	        }
	    }
	 
	    public bool ClearTable(String table)
	    {
	        try
	        {
	             
	            this.ExecuteNonQuery(String.Format("delete from {0};", table));
	            return true;
	        }
	        catch
	        {
	            return false;
	        }
		}
		
		private bool WriteDefault()
		{
			try
			{
				this.ExecuteNonQuery(
	                    "CREATE TABLE Track(" +
	                    "ID 		INTEGER PRIMARY KEY, " +
	                    "Name 		TEXT, " +
				        "Groups		TEXT" +
	                    ");"
				                     );
				
				this.ExecuteNonQuery(
						"CREATE TABLE Groups(" +
						"ID 		INTEGER PRIMARY KEY, " +
						"Name 		TEXT, " +
						"IsDefault 	INTEGER, " +
						"CanBuild 	INTEGER, " +
						"Prefix 	TEXT, " +
						"Suffix 	TEXT, " +
						"Color 		TEXT, " +
						"Permissions TEXT, " +
				        "Inheritance TEXT, " +
						"Tracks 	TEXT " +
						");"
				                     );
				
				this.ExecuteNonQuery(
				       	"CREATE TABLE Currency(" +
				       	"ID 		INTEGER PRIMARY KEY, " +
				       	"Name 		TEXT, " +
				       	"Major 		TEXT, " +
				       	"SingleMajor TEXT, " +
				       	"Minor 		TEXT, " +
				       	"SingleMinor TEXT" +
				       	");"
				                     );
				
				this.ExecuteNonQuery(
				        "CREATE TABLE Bank(" +
				        "ID 		INTEGER PRIMARY KEY, " +
				        "Name		TEXT, " +
				        "IsDefault  INTEGER, " +
				        "OpenFee 	NUMERIC, " +
				        "InitialHolding NUMERIC, " +
				        "InterestInterval INTEGER, " +
				        "DefaultInterest NUMERIC, " +
				        "CurrencyID INTEGER, " +
				        "FOREIGN KEY (CurrencyID) REFERENCES Currency(ID)" +
				        ");"
				                     );
				
				this.ExecuteNonQuery(
				        "CREATE TABLE Account(" +
				        "ID 		INTEGER PRIMARY KEY, " +
				        "Name 		TEXT, " +
				        "PlayerAccount INTEGER, " +
				        "Balance 	NUMERIC, " +
				        "Interest 	NUMERIC, " +
				        "Players 	TEXT, " +
				        "BankID 	INTEGER, " +
				        "FOREIGN KEY (BankID) REFERENCES Bank(ID)" +
				        ");"
				                     );
				
				this.ExecuteNonQuery(
						"CREATE TABLE Player(" +
						"ID 		INTEGER PRIMARY KEY, " +
						"Name 		TEXT, " +
						"ip 		TEXT, " +
						"NickName 	TEXT, " +
						"CanBuild 	INTEGER, " +
						"Prefix 	TEXT, " +
						"Suffix 	TEXT, " +
						"Color 		TEXT, " +
						"DND 		INTEGER, " +
						"GodMode 	INTEGER, " +
						"DefAccount INTEGER, " +
						"OtherAcc 	TEXT, " +
						"GroupID 	INTEGER, " +
						"SubGroups 	TEXT, " +
						"ExtraPerms TEXT, " +
						"FOREIGN KEY(DefAccount) REFERENCES Account(ID), " +
						"FOREIGN KEY(GroupID) REFERENCES Groups(ID)" +
						");"
				                     );
				
				this.ExecuteNonQuery(
				        "CREATE TABLE Permission(" +
				        "ID 		INTEGER PRIMARY KEY, " +
				        "Node TEXT" +
				        ");"
				                     );
				
				this.ExecuteNonQuery(
				      	"CREATE TABLE Item(" +
				      	"ID 		INTEGER PRIMARY KEY, " +
				      	"Value 		INTEGER, " +
				      	"Meta 		INTEGER, " +
				      	"Alias TEXT" +
				      	");"
				                     );
				
				this.ExecuteNonQuery(
				        "INSERT INTO Track VALUES(1,'Default', '1,2,3,4');"
				                     );
				
				this.ExecuteNonQuery(
				        "INSERT INTO Groups VALUES(1,'Guest',1,1,NULL,NULL,'%3', '2,3,4', NULL, 1);"
				                     );
				this.ExecuteNonQuery(
				        "INSERT INTO Groups VALUES(2,'Builder',0,1,NULL,NULL,'%1', '7', '1', '1');"
				                     );
				this.ExecuteNonQuery(
				        "INSERT INTO Groups VALUES(3,'Moderator',0,1,NULL,NULL,'%c', '4,5,6', '2', '1');"
				                     );
				this.ExecuteNonQuery(
				        "INSERT INTO Groups VALUES(4,'Admin',0,1,NULL,NULL,'%7', '1', '3', '1');"
				                     );
				
				this.ExecuteNonQuery(
						"INSERT INTO Currency VALUES(1,'Dollars','Dollars','Dollar','Cents','Cent');"                
				                     );
				
				this.ExecuteNonQuery(
				        "INSERT INTO Bank VALUES(1,'Forger''s Investors',1,0,0,0,0,1);"
				                     );
				
				this.ExecuteNonQuery(
				        "INSERT INTO Account VALUES(1,'Silentneeb',1,1000,1.1,1,1);"
				                     );
				
				this.ExecuteNonQuery(
				        "INSERT INTO Player VALUES(1,'Silentneeb',NULL,'Silent',1,NULL,NULL,'%4', 1, NULL, 1, NULL, 1, NULL, NULL);"
				                     );
				
				this.ExecuteNonQuery(
				        "INSERT INTO Permission VALUES(1,'*')"
				                     );
				this.ExecuteNonQuery(
				     	"INSERT INTO Permission(Node) VALUES('core.info.*')"
				                     );
				this.ExecuteNonQuery(
				     	"INSERT INTO Permission(Node) VALUES('core.general.*')"
				                     );
				this.ExecuteNonQuery(
				     	"INSERT INTO Permission(Node) VALUES('core.cheat.hackz')"
				                     );
				this.ExecuteNonQuery(
				     	"INSERT INTO Permission(Node) VALUES('core.other.*')"
				                     );
				this.ExecuteNonQuery(
				     	"INSERT INTO Permission(Node) VALUES('core.mod.*')"
				                     );
				this.ExecuteNonQuery(
				     	"INSERT INTO Permission(Node) VALUES('core.build.*')"
				                     );
				
			}
			catch
			{
				return false;
			}
			return true;
		}
		
	}
	
}