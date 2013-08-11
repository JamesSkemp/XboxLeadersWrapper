using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XboxLeadersWrapper
{
	public partial class XboxApi
	{
		/// <summary>
		/// Pulls a collection of data for a particular user.
		/// </summary>
		/// <returns>True if the request is successful.</returns>
		public bool PullUserData(DataRequest dataRequest)
		{
			var pauseDelay = 3000;

			if (string.IsNullOrWhiteSpace(dataRequest.OutputDirectory))
			{
				throw new InvalidOperationException("You must provide a path to the directory to output data to.");
			}
			else if (string.IsNullOrWhiteSpace(dataRequest.FileNameFormat))
			{
				throw new InvalidOperationException("You must provide a file name format.");
			}
			else if (string.IsNullOrWhiteSpace(dataRequest.Gamertag) || !IsValidGamertag(dataRequest.Gamertag))
			{
				throw new InvalidOperationException("You must provide the valid Xbox Live gamertag of the user to get data for.");
			}

			var outputDirectory = string.Format(dataRequest.OutputDirectory, dataRequest.Gamertag);
			var filePath = Path.Combine(outputDirectory, dataRequest.FileNameFormat + ".xml");
			if (!Directory.Exists(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}

			var currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");

			if (dataRequest.GetProfile)
			{
				var profilePath = string.Format(filePath, "profile");
				dataRequest.PerformBackup("profile", "__{0}-" + currentTime);
				XDocument.Parse(this.GetProfile(dataRequest.Gamertag)).Save(profilePath);
				Thread.Sleep(pauseDelay);
			}
			var gamesPath = string.Format(filePath, "games");
			if (dataRequest.GetGames)
			{
				dataRequest.PerformBackup("games", "__{0}-" + currentTime);
				XDocument.Parse(this.GetGames(dataRequest.Gamertag)).Save(gamesPath);
				Thread.Sleep(pauseDelay);
			}
			var friendsPath = string.Format(filePath, "friends");
			if (dataRequest.GetFriends)
			{
				dataRequest.PerformBackup("friends", "__{0}-" + currentTime);
				XDocument.Parse(this.GetFriends(dataRequest.Gamertag)).Save(friendsPath);
				Thread.Sleep(pauseDelay);
			}
			if (dataRequest.GetGameAchievementData)
			{
				if (!File.Exists(gamesPath))
				{
					throw new NotImplementedException("Games data does not exist. Please GetGames first.");
				}
				if (dataRequest.GameIds != null && dataRequest.GameIds.Count > 0)
				{
					// Requesting very specific games.
					foreach (var gameId in dataRequest.GameIds)
					{
						var gamePath = string.Format(filePath, "achievements-" + gameId);
						dataRequest.PerformBackup("achievements-" + gameId, "__{0}-" + currentTime);
						XDocument.Parse(this.GetAchievements(dataRequest.Gamertag, gameId)).Save(gamePath);
						Thread.Sleep(pauseDelay);
					}
				}
				else
				{
					// We need to get what games the user has played and then get data for them.
					var playedGames = XDocument.Load(gamesPath).Root.Descendants("game");
					if (dataRequest.GetLastGames.HasValue && dataRequest.GetLastGames.Value > 0)
					{
						// Limit to only the last x games.
						playedGames = playedGames.Take(dataRequest.GetLastGames.Value);
					}
					foreach (var playedGame in playedGames)
					{
						var gameId = playedGame.Element("id").Value;
						var gamePath = string.Format(filePath, "achievements-" + gameId);
						dataRequest.PerformBackup("achievements-" + gameId, "__{0}-" + currentTime);
						XDocument.Parse(this.GetAchievements(dataRequest.Gamertag, gameId)).Save(gamePath);
						Thread.Sleep(pauseDelay);
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Data request options for pulling a user's data.
		/// </summary>
		public class DataRequest
		{
			/// <summary>
			/// Gamertag of the user to get data for.
			/// </summary>
			public string Gamertag { get; set; }
			/// <summary>
			/// Whether to get profile data for the user.
			/// </summary>
			public bool GetProfile { get; set; }
			/// <summary>
			/// Whether to get a listing of the user's games.
			/// </summary>
			public bool GetGames { get; set; }
			/// <summary>
			/// Whether to get individual game data for the user.
			/// </summary>
			public bool GetGameAchievementData { get; set; }
			/// <summary>
			/// List of game ids to pull for the user. All if empty.
			/// </summary>
			public List<string> GameIds { get; set; }
			/// <summary>
			/// Get the last x games if defined, otherwise pulls all.
			/// </summary>
			public int? GetLastGames { get; set; }
			/// <summary>
			/// Whether to get the list of friends for the user.
			/// </summary>
			public bool GetFriends { get; set; }
			/// <summary>
			/// Directory to output data to. Will replace {0} with the user's id, if provided.
			/// </summary>
			public string OutputDirectory { get; set; }
			/// <summary>
			/// File name format to use for the files generated. Defaults to xboxleaders-{0}.
			/// </summary>
			public string FileNameFormat { get; set; }
			/// <summary>
			/// Whether to backup any existing files before replacing them.
			/// </summary>
			public bool BackupFiles { get; set; }

			public DataRequest()
			{
				this.FileNameFormat = "xboxleaders-{0}";
			}

			/// <summary>
			/// Pull all profile/game/friend data for a user, but only get updated information for a certain number of games.
			/// </summary>
			/// <param name="gamesToPull">Number of recent games to pull.</param>
			public DataRequest(int gamesToPull)
				: this()
			{
				this.GetProfile = true;
				this.GetGames = true;
				this.GetFriends = true;
				this.GetLastGames = gamesToPull;
				this.BackupFiles = true;
			}

			/// <summary>
			/// Checks whether a backup needs to be performed, and backups the file if it does.
			/// </summary>
			/// <param name="fileName">Name of the file to backup, without the directory and extension.</param>
			/// <param name="backupFileFormat">String format to use when making the backup copy. Extension automatically appended.</param>
			/// <returns>True if the file was backed up.</returns>
			internal bool PerformBackup(string fileName, string backupFileFormat = "__{0}")
			{
				if (this.BackupFiles)
				{
					// Main directory for the user.
					var outputDirectory = string.Format(this.OutputDirectory, this.Gamertag);
					// Format of the full file path.
					var filePathFormat = Path.Combine(outputDirectory, this.FileNameFormat + ".xml");
					// Actual path for this particular file.
					var filePath = string.Format(filePathFormat, fileName);
					if (File.Exists(filePath))
					{
						File.Copy(filePath, Path.Combine(outputDirectory, string.Format(backupFileFormat, string.Format(this.FileNameFormat, fileName)) + ".xml"));
					}
					return true;
				}
				return false;
			}
		}
	}
}