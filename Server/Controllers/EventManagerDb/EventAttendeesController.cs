using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DocumentFormat.OpenXml.Spreadsheet;
using EventManager.Server.Models.EventManagerDb;

namespace EventManager.Server.Controllers.EventManagerDb
{
    [Route("odata/EventManagerDb/EventAttendees")]
    public partial class EventAttendeesController : ODataController
    {
        private EventManager.Server.Data.EventManagerDbContext context;

        public EventAttendeesController(EventManager.Server.Data.EventManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<EventManager.Server.Models.EventManagerDb.EventAttendee> GetEventAttendees()
        {
            var items = this.context.EventAttendees.AsQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee>();
            this.OnEventAttendeesRead(ref items);

            return items;
        }

        partial void OnEventAttendeesRead(ref IQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee> items);

        partial void OnEventAttendeeGet(ref SingleResult<EventManager.Server.Models.EventManagerDb.EventAttendee> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/EventManagerDb/EventAttendees(Event_attendee_id={Event_attendee_id})")]
        public SingleResult<EventManager.Server.Models.EventManagerDb.EventAttendee> GetEventAttendee(int key)
        {
            var items = this.context.EventAttendees.Where(i => i.Event_attendee_id == key);
            var result = SingleResult.Create(items);

            OnEventAttendeeGet(ref result);

            return result;
        }
        partial void OnEventAttendeeDeleted(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnAfterEventAttendeeDeleted(EventManager.Server.Models.EventManagerDb.EventAttendee item);

        [HttpDelete("/odata/EventManagerDb/EventAttendees(Event_attendee_id={Event_attendee_id})")]
        public IActionResult DeleteEventAttendee(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.EventAttendees
                    .Where(i => i.Event_attendee_id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnEventAttendeeDeleted(item);
                this.context.EventAttendees.Remove(item);
                this.context.SaveChanges();
                this.OnAfterEventAttendeeDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnEventAttendeeUpdated(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnAfterEventAttendeeUpdated(EventManager.Server.Models.EventManagerDb.EventAttendee item);

        [HttpPut("/odata/EventManagerDb/EventAttendees(Event_attendee_id={Event_attendee_id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutEventAttendee(int key, [FromBody]EventManager.Server.Models.EventManagerDb.EventAttendee item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.Event_attendee_id != key))
                {
                    return BadRequest();
                }
                this.OnEventAttendeeUpdated(item);
                this.context.EventAttendees.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.EventAttendees.Where(i => i.Event_attendee_id == key);
                ;
                this.OnAfterEventAttendeeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/EventManagerDb/EventAttendees(Event_attendee_id={Event_attendee_id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchEventAttendee(int key, [FromBody]Delta<EventManager.Server.Models.EventManagerDb.EventAttendee> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.EventAttendees.Where(i => i.Event_attendee_id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnEventAttendeeUpdated(item);
                this.context.EventAttendees.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.EventAttendees.Where(i => i.Event_attendee_id == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnEventAttendeeCreated(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnAfterEventAttendeeCreated(EventManager.Server.Models.EventManagerDb.EventAttendee item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] EventManager.Server.Models.EventManagerDb.EventAttendee item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnEventAttendeeCreated(item);
                this.context.EventAttendees.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.EventAttendees.Where(i => i.Event_attendee_id == item.Event_attendee_id);

                ;

                this.OnAfterEventAttendeeCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("/odata/EventManagerDb/EventAttendees/AddList")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult AddList([FromBody] List<EventManager.Server.Models.EventManagerDb.EventAttendee> items)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (items == null || items.Count == 0)
                {
                    return BadRequest("The list of EventAttendees is empty.");
                }

                foreach (var item in items)
                {
                    this.OnEventAttendeeCreated(item);
                    this.context.EventAttendees.Add(item);
                }
                this.context.SaveChanges();

                List<int> insertedIds = items.Select(i => i.Event_attendee_id).ToList();

                var itemsToReturn = this.context.EventAttendees.Where(i => insertedIds.Contains(i.Event_attendee_id)).ToList();

                foreach (var item in itemsToReturn)
                {
                    this.OnAfterEventAttendeeCreated(item);
                }

                return new ObjectResult(itemsToReturn)
                {
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("/odata/EventManagerDb/GetEventAttendeeByEventId(Event_id={Event_id})")]
        public List<EventManager.Server.Models.EventManagerDb.Attendee> GetEventAttendeeByEventId(int Event_id)
        {
            var items = this.context.EventAttendees.Where(i => i.Event_id == Event_id).ToList();
            List<Attendee> result = new();
            foreach (var item in items)
            {
                var attendee = this.context.Attendees.FirstOrDefault(a => a.Id == item.Attendee_id);
                result.Add(attendee);
            }

            return result;
        }

        [HttpPost("/odata/EventManagerDb/EventAttendees/UpdateList")]
        public IActionResult UpdateList([FromBody] List<EventManager.Server.Models.EventManagerDb.EventAttendee> items)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (items == null || items.Count == 0)
                {
                    return BadRequest("The list of EventAttendees is empty.");
                }
                var rmItems = this.context.EventAttendees.Where(a => a.Event_id == items.FirstOrDefault().Event_id).ToList();

                // delete previous values first
                this.context.EventAttendees.RemoveRange(rmItems);
                this.context.SaveChanges();

                foreach (var item in items)
                {
                    this.OnEventAttendeeCreated(item);
                    this.context.EventAttendees.Add(item);
                }
                this.context.SaveChanges();

                List<int> insertedIds = items.Select(i => i.Event_attendee_id).ToList();

                var itemsToReturn = this.context.EventAttendees.Where(i => insertedIds.Contains(i.Event_attendee_id)).ToList();

                foreach (var item in itemsToReturn)
                {
                    this.OnAfterEventAttendeeCreated(item);
                }

                return new ObjectResult(itemsToReturn)
                {
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
