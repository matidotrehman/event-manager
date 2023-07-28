using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using EventManager.Server.Data;

namespace EventManager.Server
{
    public partial class EventManagerDbService
    {
        EventManagerDbContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly EventManagerDbContext context;
        private readonly NavigationManager navigationManager;

        public EventManagerDbService(EventManagerDbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportAttendeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/attendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/attendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAttendeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/attendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/attendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAttendeesRead(ref IQueryable<EventManager.Server.Models.EventManagerDb.Attendee> items);

        public async Task<IQueryable<EventManager.Server.Models.EventManagerDb.Attendee>> GetAttendees(Query query = null)
        {
            var items = Context.Attendees.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAttendeesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAttendeeGet(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnGetAttendeeById(ref IQueryable<EventManager.Server.Models.EventManagerDb.Attendee> items);


        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> GetAttendeeById(int id)
        {
            var items = Context.Attendees
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAttendeeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAttendeeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAttendeeCreated(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnAfterAttendeeCreated(EventManager.Server.Models.EventManagerDb.Attendee item);

        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> CreateAttendee(EventManager.Server.Models.EventManagerDb.Attendee attendee)
        {
            OnAttendeeCreated(attendee);

            var existingItem = Context.Attendees
                              .Where(i => i.Id == attendee.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Attendees.Add(attendee);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(attendee).State = EntityState.Detached;
                throw;
            }

            OnAfterAttendeeCreated(attendee);

            return attendee;
        }

        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> CancelAttendeeChanges(EventManager.Server.Models.EventManagerDb.Attendee item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAttendeeUpdated(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnAfterAttendeeUpdated(EventManager.Server.Models.EventManagerDb.Attendee item);

        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> UpdateAttendee(int id, EventManager.Server.Models.EventManagerDb.Attendee attendee)
        {
            OnAttendeeUpdated(attendee);

            var itemToUpdate = Context.Attendees
                              .Where(i => i.Id == attendee.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(attendee);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAttendeeUpdated(attendee);

            return attendee;
        }

        partial void OnAttendeeDeleted(EventManager.Server.Models.EventManagerDb.Attendee item);
        partial void OnAfterAttendeeDeleted(EventManager.Server.Models.EventManagerDb.Attendee item);

        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> DeleteAttendee(int id)
        {
            var itemToDelete = Context.Attendees
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAttendeeDeleted(itemToDelete);


            Context.Attendees.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAttendeeDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportEventsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEventsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEventsRead(ref IQueryable<EventManager.Server.Models.EventManagerDb.Event> items);

        public async Task<IQueryable<EventManager.Server.Models.EventManagerDb.Event>> GetEvents(Query query = null)
        {
            var items = Context.Events.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnEventsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEventGet(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnGetEventById(ref IQueryable<EventManager.Server.Models.EventManagerDb.Event> items);


        public async Task<EventManager.Server.Models.EventManagerDb.Event> GetEventById(int id)
        {
            var items = Context.Events
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetEventById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEventGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEventCreated(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnAfterEventCreated(EventManager.Server.Models.EventManagerDb.Event item);

        public async Task<EventManager.Server.Models.EventManagerDb.Event> CreateEvent(EventManager.Server.Models.EventManagerDb.Event _event)
        {
            OnEventCreated(_event);

            var existingItem = Context.Events
                              .Where(i => i.Id == _event.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Events.Add(_event);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(_event).State = EntityState.Detached;
                throw;
            }

            OnAfterEventCreated(_event);

            return _event;
        }

        public async Task<EventManager.Server.Models.EventManagerDb.Event> CancelEventChanges(EventManager.Server.Models.EventManagerDb.Event item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEventUpdated(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnAfterEventUpdated(EventManager.Server.Models.EventManagerDb.Event item);

        public async Task<EventManager.Server.Models.EventManagerDb.Event> UpdateEvent(int id, EventManager.Server.Models.EventManagerDb.Event _event)
        {
            OnEventUpdated(_event);

            var itemToUpdate = Context.Events
                              .Where(i => i.Id == _event.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(_event);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEventUpdated(_event);

            return _event;
        }

        partial void OnEventDeleted(EventManager.Server.Models.EventManagerDb.Event item);
        partial void OnAfterEventDeleted(EventManager.Server.Models.EventManagerDb.Event item);

        public async Task<EventManager.Server.Models.EventManagerDb.Event> DeleteEvent(int id)
        {
            var itemToDelete = Context.Events
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEventDeleted(itemToDelete);


            Context.Events.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEventDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportEventAttendeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/eventattendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/eventattendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEventAttendeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/eventattendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/eventattendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEventAttendeesRead(ref IQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee> items);

        public async Task<IQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee>> GetEventAttendees(Query query = null)
        {
            var items = Context.EventAttendees.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnEventAttendeesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEventAttendeeGet(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnGetEventAttendeeByEventAttendeeId(ref IQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee> items);


        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> GetEventAttendeeByEventAttendeeId(int eventattendeeid)
        {
            var items = Context.EventAttendees
                              .AsNoTracking()
                              .Where(i => i.Event_attendee_id == eventattendeeid);

 
            OnGetEventAttendeeByEventAttendeeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEventAttendeeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEventAttendeeCreated(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnAfterEventAttendeeCreated(EventManager.Server.Models.EventManagerDb.EventAttendee item);

        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> CreateEventAttendee(EventManager.Server.Models.EventManagerDb.EventAttendee eventattendee)
        {
            OnEventAttendeeCreated(eventattendee);

            var existingItem = Context.EventAttendees
                              .Where(i => i.Event_attendee_id == eventattendee.Event_attendee_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.EventAttendees.Add(eventattendee);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(eventattendee).State = EntityState.Detached;
                throw;
            }

            OnAfterEventAttendeeCreated(eventattendee);

            return eventattendee;
        }

        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> CancelEventAttendeeChanges(EventManager.Server.Models.EventManagerDb.EventAttendee item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEventAttendeeUpdated(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnAfterEventAttendeeUpdated(EventManager.Server.Models.EventManagerDb.EventAttendee item);

        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> UpdateEventAttendee(int eventattendeeid, EventManager.Server.Models.EventManagerDb.EventAttendee eventattendee)
        {
            OnEventAttendeeUpdated(eventattendee);

            var itemToUpdate = Context.EventAttendees
                              .Where(i => i.Event_attendee_id == eventattendee.Event_attendee_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(eventattendee);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEventAttendeeUpdated(eventattendee);

            return eventattendee;
        }

        partial void OnEventAttendeeDeleted(EventManager.Server.Models.EventManagerDb.EventAttendee item);
        partial void OnAfterEventAttendeeDeleted(EventManager.Server.Models.EventManagerDb.EventAttendee item);

        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> DeleteEventAttendee(int eventattendeeid)
        {
            var itemToDelete = Context.EventAttendees
                              .Where(i => i.Event_attendee_id == eventattendeeid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEventAttendeeDeleted(itemToDelete);


            Context.EventAttendees.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEventAttendeeDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}