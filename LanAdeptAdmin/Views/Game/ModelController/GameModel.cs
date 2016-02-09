using System.ComponentModel;
using LanAdeptData.Model.Tournaments;

namespace LanAdeptAdmin.Views.Games.ModelController
{
	public class GameModel
	{
		public int GameID { get; set; }

		[DisplayName("Nom")]
		public string Name { get; set; }

		public GameModel() {

		}

		public GameModel(Game game) {
			GameID = game.GameID;
			Name = game.Name;
		}

	}
}