using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxLeadersWrapper
{
	public partial class XboxApi
	{
		/// <summary>
		/// Get the profile for an individual user.
		/// </summary>
		/// <param name="gamertag">Gamertag of the user to query.</param>
		/// <returns>String, of whichever format, for the user.</returns>
		public string GetProfile(string gamertag)
		{
			var parameters = string.Format("gamertag={0}&region={1}", gamertag, this.Region);
			return ApiRequest("profile", parameters);
		}
	}
}
