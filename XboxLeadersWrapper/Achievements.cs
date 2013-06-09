namespace XboxLeadersWrapper
{
	public partial class XboxApi
	{
		/// <summary>
		/// Get achievements for a particular game for an individual user.
		/// </summary>
		/// <param name="gamertag">Gamertag of the user to query.</param>
		/// <param name="gameId">Id of the game to get achievements for.</param>
		/// <returns>String, of whichever format, for the user.</returns>
		public string GetAchievements(string gamertag, string gameId)
		{
			var parameters = string.Format("gamertag={0}&gameid={1}&region={2}", gamertag, gameId, this.Region);
			return ApiRequest("achievements", parameters);
		}
	}
}
