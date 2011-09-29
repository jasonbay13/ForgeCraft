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
				Server.Log("Writing new Database");
				SQLiteConnection.CreateFile("properties/ForgeCraft.db");
				if (!WriteDefault())
					Server.Log("Couldn't write default Database");
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
				Server.Log(e.Message);
				Server.Log(e.StackTrace.ToString());
	        }
	        return dt;
	    }
	     
	    public int ExecuteNonQuery(string sql)
	    {
			//Server.Log(sql);
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
			//Server.Log(sql);
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
				//Server.Log(String.Format("update {0} set {1} where {2};", tableName, vals, where));
	            this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
	        }
	        catch (Exception e)
	        {
				Server.Log(e.Message);
				Server.Log(e.StackTrace.ToString());
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
				//Server.Log(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
   	            this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
   	        }
   	        catch (Exception e)
	        {
				Server.Log(e.Message);
				Server.Log(e.StackTrace.ToString());
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
				#region CREATETABLES
				this.ExecuteNonQuery(
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
				                     );
				
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
				#endregion
				
				#region INSERTS
				this.ExecuteNonQuery("INSERT INTO Track VALUES(1,'Default', '1,2,3,4');");
				
				this.ExecuteNonQuery("INSERT INTO Groups VALUES(1,'Guest',10,1,1,NULL,NULL,'%3', '2,3,4', NULL, 1);");
				this.ExecuteNonQuery("INSERT INTO Groups VALUES(2,'Builder',20,0,1,NULL,NULL,'%1', '7', '1', '1');");
				this.ExecuteNonQuery("INSERT INTO Groups VALUES(3,'Moderator',30,0,1,NULL,NULL,'%c', '4,5,6', '2', '1');");
				this.ExecuteNonQuery("INSERT INTO Groups VALUES(4,'Admin',40,0,1,NULL,NULL,'%7', '1', '3', '1');");
				
				this.ExecuteNonQuery("INSERT INTO Currency VALUES(1,'Dollars','Dollars','Dollar','Cents','Cent');");
				
				this.ExecuteNonQuery("INSERT INTO Bank VALUES(1,'Forger''s Investors',1,0,0,0,0,1);");
				
				this.ExecuteNonQuery("INSERT INTO Account VALUES(1,'Silentneeb',1,1000,1.1,1,1);");
				
				//add whoever is missing, or change anything
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, InventoryID, NickName, CanBuild, Color, DefAccount, GroupID) VALUES(1, 'Silentneeb', 1, 'Silent', 1, '%4', 1, 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(2, 'EricKilla', 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(3, 'edh649', 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(4, 'Merlin33069', 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(5, 'hypereddie10', 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(6, 'Soccer101nic', 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(7, 'quaisaq', 4);");
				this.ExecuteNonQuery("INSERT INTO Player(ID, Name, GroupID) VALUES(8, 'headdetect', 4);");
				
				this.ExecuteNonQuery("INSERT INTO Permission VALUES(1,'*');");
				this.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES('core.info.*');");
				this.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES('core.general.*');");
				this.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES('core.cheat.hacks');");
				this.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES('core.other.*');");
				this.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES('core.mod.*');");
				this.ExecuteNonQuery("INSERT INTO Permission(Node) VALUES('core.build.*');");
				
				this.ExecuteNonQuery("INSERT INTO Inventory(ID, slot36) VALUES(1, '278:0:1');");
				#endregion
			}
			catch (Exception e)
			{
				Server.Log(e.Message.ToString());
				Server.Log(e.StackTrace.ToString());
				return false;
			}
			additems();
			return true;
		}
		
		private void additems()
		{
			//this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES();");
			
			try{
			#region BLOCKS
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(0, 0, 'air');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(1, 0, 'stone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(2, 0, 'grass');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(3, 0, 'dirt');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(4, 0, 'cobblestone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(5, 0, 'woodenplank');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(6, 0, 'sapling');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(6, 1, 'pinesapling');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(6, 2, 'birchsapling');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(7, 0, 'bedrock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(8, 0, 'water');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(9, 0, 'stillwater');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(10, 0, 'lava');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(11, 0, 'stilllava');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(12, 0, 'sand');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(13, 0, 'gravel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(14, 0, 'goldore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(15, 0, 'ironore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(16, 0, 'coalore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(17, 0, 'wood');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(17, 1, 'pinewood');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(17, 2, 'birchwood');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(18, 0, 'leaves');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(19, 0, 'sponge');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(20, 0, 'glass');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(21, 0, 'lapisluzuliore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(22, 0, 'lapisluzuliblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(23, 0, 'dispenser');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(24, 0, 'sandstone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(25, 0, 'noteblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(26, 0, 'bedblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(27, 0, 'poweredrail');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(28, 0, 'detectorrail');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(29, 0, 'stickypiston');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(30, 0, 'cobweb');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(31, 0, 'tallgrass');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(32, 0, 'deadshrubs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(33, 0, 'piston');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(34, 0, 'pistonhead');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(34, 1, 'stickypistonhead');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 0, 'wool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 1, 'orangewool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 2, 'magentawool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 3, 'lightbluewool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 4, 'yellowwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 5, 'limewool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 6, 'pinkwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 7, 'graywool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 8, 'lightgraywool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 9, 'cyanwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 10, 'purplewool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 11, 'bluewool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 12, 'brownwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 13, 'greenwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 14, 'redwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(35, 15, 'blackwool');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(36, 0, 'movedblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(37, 0, 'dandelion');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(38, 0, 'rose');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(39, 0, 'brownmushroom');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(40, 0, 'redmushroom');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(41, 0, 'goldblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(42, 0, 'ironblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(43, 0, 'doubleslabs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(44, 0, 'slabs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(45, 0, 'brickblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(46, 0, 'tnt');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(47, 0, 'bookshelf');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(48, 0, 'mossstone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(49, 0, 'obsidian');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(50, 0, 'torch');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(51, 0, 'fire');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(52, 0, 'mobspawner');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(53, 0, 'woodenstiars');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(54, 0, 'chest');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(55, 0, 'redstonewire');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(56, 0, 'diamondore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(57, 0, 'diamondblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(58, 0, 'craftingtable');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(58, 0, 'workbench');");
			//grown wheat??
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(60, 0, 'farmland');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(61, 0, 'furnace');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(62, 0, 'burningfurnace');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(63, 0, 'signpost');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(64, 0, 'wooddoorblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(65, 0, 'ladders');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(66, 0, 'rails');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(67, 0, 'cobblestonestairs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(68, 0, 'wallsign');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(69, 0, 'lever');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(70, 0, 'stonepressureplate' );");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(71, 0, 'irondoorblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(72, 0, 'woodenpressureplate');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(73, 0, 'redstoneore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(74, 0, 'glowingredstoneore');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(75, 0, 'offredstonetorch');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(76, 0, 'redstonetorch');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(77, 0, 'stonebutton');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(78, 0, 'snow');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(79, 0, 'ice');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(80, 0, 'snowblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(81, 0, 'cactus');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(82, 0, 'clayblock');");
			//grown sugar cane??
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(84, 0, 'jukebox');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(85, 0, 'fence');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(86, 0, 'pumpkin');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(87, 0, 'netherrack');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(88, 0, 'soulsand');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(89, 0, 'glowstoneblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(90, 0, 'portalblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(91, 0, 'jackolantern');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(92, 0, 'cakeblock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(93, 0, 'offredstonerepeater');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(94, 0, 'onredstonerepeater');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(95, 0, 'lockedchest');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(96, 0, 'trapdoor');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(97, 0, 'silverfishstone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(98, 0, 'stonebrick');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(98, 1, 'mossystonebrick');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(98, 2, 'crackedstonebrick');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(99, 0, 'hugebrownmushroom');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(100, 0, 'hugeredmushroom');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(101, 0, 'ironbars');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(102, 0, 'glasspane');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(103, 0, 'melon');");
			//melon and pumpkin stem??
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(106, 0, 'vines');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(107, 0, 'fencegate');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(108, 0, 'brickstairs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(109, 0, 'stonebrickstairs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(110, 0, 'mycelium');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(111, 0, 'lillypad');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(112, 0, 'netherbrick');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(113, 0, 'netherbrickfence');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(114, 0, 'netherbrickstairs');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(115, 0, 'netherwart');");
			#endregion
			
			#region ITEMS
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(256, 0, 'ironshovel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(257, 0, 'ironpickaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(258, 0, 'ironaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(259, 0, 'flintandsteel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(260, 0, 'redapple');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(261, 0, 'bow');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(262, 0, 'arrow');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(263, 0, 'coal');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(263, 0, 'charcoal');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(264, 0, 'diamond');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(265, 0, 'ironingot');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(266, 0, 'goldingot');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(267, 0, 'ironsword');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(268, 0, 'woodensword');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(269, 0, 'woodenshovel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(270, 0, 'woodenpickaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(271, 0, 'woodenaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(272, 0, 'stonesword');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(273, 0, 'stoneshovel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(274, 0, 'stonepickaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(275, 0, 'stoneaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(276, 0, 'diamondsword');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(277, 0, 'diamondshovel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(278, 0, 'diamondpickaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(279, 0, 'diamondaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(280, 0, 'stick');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(281, 0, 'bowl');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(282, 0, 'mushroomsoup');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(283, 0, 'goldsword');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(284, 0, 'goldshovel');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(285, 0, 'goldpickaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(286, 0, 'goldaxe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(287, 0, 'string');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(288, 0, 'feather');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(289, 0, 'gunpowder');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(290, 0, 'woodenhoe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(291, 0, 'stonehoe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(292, 0, 'ironhoe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(293, 0, 'diamondhoe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(294, 0, 'goldhoe');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(295, 0, 'seeds');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(296, 0, 'wheat');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(297, 0, 'bread');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(298, 0, 'leathercap');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(299, 0, 'leathertunic');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(300, 0, 'leatherpants');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(301, 0, 'leatherboots');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(302, 0, 'chainhelmet');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(303, 0, 'chainchestplate');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(304, 0, 'chainleggings');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(305, 0, 'chainboots');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(306, 0, 'ironhelmet');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(307, 0, 'ironchestplate');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(308, 0, 'ironleggings');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(309, 0, 'ironboots');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(310, 0, 'diamondhelmet');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(311, 0, 'diamondchestplate');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(312, 0, 'diamondleggings');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(313, 0, 'diamondboots');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(314, 0, 'goldhelmet');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(315, 0, 'goldchestplate');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(316, 0, 'goldleggings');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(317, 0, 'goldboots');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(318, 0, 'flint');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(319, 0, 'rawporkchop');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(320, 0, 'cookedporkchop');");//
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(321, 0, 'painting');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(322, 0, 'goldenapple');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(323, 0, 'sign');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(324, 0, 'wodendoor');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(325, 0, 'bucket');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(326, 0, 'waterbucket');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(327, 0, 'lavabuket');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(328, 0, 'minecart');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(329, 0, 'saddle');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(330, 0, 'irondoor');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(331, 0, 'redstone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(332, 0, 'snowball');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(333, 0, 'boat');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(334, 0, 'leather');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(335, 0, 'milk');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(336, 0, 'claybrick');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(337, 0, 'clay');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(338, 0, 'sugarcane');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(339, 0, 'paper');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(340, 0, 'book');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(341, 0, 'slimeball');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(342, 0, 'chestminecart');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(343, 0, 'furnaceminecart');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(344, 0, 'egg');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(345, 0, 'compass');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(346, 0, 'fishingrod');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(347, 0, 'clock');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(348, 0, 'glowstonedust');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(349, 0, 'rawfish');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(350, 0, 'cookedfish');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 0, 'blackdye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 1, 'reddye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 2, 'greendye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 3, 'browndye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 4, 'bluedye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 5, 'purpledye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 6, 'cyandye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 7, 'lightgraydye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 8, 'graydye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 9, 'pinkdye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 10, 'limedye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 11, 'yellowdye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 12, 'lightbluedye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 13, 'magentadye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 14, 'orangedye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(351, 15, 'whitedye');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(352, 0, 'bone');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(353, 0, 'sugar');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(354, 0, 'cake');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(355, 0, 'bed');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(356, 0, 'redstonerepeater');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(357, 0, 'cookie');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(358, 0, 'map');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(359, 0, 'shears');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(360, 0, 'melonslice');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(361, 0, 'pumpkinseeds');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(362, 0, 'melonseeds');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(363, 0, 'rawsteak');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(364, 0, 'cookedsteak');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(365, 0, 'rawchicken');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(366, 0, 'cookedchicken');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(367, 0, 'rottenflesh');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(368, 0, 'enderpearl');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(369, 0, 'blazerod');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(370, 0, 'ghasttear');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(371, 0, 'goldnugget');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(372, 0, 'netherwart');");				
				
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(2256, 0, 'goldmusicdisc');");
			this.ExecuteNonQuery("INSERT INTO Item(Value, Meta, Alias) VALUES(2557, 0, 'greenmusicdisc');");
			#endregion
		}
		catch(Exception e){Server.Log(e.Message.ToString()); Server.Log(e.StackTrace.ToString());}
		}
		
	}	
}