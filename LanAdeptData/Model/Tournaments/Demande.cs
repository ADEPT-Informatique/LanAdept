namespace LanAdeptData.Model
{
    public class Demande
    {
        public int DemandeID { get; set; }

        #region Navigation properties

        public virtual GamerTag GamerTag { get; set; }

        public virtual Team Team { get; set; }

        #endregion
    }
}
