using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdept.Models
{
    public class ListeModel
    {
        public Setting Settings { get; set; }
        public int NbPlacesLibres { get; set; }
        public string activeUser { get; set; }
        public List<string> infoSeats { get; set; }
    }
}