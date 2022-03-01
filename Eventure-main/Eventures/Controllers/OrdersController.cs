using Eventures.App.Domain;
using Eventures.App.Models;
using Eventures.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eventures.App.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext context;

        public OrdersController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [Authorize(Roles = "Administrator")]

        public IActionResult Index()
        {
            List<OrderListingViewModel> orders = this.context.Orders.Select(o => new OrderListingViewModel
            {
              Id = o.Id,
              // EventId = o. EventId,
              EventName = o.Event.Name,
              // EventStart = o.Event. Start.ToString("dd-mm-yyyy hh:mm", CultureInfo. InvariantCulture),
              // EventEnd = o.Event.End.ToString("dd-mm-yyyy hh:mm", CultureInfo. InvariantCulture),
              // EventPlace = o. Event Place,
              // CustomerId = o. CustomerId,
              CustomerUsername = o.Customer.UserName,
              TicketsCount = o.TicketsCount,
              OrderedOn = o.OrderedOn.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
            })
            .ToList();

            return this.View(orders);
        }

        [HttpPost]
        public IActionResult Create(OrderCreateBindingModel bindingModel)
        {
            if (this.ModelState.IsValid)
            {
                string currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = this.context.Users.SingleOrDefault(u => u.Id == currentUserId);
                var ev = this.context.Events.SingleOrDefault(e => e.Id == bindingModel.EventId);

                if (user == null || ev == null || ev.TotalTickets < bindingModel.TicketsCount)
                {
                    return this.RedirectToAction("All", "Events");
                }
                Order orderForDb = new Order()
                {
                    OrderedOn = DateTime.UtcNow,
                    EventId = bindingModel.EventId,
                    TicketsCount = bindingModel.TicketsCount,
                    CustomerId = currentUserId
                };

                ev.TotalTickets -= bindingModel.TicketsCount;

                this.context.Events.Update(ev);
                this.context.Orders.Add(orderForDb);
                this.context.SaveChanges();
            }
            return this.RedirectToAction("All", "Events");
        }

    }
}
