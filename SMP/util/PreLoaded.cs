using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP.PreLoadedData
{
	class PreLoaded
	{
		//Pre loaded data holders go here, data from the database gets pulled, saved here and the database gets updated when we need to do so.
		//
		//This allows for a faster server, and since we use sqlite and not SQL, we dont have to worry as much about stuff needing to be updated (repulled) from the database.
	}
}
