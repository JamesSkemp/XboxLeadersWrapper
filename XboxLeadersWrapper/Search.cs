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
		/// Get search results for a particular query.
		/// </summary>
		/// <param name="query">Word or phrase to search the Xbox Marketplace for.</param>
		/// <returns>String, of which format, with the search results.</returns>
		public string GetSearch(string query)
		{
			var parameters = string.Format("query={0}&region={1}", query, this.Region);
			return ApiRequest("search", parameters);
		}
	}
}
