using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventuresApp.Models
{
    public class OrderListingViewModel
    {
        public string Id { get; set; }

        public string OrderedOn { get; set; }

        public string EventId { get; set; }

        public string EventName { get; set; }
        
        public string EventStart { get; set; }

        public string EventEnd { get; set; }

        public string EventPlace { get; set; }

        public string CustomerId { get; set; }

        public string CustomerUsername { get; set; }

        public int TicketsCount { get; set; }
        [Authorize(Roles="Administator")]
        public IActionResult Index()
        {
            List<OrderListingViewModel> orders = this.context.Orders.Select(eventFromDb => new OrderListingViewModel
            {
                EventName = eventFromDb.Event.Name,
                CustomerUsername = eventFromDb.Customer.UserName,
                TicketsCount = eventFromDb.TicketsCount,
                OrderedOn = eventFromDb.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
            })
                .ToList();
            return this.View(orders);
            }
        }
    }

