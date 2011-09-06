using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace SMP
{
	class Database
	{
		public static bool started = false;
		public static SQLiteConnection con;

		public static void Init()
		{
			con = new SQLiteConnection("Data Source=data.sdb");
			con.Open();

			started = true;
		}
		public static List<string[]> get(string query)
		{
			int count = 0;
			List<string[]> output = new List<string[]>();
			SQLiteCommand cmd = new SQLiteCommand(query, con);
			SQLiteDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				if (dr.FieldCount >= 1)
				{
					string[] s = new string[dr.FieldCount];
					for (int i = 0; i < dr.FieldCount; i++)
					{
						s[i] = dr.GetValue(i).ToString();
					}
					output.Add(s);
					count++;
				}
			}
			if (count >= 1)
				return output;
			else
				return null;
		}

		public static void donow(string command)
		{
			SQLiteCommand comm = new SQLiteCommand();
			comm.Connection = con;
			comm.CommandText = command;

			try
			{
				comm.ExecuteNonQuery();
			}
			catch (SQLiteException exception)
			{
				Server.Log("Failed :" + exception.Message);
			}
			catch (Exception e)
			{
				Server.Log("Failed :" + e.Message);
			}
		}

		public static void E(int e)
		{
			//Add case for Application.exit crashing
		}

		public static void InitialSetup()
		{
			//Add Database Initialization
		}
		public static void update(bool all, string current)
		{
			//Add Database Updating Stuffs
		}
	}
}
