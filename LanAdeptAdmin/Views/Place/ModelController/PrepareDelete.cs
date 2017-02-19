using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Views.Place.ModelController
{
    public class PrepareDelete
    {
        [Required]
        [Display(Name ="Nombre de table de 24 places")]
        public int NumberOf24SeatsRow { get; set; }
        [Required]
        [Display(Name = "Nombre de table de 9 places")]
        public int NumberOf9SeatsRow { get; set; }
        [Required]
        [Display(Name = "Nombre de table de 24 places supplementaire")]
        public int NumberOf24SeatsSpearTable { get; set; }
        [Required]
        [Display(Name = "Nombre de table de spécial")]
        public int NumberOfSpecialRowSeatsTable { get; set; }
        [Required]
        [Display(Name = "Nombre de place sur les tables spécial")]
        public int NumberOfSpecialRowSeats { get; set; }
    }
}