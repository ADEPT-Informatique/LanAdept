using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Place.ModelController
{
    public class PDFModel
    {
        public Reservation reservation { get; set; }
        public Setting setting { get; set; } 
    }
}