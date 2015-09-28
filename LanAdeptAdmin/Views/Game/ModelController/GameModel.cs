
namespace LanAdeptAdmin.Views.Game.ModelController
{
	public class GameModel
	{
		public int GameID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }


		public GameModel() {

		}

		public GameModel(LanAdeptData.Model.Game game) {
			GameID = game.GameID;
			Name = game.Name;
			Description = game.Description;
		}

	}
}