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
    [Route("odata/EventManagerDb/Events")]
    public partial class EventsController : ODataController
    {
        private EventManager.Server.Data.EventManagerDbContext context;

        public EventsController(EventManager.Server.Data.EventManagerDbContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<EventManager.Server.Models.EventManagerDb.Event> GetEvents()
        {
            var items = this.context.Events.AsQueryable<EventManager.Server.Models.EventManagerDb.Event>();
            this.OnEventsRead(ref items);

            return items;
        }

        partial void OnEventsRead(ref IQueryable<EventManager.Server.Models.EventManagerDb.Event> items);

        partial void OnEventGet(ref SingleResult<EventManager.Server.Models.EventManagerDb.Event> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/EventManagerDb/Events(Id={Id})")]
        public SingleResult<EventManager.Server.Models.EventManagerDb.Event> GetEvent(int key)
        {
            var items = this.context.Events.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnEventGet(ref result);

            return result;
        }
        partial void OnEventDeleted(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnAfterEventDeleted(EventManager.Server.Models.EventManagerDb.Event item);

        [HttpDelete("/odata/EventManagerDb/Events(Id={Id})")]
        public IActionResult DeleteEvent(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Events
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnEventDeleted(item);
                this.context.Events.Remove(item);
                this.context.SaveChanges();
                this.OnAfterEventDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnEventUpdated(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnAfterEventUpdated(EventManager.Server.Models.EventManagerDb.Event item);

        [HttpPut("/odata/EventManagerDb/Events(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutEvent(int key, [FromBody]EventManager.Server.Models.EventManagerDb.Event item)
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
                this.OnEventUpdated(item);
                this.context.Events.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Events.Where(i => i.Id == key);
                ;
                this.OnAfterEventUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/EventManagerDb/Events(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchEvent(int key, [FromBody]Delta<EventManager.Server.Models.EventManagerDb.Event> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Events.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnEventUpdated(item);
                this.context.Events.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Events.Where(i => i.Id == key);
                ;
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnEventCreated(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnAfterEventCreated(EventManager.Server.Models.EventManagerDb.Event item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] EventManager.Server.Models.EventManagerDb.Event item)
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

                this.OnEventCreated(item);
                this.context.Events.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Events.Where(i => i.Id == item.Id);

                this.OnAfterEventCreated(item);

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
