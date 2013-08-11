XboxLeadersWrapper
==================

.NET wrapper for version 2.0 (using Mashape) of the XboxLeaders.com API.

Video Games Spa example
===

The following is a C# example that will grab all information for a user for generating a Video Game Spa site.

  // See https://www.mashape.com/xboxleaders/xboxleadersapi
	var apiKey = "insert your api key here";
	// Ready the API.
	var api = new XboxLeadersWrapper.XboxApi(apiKey);
	api.Format = XboxLeadersWrapper.ResultFormat.Xml;
	// Create a request to get information for a particular user.
	var dataRequest = new XboxLeadersWrapper.XboxApi.DataRequest() {
		// Id of the user to pull information for.
		Gamertag = "strivinglife",
		// Where to output the XML returned by the API.
		OutputDirectory = @"C:\Users\James\Projects\GitHub\VideoGamesSpa\_output\{0}\xboxleaders\",
		// Format of the actual file; adds .xml to the end.
		FileNameFormat = "{0}",
		// Backup existing files; adds '__' as a prefix and a dash and the current date/time as a prefix.
		BackupFiles = true,
		// Get the user's profile.
		GetProfile = true,
		// Get the user's friends.
		GetFriends = true,
		// Get the listing of all games played by the user.
		GetGames = true,
		// Get information about individual games.
		GetGameAchievementData = true,
		// Get only the last x games.
		GetLastGames = 1
	};
	// Pull the requested data.
	api.PullUserData(dataRequest);

Links
========

Learn about the API: https://www.mashape.com/xboxleaders/XboxLeadersAPI

Learn about Xbox Leaders: https://www.xboxleaders.com/

Learn about me: http://jamesrskemp.com/

View my implementation: http://media.jamesrskemp.com/vg/Default.html
