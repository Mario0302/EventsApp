using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Eventures.App.Models
{
    public class OrderListingViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string OrderedOn { get; set; }

        public string EventId { get; set; }
        [Display(Name = "Event")]
        public string EventName { get; set; }

        public string EventStart { get; set; }

        public string EventEnd { get; set; }

        public string EventPlace { get; set; }

        public string CustomerId { get; set; }
        [Display(Name = "Customer")]
        public string CustomerUsername { get; set; }

        public int TicketsCount { get; set; }
    }
}
