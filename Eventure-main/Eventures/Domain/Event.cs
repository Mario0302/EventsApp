using Eventures.App.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventures.Domain
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Place { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int TotalTickets { get; set; }

        [Column(TypeName = "decimal(12,3)")]

        public decimal PricePerTicket { get; set; }

        public EventuresUser Owner { get; set; }

        public string OwnerId { get; set; }

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
