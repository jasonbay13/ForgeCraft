using System;
using System.Collections.Generic;

namespace SMP
{
	public class Bank
	{
		public string Name;
		public bool Default;
		//public Currency currency;
		public float OpenFee;
		public float IntialHolding;
		public int InterestInterval; //in secs
		public List<Account> Accounts = new List<Account>();
		
		public static List<Bank> Banks = new List<Bank>();
		public static Bank DefaultBank;
		
		public Bank (string name, bool Default)
		{
			this.Name = name;
			this.Default = Default;
			
			if (Default) DefaultBank = this;
			
			Banks.Add(this);
			
		}
		
		public Bank (string name, bool Default, float openfee, float initialholding, int interestinterval)
		{
			this.Name = name;
			this.Default = Default;
			this.OpenFee = openfee;
			this.IntialHolding = initialholding;
			this.InterestInterval = interestinterval;
			
			if (Default) DefaultBank = this;
			
			Banks.Add(this);
		}
		
		public static Bank FindBank(string name)
		{
			foreach(Bank b in Banks)
			{
				if (b.Name.ToLower() == name.ToLower())	
					return b;
				
				return null;
			}
		}
	}
}

