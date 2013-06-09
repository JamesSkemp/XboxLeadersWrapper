namespace XboxLeadersWrapper
{
	public partial class XboxApi
	{
		/// <summary>
		/// Get the friends for an individual user.
		/// </summary>
		/// <param name="gamertag">Gamertag of the user to query.</param>
		/// <returns>String, of whichever format, for the user.</returns>
		public string GetFriends(string gamertag)
		{
			var parameters = string.Format("gamertag={0}&region={1}", gamertag, this.Region);
			return ApiRequest("friends", parameters);
		}
	}
}
