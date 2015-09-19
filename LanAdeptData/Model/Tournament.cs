using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Tournament
	{
		public int TournamentID { get; set; }

		[DataType(DataType.DateTime)]
		[Display(Name = "Heure")]
		[DisplayFormat(DataFormatString = @"{0:HH\hmm}")]
		public DateTime? StartTime { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Date")]
		[DisplayFormat(DataFormatString = "{0:D}")]
		public DateTime? StartDate { get; set; }

		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:D}")]
		public DateTime CreationDate { get; set; }

        public virtual Game Game { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}
