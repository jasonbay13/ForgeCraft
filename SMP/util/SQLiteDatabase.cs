/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using SMP.util;

namespace SMP
{
	public class SQLiteDatabase
	{
		private string dbConnection;
		
		internal SQLiteDatabase ()
		{
			
			dbConnection = "Data Source=properties/ForgeCraft.db";
			if (!Directory.Exists("properties"))
				Directory.CreateDirectory("properties");

			if (!File.Exists("properties/ForgeCraft.db"))
			{
				Logger.Log("Writing new Database");
				SQLiteConnection.CreateFile("properties/ForgeCraft.db");
				if (!WriteDefault())
					Logger.Log("Couldn't write default Database");
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
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace.ToString());
	        }
	        return dt;
	    }
	     
	    public int ExecuteNonQuery(string sql)
	    {
			//Logger.Log(sql);
	        SQLiteConnection cnn = new SQLiteConnection(dbConnection);
	        cnn.Open();
	        SQLiteCommand mycommand = new SQLiteCommand(cnn);
	        mycommand.CommandText = sql;
	        int rowsUpdated = mycommand.ExecuteNonQuery();
	        cnn.Close();
	        return rowsUpdated;
	    }
	 
	    public string ExecuteScalar(string sql)
	    {
			//Logger.Log(sql);
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
				//Logger.Log(String.Format("update {0} set {1} where {2};", tableName, vals, where));
	            this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
	        }
	        catch (Exception e)
	        {
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace.ToString());
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
	            Logger.LogError(e);
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
				//Logger.Log(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
   	            this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
   	        }
   	        catch (Exception e)
	        {
				Logger.Log(e.Message);
				Logger.Log(e.StackTrace.ToString());
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
            using (SQLiteConnection cnn = new SQLiteConnection(dbConnection))
            {
                cnn.Open();
                using (SQLiteTransaction dbTrans = cnn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        try
                        {
                            #region CREATETABLES
                            cmd.CommandText =
                                    "CREATE TABLE Inventory(" +
                                    "ID INTEGER PRIMARY KEY, " +
                                    "slot0 			TEXT, " +
                                    "slot1 			TEXT, " +
                                    "slot2 			TEXT, " +
                                    "slot3 			TEXT, " +
                                    "slot4 			TEXT, " +
                                    "slot5 			TEXT, " +
                                    "slot6			TEXT, " +
                                    "slot7			TEXT, " +
                                    "slot8			TEXT, " +
                                    "slot9			TEXT, " +
                                    "slot10			TEXT, " +
                                    "slot11			TEXT, " +
                                    "slot12			TEXT, " +
                                    "slot13			TEXT, " +
                                    "slot14			TEXT, " +
                                    "slot15			TEXT, " +
                                    "slot16			TEXT, " +
                                    "slot17			TEXT, " +
                                    "slot18			TEXT, " +
                                    "slot19			TEXT, " +
                                    "slot20			TEXT, " +
                                    "slot21			TEXT, " +
                                    "slot22			TEXT, " +
                                    "slot23			TEXT, " +
                                    "slot24			TEXT, " +
                                    "slot25			TEXT, " +
                                    "slot26			TEXT, " +
                                    "slot27			TEXT, " +
                                    "slot28			TEXT, " +
                                    "slot29			TEXT, " +
                                    "slot30			TEXT, " +
                                    "slot31			TEXT, " +
                                    "slot32			TEXT, " +
                                    "slot33			TEXT, " +
                                    "slot34			TEXT, " +
                                    "slot35			TEXT, " +
                                    "slot36			TEXT, " +
                                    "slot37			TEXT, " +
                                    "slot38			TEXT, " +
                                    "slot39			TEXT, " +
                                    "slot40			TEXT, " +
                                    "slot41			TEXT, " +
                                    "slot42			TEXT, " +
                                    "slot43			TEXT, " +
                                    "slot44			TEXT" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Track(" +
                                    "ID 		INTEGER PRIMARY KEY, " +
                                    "Name 		TEXT, " +
                                    "Groups		TEXT" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Groups(" +
                                    "ID 		INTEGER PRIMARY KEY, " +
                                    "Name 		TEXT, " +
                                    "PermLevel	INTEGER, " +
                                    "IsDefault 	INTEGER, " +
                                    "CanBuild 	INTEGER, " +
                                    "Prefix 	TEXT, " +
                                    "Suffix 	TEXT, " +
                                    "Color 		TEXT, " +
                                    "Permissions TEXT, " +
                                    "Inheritance TEXT, " +
                                    "Tracks 	TEXT " +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Currency(" +
                                    "ID 		INTEGER PRIMARY KEY, " +
                                    "Name 		TEXT, " +
                                    "Major 		TEXT, " +
                                    "SingleMajor TEXT, " +
                                    "Minor 		TEXT, " +
                                    "SingleMinor TEXT" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
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
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
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
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Player(" +
                                    "ID 		INTEGER PRIMARY KEY, " +
                                    "Name 		TEXT, " +
                                    "ip 		TEXT, " +
                                    "InventoryID	INTEGER, " +
                                    "Exp		INTEGER, " +
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
                                    "FOREIGN KEY(GroupID) REFERENCES Groups(ID), " +
                                    "FOREIGN KEY(InventoryID) REFERENCES Inventory(ID)" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Permission(" +
                                    "ID 		INTEGER PRIMARY KEY, " +
                                    "Node TEXT" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Item(" +
                                    "ID 		INTEGER PRIMARY KEY, " +
                                    "Value 		INTEGER, " +
                                    "Meta 		INTEGER, " +
                                    "Alias TEXT" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Warp(" +
                                    "ID			INTEGER PRIMARY KEY, " +
                                    "Name		TEXT, " +
                                    "X			INTEGER, " +
                                    "Y			INTEGER, " +
                                    "Z			INTEGER," +
                                    "World  	TEXT" +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE Sign(" +
                                    "X			INTEGER, " +
                                    "Y			INTEGER, " +
                                    "Z			INTEGER, " +
                                    "World  	TEXT, " +
                                    "Line1		TEXT, " +
                                    "Line2		TEXT, " +
                                    "Line3		TEXT, " +
                                    "Line4		TEXT " +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText =
                                    "CREATE TABLE SavedLoc(" +
                                    "Username  	TEXT, " +
                                    "World		TEXT, " +
                                    "X			DOUBLE, " +
                                    "Y			DOUBLE, " +
                                    "Z			DOUBLE, " +
                                    "Yaw		FLOAT, " +
                                    "Pitch		FLOAT " +
                                    ");"
                                                 ;
                            cmd.ExecuteNonQuery();
                            #endregion

                            #region INSERTS

                            cmd.CommandText = "INSERT INTO Track VALUES(1,'Default', '1,2,3,4');"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Groups VALUES(1,'Guest',10,1,1,NULL,NULL,'%3', '2,3,4', NULL, 1);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Groups VALUES(2,'Builder',20,0,1,NULL,NULL,'%1', '7', '1', '1');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Groups VALUES(3,'Moderator',30,0,1,NULL,NULL,'%c', '4,5,6', '2', '1');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Groups VALUES(4,'Admin',40,0,1,NULL,NULL,'%7', '1', '3', '1');"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Currency VALUES(1,'Dollars','Dollars','Dollar','Cents','Cent');"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Bank VALUES(1,'Forger''s Investors',1,0,0,0,0,1);"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Account VALUES(1,'Silentneeb',1,1000,1.1,1,1);"; cmd.ExecuteNonQuery();

                            //add whoever is missing, or change anything
                            cmd.CommandText = "INSERT INTO Player(ID, Name, InventoryID, NickName, CanBuild, Color, DefAccount, GroupID) VALUES(1, 'Silentneeb', 1, 'Silent', 1, '%4', 1, 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(2, 'EricKilla', 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(3, 'edh649', 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(4, 'Merlin33069', 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(5, 'hypereddie10', 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(6, 'Soccer101nic', 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(7, 'quaisaq', 4);"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Player(ID, Name, GroupID) VALUES(8, 'headdetect', 4);"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Permission VALUES(1,'*');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Permission(Node) VALUES('core.info.*');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Permission(Node) VALUES('core.general.*');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Permission(Node) VALUES('core.cheat.hacks');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Permission(Node) VALUES('core.other.*');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Permission(Node) VALUES('core.mod.*');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Permission(Node) VALUES('core.build.*');"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Inventory(ID, slot36) VALUES(1, '278:0:1');"; cmd.ExecuteNonQuery();
                            #endregion
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e.Message.ToString());
                            Logger.Log(e.StackTrace.ToString());
                            return false;
                        }
                    }
                    dbTrans.Commit();
                }
                cnn.Close();
            }
            additems();
            return true;
        }

        private void additems()
        {
            //this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES();");
            using (SQLiteConnection cnn = new SQLiteConnection(dbConnection))
            {
                cnn.Open();
                using (SQLiteTransaction dbTrans = cnn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        try
                        {
                            #region BLOCKS
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(0, 0, 'air');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(1, 0, 'stone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2, 0, 'grass');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(3, 0, 'dirt');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(4, 0, 'cobblestone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(5, 0, 'woodenplank');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(6, 0, 'sapling');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(6, 1, 'pinesapling');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(6, 2, 'birchsapling');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(7, 0, 'bedrock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(8, 0, 'water');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(9, 0, 'stillwater');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(10, 0, 'lava');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(11, 0, 'stilllava');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(12, 0, 'sand');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(13, 0, 'gravel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(14, 0, 'goldore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(15, 0, 'ironore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(16, 0, 'coalore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(17, 0, 'wood');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(17, 1, 'pinewood');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(17, 2, 'birchwood');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(18, 0, 'leaves');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(19, 0, 'sponge');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(20, 0, 'glass');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(21, 0, 'lapisluzuliore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(22, 0, 'lapisluzuliblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(23, 0, 'dispenser');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(24, 0, 'sandstone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(25, 0, 'noteblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(26, 0, 'bedblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(27, 0, 'poweredrail');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(28, 0, 'detectorrail');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(29, 0, 'stickypiston');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(30, 0, 'cobweb');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(31, 0, 'tallgrass');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(32, 0, 'deadshrubs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(33, 0, 'piston');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(34, 0, 'pistonhead');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(34, 1, 'stickypistonhead');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 0, 'wool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 1, 'orangewool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 2, 'magentawool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 3, 'lightbluewool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 4, 'yellowwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 5, 'limewool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 6, 'pinkwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 7, 'graywool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 8, 'lightgraywool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 9, 'cyanwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 10, 'purplewool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 11, 'bluewool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 12, 'brownwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 13, 'greenwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 14, 'redwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(35, 15, 'blackwool');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(36, 0, 'movedblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(37, 0, 'dandelion');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(38, 0, 'rose');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(39, 0, 'brownmushroom');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(40, 0, 'redmushroom');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(41, 0, 'goldblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(42, 0, 'ironblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(43, 0, 'doubleslabs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(44, 0, 'slabs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(45, 0, 'brickblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(46, 0, 'tnt');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(47, 0, 'bookshelf');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(48, 0, 'mossstone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(49, 0, 'obsidian');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(50, 0, 'torch');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(51, 0, 'fire');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(52, 0, 'mobspawner');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(53, 0, 'woodenstiars');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(54, 0, 'chest');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(55, 0, 'redstonewire');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(56, 0, 'diamondore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(57, 0, 'diamondblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(58, 0, 'craftingtable');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(58, 0, 'workbench');"; cmd.ExecuteNonQuery();
                            //grown wheat??
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(60, 0, 'farmland');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(61, 0, 'furnace');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(62, 0, 'burningfurnace');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(63, 0, 'signpost');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(64, 0, 'wooddoorblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(65, 0, 'ladders');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(66, 0, 'rails');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(67, 0, 'cobblestonestairs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(68, 0, 'wallsign');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(69, 0, 'lever');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(70, 0, 'stonepressureplate' );"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(71, 0, 'irondoorblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(72, 0, 'woodenpressureplate');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(73, 0, 'redstoneore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(74, 0, 'glowingredstoneore');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(75, 0, 'offredstonetorch');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(76, 0, 'redstonetorch');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(77, 0, 'stonebutton');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(78, 0, 'snow');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(79, 0, 'ice');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(80, 0, 'snowblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(81, 0, 'cactus');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(82, 0, 'clayblock');"; cmd.ExecuteNonQuery();
                            //grown sugar cane??
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(84, 0, 'jukebox');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(85, 0, 'fence');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(86, 0, 'pumpkin');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(87, 0, 'netherrack');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(88, 0, 'soulsand');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(89, 0, 'glowstoneblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(90, 0, 'portalblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(91, 0, 'jackolantern');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(92, 0, 'cakeblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(93, 0, 'offredstonerepeater');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(94, 0, 'onredstonerepeater');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(95, 0, 'lockedchest');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(96, 0, 'trapdoor');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(97, 0, 'silverfishstone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(98, 0, 'stonebrick');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(98, 1, 'mossystonebrick');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(98, 2, 'crackedstonebrick');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(99, 0, 'hugebrownmushroom');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(100, 0, 'hugeredmushroom');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(101, 0, 'ironbars');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(102, 0, 'glasspane');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(103, 0, 'melon');"; cmd.ExecuteNonQuery();
                            //melon and pumpkin stem??
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(106, 0, 'vines');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(107, 0, 'fencegate');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(108, 0, 'brickstairs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(109, 0, 'stonebrickstairs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(110, 0, 'mycelium');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(111, 0, 'lillypad');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(112, 0, 'netherbrick');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(113, 0, 'netherbrickfence');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(114, 0, 'netherbrickstairs');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(115, 0, 'netherwartblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(116, 0, 'enchantmenttable');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(117, 0, 'brewingstandblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(118, 0, 'cauldronblock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(119, 0, 'airportal');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(120, 0, 'airportalframe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(121, 0, 'whitestone');"; cmd.ExecuteNonQuery();
                            #endregion

                            #region ITEMS
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(256, 0, 'ironshovel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(257, 0, 'ironpickaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(258, 0, 'ironaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(259, 0, 'flintandsteel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(260, 0, 'redapple');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(261, 0, 'bow');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(262, 0, 'arrow');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(263, 0, 'coal');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(263, 0, 'charcoal');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(264, 0, 'diamond');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(265, 0, 'ironingot');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(266, 0, 'goldingot');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(267, 0, 'ironsword');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(268, 0, 'woodensword');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(269, 0, 'woodenshovel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(270, 0, 'woodenpickaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(271, 0, 'woodenaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(272, 0, 'stonesword');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(273, 0, 'stoneshovel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(274, 0, 'stonepickaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(275, 0, 'stoneaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(276, 0, 'diamondsword');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(277, 0, 'diamondshovel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(278, 0, 'diamondpickaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(279, 0, 'diamondaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(280, 0, 'stick');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(281, 0, 'bowl');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(282, 0, 'mushroomsoup');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(283, 0, 'goldsword');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(284, 0, 'goldshovel');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(285, 0, 'goldpickaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(286, 0, 'goldaxe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(287, 0, 'string');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(288, 0, 'feather');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(289, 0, 'gunpowder');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(290, 0, 'woodenhoe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(291, 0, 'stonehoe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(292, 0, 'ironhoe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(293, 0, 'diamondhoe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(294, 0, 'goldhoe');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(295, 0, 'seeds');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(296, 0, 'wheat');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(297, 0, 'bread');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(298, 0, 'leathercap');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(299, 0, 'leathertunic');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(300, 0, 'leatherpants');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(301, 0, 'leatherboots');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(302, 0, 'chainhelmet');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(303, 0, 'chainchestplate');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(304, 0, 'chainleggings');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(305, 0, 'chainboots');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(306, 0, 'ironhelmet');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(307, 0, 'ironchestplate');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(308, 0, 'ironleggings');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(309, 0, 'ironboots');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(310, 0, 'diamondhelmet');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(311, 0, 'diamondchestplate');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(312, 0, 'diamondleggings');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(313, 0, 'diamondboots');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(314, 0, 'goldhelmet');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(315, 0, 'goldchestplate');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(316, 0, 'goldleggings');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(317, 0, 'goldboots');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(318, 0, 'flint');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(319, 0, 'rawporkchop');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(320, 0, 'cookedporkchop');"; cmd.ExecuteNonQuery();//
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(321, 0, 'painting');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(322, 0, 'goldenapple');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(323, 0, 'sign');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(324, 0, 'wodendoor');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(325, 0, 'bucket');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(326, 0, 'waterbucket');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(327, 0, 'lavabuket');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(328, 0, 'minecart');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(329, 0, 'saddle');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(330, 0, 'irondoor');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(331, 0, 'redstone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(332, 0, 'snowball');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(333, 0, 'boat');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(334, 0, 'leather');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(335, 0, 'milk');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(336, 0, 'claybrick');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(337, 0, 'clay');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(338, 0, 'sugarcane');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(339, 0, 'paper');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(340, 0, 'book');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(341, 0, 'slimeball');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(342, 0, 'chestminecart');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(343, 0, 'furnaceminecart');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(344, 0, 'egg');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(345, 0, 'compass');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(346, 0, 'fishingrod');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(347, 0, 'clock');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(348, 0, 'glowstonedust');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(349, 0, 'rawfish');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(350, 0, 'cookedfish');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 0, 'blackdye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 1, 'reddye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 2, 'greendye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 3, 'browndye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 4, 'bluedye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 5, 'purpledye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 6, 'cyandye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 7, 'lightgraydye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 8, 'graydye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 9, 'pinkdye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 10, 'limedye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 11, 'yellowdye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 12, 'lightbluedye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 13, 'magentadye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 14, 'orangedye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(351, 15, 'whitedye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(352, 0, 'bone');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(353, 0, 'sugar');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(354, 0, 'cake');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(355, 0, 'bed');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(356, 0, 'redstonerepeater');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(357, 0, 'cookie');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(358, 0, 'map');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(359, 0, 'shears');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(360, 0, 'melonslice');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(361, 0, 'pumpkinseeds');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(362, 0, 'melonseeds');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(363, 0, 'rawsteak');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(364, 0, 'cookedsteak');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(365, 0, 'rawchicken');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(366, 0, 'cookedchicken');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(367, 0, 'rottenflesh');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(368, 0, 'enderpearl');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(369, 0, 'blazerod');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(370, 0, 'ghasttear');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(371, 0, 'goldnugget');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(372, 0, 'netherwart');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(373, 0, 'potion');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(374, 0, 'glassbottle');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(375, 0, 'spidereye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(376, 0, 'fermentedspidereye');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(377, 0, 'blazepowder');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(378, 0, 'magmacream');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(379, 0, 'brewingstand');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(380, 0, 'cauldron');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(381, 0, 'eyeofender');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(382, 0, 'glisteringmelon');"; cmd.ExecuteNonQuery();

                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2256, 0, 'goldmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2557, 0, 'greenmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2258, 0, 'blocksmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2259, 0, 'chirpmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2260, 0, 'farmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2261, 0, 'mallmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2262, 0, 'mellohimusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2263, 0, 'stalmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2264, 0, 'stradmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2265, 0, 'wardmusicdisc');"; cmd.ExecuteNonQuery();
                            cmd.CommandText = "INSERT INTO Item(Value, Meta, Alias) VALUES(2266, 0, '11musicdisc');"; cmd.ExecuteNonQuery();
                            #endregion
                        }
                        catch (Exception e) { Logger.Log(e.Message.ToString()); Logger.Log(e.StackTrace.ToString()); }

                    }
                    dbTrans.Commit();
                }
                cnn.Close();
            }
        }
		
	}	
}