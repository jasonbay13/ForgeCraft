using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
   public partial class Remote
    {
       public bool HandleLogin(string message)
       {
           string[] splitted = message.Split(':');
           if (splitted[0] == "head" && splitted[1] == "lols")
           { username = splitted[0]; password =  return true; }
           else
           return false;
       }
    }
}
