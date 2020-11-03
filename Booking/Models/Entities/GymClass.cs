﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Models.Entities
{
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime => StartDate + Duration;
        public string Description { get; set; }

        //Navigation property
        public ICollection<ApplicationUserGymClass> AttendedMembers { get; set; }
    }
}
