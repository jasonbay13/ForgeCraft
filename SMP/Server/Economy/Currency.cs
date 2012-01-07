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
	public class Currency
	{
		public string Name;
		public string Major;
		public string SingleMajor;
		public string Minor;
		public string SingleMinor;
		
		public static List<Currency> Currencies = new List<Currency>();
		
		public Currency (string name, string major, string smajor, string minor, string sminor)
		{
			this.Name = name;
			this.Major = major;
			this.SingleMajor = smajor;
			this.Minor = minor;
			this.SingleMinor = sminor;
			
			Currencies.Add(this);
		}
		
		public static Currency FindCurrency(string name)
		{
			foreach(Currency c in Currencies)
			{
				if (c.Name == name)
					return c;
			}
			return null;
		}
		
	}
}

