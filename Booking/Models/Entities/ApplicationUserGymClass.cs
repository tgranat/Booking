using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Models.Entities
{
    public class ApplicationUserGymClass
    {
        public int GymClassId { get; set; }
        public string ApplicationUserId { get; set; }

        //Navigation property
        public GymClass GymClass { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
