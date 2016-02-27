using LanAdeptData.Model;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Places.ModelController
{
    public class MaPlaceModel
    {
        public Reservation reservation { get; set; }
        public Setting setting { get; set; } 
    }
}