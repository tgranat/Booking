using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Models.ViewModels
{
    public class EditGymClassViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Duration { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
