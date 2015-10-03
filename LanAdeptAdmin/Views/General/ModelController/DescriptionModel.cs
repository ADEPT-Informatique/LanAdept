using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Views.General.ModelController {
    public class DescriptionModel {
        [DataType(DataType.MultilineText)]
        [DisplayName("Description")]
        public string Description { get; set; }
    }
}