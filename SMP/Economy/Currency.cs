using System;
using System.Collections.Generic;
namespace SMP
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

