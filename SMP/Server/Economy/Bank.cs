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
using System.Collections.Generic;

namespace SMP.ECO
{
	public class Bank
	{
		public string Name;
		public bool Default;
		public Currency currency;
		public float OpenFee;
		public float IntialHolding;
		public int InterestInterval; //in seconds
		public float DefaultInterest;
		public List<Account> Accounts = new List<Account>(); //only holds online accounts
		
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
			}
			return null;
		}
		
		/// <summary>
		/// Finds the account with name in this.Bank
		/// </summary>
		public Account FindAccount(string name)
		{
			//TODO: extend to search DB
			foreach(Account acc in Accounts)
			{
				if (acc.Name == name)
					return acc;
			}
			return null;
		}
	}
}

