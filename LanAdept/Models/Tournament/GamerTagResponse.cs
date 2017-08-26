namespace LanAdept.Models
{
    public class GamerTagResponse
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public int GamerTagID { get; set; }

        public string Gamertag { get; set; }
    }
}