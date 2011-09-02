using System;
using System.Collections.Generic;

namespace SMP
{
	public class Account
	{
		public Bank bank;
		public uint AccountID; //this is a global ID and is used for finding it in the DB
		public bool PlayerAccount; //player(true), or a server(false) account
		public string Name; //name on the account
		public float Interest; //as a percent
		public float Balance;
		
		public static List<Account> Accounts = new List<Account> (); //only a list of online player accounts
		
		#region CONSTRUCTORS
		public Account (Bank bank, uint AccountID, bool PlayerAccount, string name, float interest)
		{
			this.bank = bank;
			this.PlayerAccount = PlayerAccount;
			this.Name = name;
			this.Interest = interest;
			
		 	Account tempacc = FindAccount(AccountID);
			
			if (tempacc != null) AccountID = Accounts[Accounts.Count - 1].AccountID; //maybe pull from the db instead
			
			while(tempacc != null)
			{
				AccountID++;
				tempacc = FindAccount(AccountID);		
			}
			
			this.AccountID = AccountID;
			
			Accounts.Add(this);
		}		
		
		public Account (Bank bank, string name)
		{
			uint accountid;
			
			this.bank = bank;
			this.PlayerAccount = true;
			this.Name = name;
			this.Interest = this.bank.DefaultInterest;			
			
			Account tempacc = FindAccount(Accounts[Accounts.Count - 1].AccountID); //TODO: Find the last entry in DB
			accountid = tempacc.AccountID;
			
			while(tempacc != null)
			{
				accountid++;
				tempacc = FindAccount(accountid);		
			}
			
			this.AccountID = accountid;
			
			Accounts.Add(this);
			
			
		}
		#endregion
		
		public static Account FindAccount(string name)
		{
			//TODO: Make sure there aren't other accounts under the same name, and extend to search the db
			foreach (Account acc in Accounts)
			{
				if (acc.Name == name)
					return acc;
			}
			return null;
		}
		
		public static Account FindAccount(uint accountID)
		{
			//TODO: extend to search DB
			foreach (Account acc in Accounts)
			{
				if (acc.AccountID == accountID)
					return acc;
			}
			return null;
		}
		
	}
}