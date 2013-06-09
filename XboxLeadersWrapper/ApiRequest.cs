using System.IO;
using System.Net;
using System.Text;

namespace XboxLeadersWrapper
{
	public partial class XboxApi
	{
		/// <summary>
		/// Helper to make a request to the API.
		/// </summary>
		/// <param name="methodPath">Path to the method to call.</param>
		/// <returns>API response.</returns>
		private string ApiRequest(string methodPath, string parameters)
		{
			var requestUrl = string.Format(XboxApi.apiUrl, this.Version, methodPath, this.Format.ToString().ToLower(), parameters);

			var request = WebRequest.CreateHttp(requestUrl);
			request.Headers.Add(XboxApi.authorizationHeader, this.ApiKey);
			request.Timeout = this.Timeout;

			using (var response = request.GetResponse())
			{
				StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				return reader.ReadToEnd();
			}
		}
	}
}
