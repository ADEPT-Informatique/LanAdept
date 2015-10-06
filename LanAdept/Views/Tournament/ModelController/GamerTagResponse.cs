using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournament.ModelController
{
    public class GamerTagResponse
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public int GamerTagID { get; set; }

        public string Gamertag { get; set; }
    }
}