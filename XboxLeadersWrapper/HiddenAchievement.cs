using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XboxLeadersWrapper
{
	public partial class XboxApi
	{
		/// <summary>
		/// Full path that contains XML files with data about hidden achievements.
		/// </summary>
		public string HiddenAchievementsXmlPath { get; set; }
		/// <summary>
		/// Namespace for the XML files.
		/// </summary>
		public XNamespace HiddenAchievementsNamespace { get; set; }

		partial void ConstructorExtensions()
		{
			this.HiddenAchievementsNamespace = "http://media.jamesrskemp.com/ns/XblAchievements/201307";
		}

		/// <summary>
		/// A hidden, or secret, achievement for an Xbox Live game.
		/// </summary>
		public class HiddenAchievement
		{
			/// <summary>
			/// Id of the achievement.
			/// </summary>
			public string Id { get; set; }
			/// <summary>
			/// Id of the game the achievement is from.
			/// </summary>
			public string GameId { get; set; }
			/// <summary>
			/// Title of the achievement.
			/// </summary>
			public string Title { get; set; }
			/// <summary>
			/// Title of the game the achievement is from.
			/// </summary>
			public string GameTitle { get; set; }
			/// <summary>
			/// URL of the unlocked tile image.
			/// </summary>
			public string Image { get; set; }
			/// <summary>
			/// Description of the achievement.
			/// </summary>
			public string Description { get; set; }

			public HiddenAchievement()
			{
			}
		}

		/// <summary>
		/// If an XML path has been populated, generates a list of hidden achievements. Excludes any files that start with an underscore (_).
		/// </summary>
		public List<HiddenAchievement> GetHiddenAchievementData()
		{
			XNamespace ns = this.HiddenAchievementsNamespace;

			var hiddenAchievements = new List<HiddenAchievement>();
			if (!string.IsNullOrWhiteSpace(this.HiddenAchievementsXmlPath) && Directory.Exists(this.HiddenAchievementsXmlPath))
			{
				var xmlDirectory = new DirectoryInfo(this.HiddenAchievementsXmlPath);
				var xmlFiles = xmlDirectory.GetFiles("*.xml").Where(f => !f.Name.StartsWith("_"));
				foreach (var xmlFile in xmlFiles)
				{
					var xml = XDocument.Load(xmlFile.FullName);
					var rootXml = xml.Element(ns + "XblAchievements");
					var gameId = rootXml.Element(ns + "Game").Attribute("id").Value;
					var gameTitle = rootXml.Element(ns + "Game").Element(ns + "Title").Value;
					var achievements = rootXml.Elements(ns + "Achievement");
					if (achievements.Count() > 0)
					{
						foreach (var achievement in achievements)
						{
							var hiddenAchievement = new HiddenAchievement();
							hiddenAchievement.GameId = gameId;
							hiddenAchievement.GameTitle = gameTitle;
							hiddenAchievement.Id = achievement.Attribute("id").Value;
							hiddenAchievement.Title = achievement.Element(ns + "Title").Value;
							hiddenAchievement.Image = achievement.Element(ns + "Image").Value;
							hiddenAchievement.Description = achievement.Element(ns + "Description").Value;
							hiddenAchievements.Add(hiddenAchievement);
						}
					}
				}
			}
			return hiddenAchievements;
		}
	}
}
