using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<GymClassViewModel> GymClasses { get; set; }
    }
}
