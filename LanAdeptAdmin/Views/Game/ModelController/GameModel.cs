
using System.ComponentModel;
namespace LanAdeptAdmin.Views.Game.ModelController
{
	public class GameModel
	{
		public int GameID { get; set; }

		[DisplayName("Nom")]
		public string Name { get; set; }

		public GameModel() {

		}

		public GameModel(LanAdeptData.Model.Game game) {
			GameID = game.GameID;
			Name = game.Name;
		}

	}
}