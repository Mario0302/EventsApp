using Eventures.App.Models;
using Eventures.Data;
using Eventures.Domain;
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
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext context;
        public EventsController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [Authorize]
        public IActionResult My(string searchString)
        {
            string currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = this.context.Users.SingleOrDefault(u => u.Id == currentUserId);
            if (user == null)
            {
                return null;
            }
            List<OrderListingViewModel> orders = this.context.Orders
            .Where(o => o.CustomerId == user.Id)
            .Select(o => new OrderListingViewModel
            {
                Id = o.Id,
                EventId = o.EventId,
                EventName = o.Event.Name,
                EventStart = o.Event.Start.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
                EventEnd = o.Event.End.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
                EventPlace = o.Event.Place,
                OrderedOn = o.OrderedOn.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
                CustomerId = o.CustomerId,
                CustomerUsername = o.Customer.UserName,
                TicketsCount = o.TicketsCount
            })
            .ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.EventPlace.Contains(searchString)).ToList();
            }
            return this.View(orders);
        }
            public IActionResult All(string searchString)
          {
            List<EventAllViewModel> events = context.Events
            .Select(eventFromDb => new EventAllViewModel
            {
              Id = eventFromDb.Id,
              Name = eventFromDb.Name,
              Place = eventFromDb.Place,
              Start = eventFromDb.Start.ToString("dd-MMM-yyyy HH:mm", CultureInfo.InvariantCulture),
              End = eventFromDb.End.ToString("dd-MMM-yyyy HH:mm", CultureInfo.InvariantCulture),
              Owner = eventFromDb.Owner.UserName
            })
            .ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(s => s.Place.Contains(searchString)).ToList();
            }
            return this.View(events);
          }
        public IActionResult Create()
        {
            return this.View();
        }
        [HttpPost]
        public IActionResult Create(EventCreateBindingModel bindingModel)
        {
            if (this.ModelState.IsValid)
            {
                string currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Event eventForDb = new Event
                {
                   Name = bindingModel.Name,
                   Place = bindingModel.Place,
                   Start = bindingModel.Start,
                   End = bindingModel.End,
                   TotalTickets = bindingModel.TotalTickets,
                   PricePerTicket = bindingModel.PricePerTicket,
                   OwnerId = currentUserId
                };
                context.Events.Add(eventForDb);
                context.SaveChanges();
                return this. RedirectToAction("All");
            }
                return this.View();
         }
    public IActionResult Index()
        {
            return View();
        }
    }
}
