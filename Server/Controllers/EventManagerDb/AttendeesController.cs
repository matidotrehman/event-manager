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

namespace EventManager.Server.Controllers.EventManagerDb
{
    [Route("odata/EventManagerDb/Attendees")]
    public partial class AttendeesController : ODataController
    {
        private EventManager.Server.Data.EventManagerDbContext context;

        public AttendeesController(EventManager.Server.Data.EventManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<EventManager.Server.Models.EventManagerDb.Attendee> GetAttendees()
        {
            var items = this.context.Attendees.AsQueryable<EventManager.Server.Models.EventManagerDb.Attendee>();
            this.OnAttendeesRead(ref items);

            return items;
        }

        partial void OnAttendeesRead(ref IQueryable<EventManager.Server.Models.EventManagerDb.Attendee> items);

        partial void OnAttendeeGet(ref SingleResult<EventManager.Server.Models.EventManagerDb.Attendee> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/EventManagerDb/Attendees(Id={Id})")]
        public SingleResult<EventManager.Server.Models.EventManagerDb.Attendee> GetAttendee(int key)
        {
            var items = this.context.Attendees.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnAttendeeGet(ref result);

            return result;
        }
        partial void OnAttendeeDeleted(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnAfterAttendeeDeleted(EventManager.Server.Models.EventManagerDb.Attendee item);

        [HttpDelete("/odata/EventManagerDb/Attendees(Id={Id})")]
        public IActionResult DeleteAttendee(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Attendees
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnAttendeeDeleted(item);
                this.context.Attendees.Remove(item);
                this.context.SaveChanges();
                this.OnAfterAttendeeDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAttendeeUpdated(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnAfterAttendeeUpdated(EventManager.Server.Models.EventManagerDb.Attendee item);

        [HttpPut("/odata/EventManagerDb/Attendees(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutAttendee(int key, [FromBody]EventManager.Server.Models.EventManagerDb.Attendee item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.Id != key))
                {
                    return BadRequest();
                }
                this.OnAttendeeUpdated(item);
                this.context.Attendees.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Attendees.Where(i => i.Id == key);
                ;
                this.OnAfterAttendeeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/EventManagerDb/Attendees(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchAttendee(int key, [FromBody]Delta<EventManager.Server.Models.EventManagerDb.Attendee> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Attendees.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnAttendeeUpdated(item);
                this.context.Attendees.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Attendees.Where(i => i.Id == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAttendeeCreated(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnAfterAttendeeCreated(EventManager.Server.Models.EventManagerDb.Attendee item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] EventManager.Server.Models.EventManagerDb.Attendee item)
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

                var existingAttendee = this.context.Attendees.FirstOrDefault(a => a.Number == item.Number);
                if (existingAttendee != null)
                {
                    // A duplicate number is found, return a specific status code to indicate conflict.
                    return StatusCode(409, "An attendee with the same number already exists.");
                }

                this.OnAttendeeCreated(item);
                this.context.Attendees.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Attendees.Where(i => i.Id == item.Id);

                ;

                this.OnAfterAttendeeCreated(item);

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
    }
}
