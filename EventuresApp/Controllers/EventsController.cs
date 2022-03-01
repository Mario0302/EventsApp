using EventuresApp.Data;
using EventuresApp.Domain;
using EventuresApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace EventuresApp.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext context;
        public EventsController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult All()
        {
            List<EventAllViewModel> events = context.Events.Select(eventFromDb => new EventAllViewModel
            {
                Id = eventFromDb.Id,
                Name = eventFromDb.Name,
                Place = eventFromDb.Place,
                Start = eventFromDb.Start.ToString("dd-MMM-yyyy HH:mm", CultureInfo.InvariantCulture),
                End = eventFromDb.End.ToString("dd-MMM-yyyy HH:mm", CultureInfo.InvariantCulture),
                Owner = eventFromDb.Owner.UserName
            }).ToList();
            return this.View(events);
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
.Where(eventFromDb => eventFromDb.CustomerId == user.Id)
.Select(eventFromDb => new OrderListingViewModel
{
    Id = eventFromDb.Id,
    EventId = eventFromDb.EventId,
    EventName = eventFromDb.Event.Name,
    EventStart = eventFromDb.Event.Start.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
    EventEnd = eventFromDb.Event.End.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
    EventPlace = eventFromDb.Event.Place,
    OrderedOn = eventFromDb.OrderedOn.ToString("dd-mm-yyyy hh:mm", CultureInfo.InvariantCulture),
    CustomerId = eventFromDb.CustomerId,
    CustomerUsername = eventFromDb.Customer.UserName,
    TicketsCount = eventFromDb.TicketsCount
})
.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(eventFromDb => eventFromDb.EventPlace.Contains(searchString)).ToList();
            }
            return this.View(orders);
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
            return this.RedirectToAction("All");
        }
        return this.View();
    }


}
}
    


