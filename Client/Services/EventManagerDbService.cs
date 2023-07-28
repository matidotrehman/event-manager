
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;
using EventManager.Server.Models.EventManagerDb;

namespace EventManager.Client
{
    public partial class EventManagerDbService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public EventManagerDbService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/EventManagerDb/");
        }


        public async System.Threading.Tasks.Task ExportAttendeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/attendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/attendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAttendeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/attendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/attendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAttendees(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.Attendee>> GetAttendees(Query query)
        {
            return await GetAttendees(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.Attendee>> GetAttendees(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Attendees");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAttendees(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.Attendee>>(response);
        }

        partial void OnCreateAttendee(HttpRequestMessage requestMessage);

        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> CreateAttendee(EventManager.Server.Models.EventManagerDb.Attendee attendee = default(EventManager.Server.Models.EventManagerDb.Attendee))
        {
            var uri = new Uri(baseUri, $"Attendees");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(attendee), Encoding.UTF8, "application/json");

            OnCreateAttendee(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<EventManager.Server.Models.EventManagerDb.Attendee>(response);
        }

        partial void OnDeleteAttendee(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAttendee(int id = default(int))
        {
            var uri = new Uri(baseUri, $"Attendees({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAttendee(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAttendeeById(HttpRequestMessage requestMessage);

        public async Task<EventManager.Server.Models.EventManagerDb.Attendee> GetAttendeeById(string expand = default(string), int id = default(int))
        {
            var uri = new Uri(baseUri, $"Attendees({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAttendeeById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<EventManager.Server.Models.EventManagerDb.Attendee>(response);
        }

        partial void OnUpdateAttendee(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAttendee(int id = default(int), EventManager.Server.Models.EventManagerDb.Attendee attendee = default(EventManager.Server.Models.EventManagerDb.Attendee))
        {
            var uri = new Uri(baseUri, $"Attendees({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(attendee), Encoding.UTF8, "application/json");

            OnUpdateAttendee(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportEventsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportEventsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetEvents(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.Event>> GetEvents(Query query)
        {
            return await GetEvents(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.Event>> GetEvents(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Events");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby + " desc", expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetEvents(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.Event>>(response);
        }

        partial void OnCreateEvent(HttpRequestMessage requestMessage);

        public async Task<EventManager.Server.Models.EventManagerDb.Event> CreateEvent(EventManager.Server.Models.EventManagerDb.Event _event = default(EventManager.Server.Models.EventManagerDb.Event))
        {
            var uri = new Uri(baseUri, $"Events");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(_event), Encoding.UTF8, "application/json");

            OnCreateEvent(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<EventManager.Server.Models.EventManagerDb.Event>(response);
        }

        partial void OnDeleteEvent(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteEvent(int id = default(int))
        {
            var uri = new Uri(baseUri, $"Events({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteEvent(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetEventById(HttpRequestMessage requestMessage);

        public async Task<EventManager.Server.Models.EventManagerDb.Event> GetEventById(string expand = default(string), int id = default(int))
        {
            var uri = new Uri(baseUri, $"Events({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetEventById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<EventManager.Server.Models.EventManagerDb.Event>(response);
        }

        partial void OnUpdateEvent(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateEvent(int id = default(int), EventManager.Server.Models.EventManagerDb.Event _event = default(EventManager.Server.Models.EventManagerDb.Event))
        {
            var uri = new Uri(baseUri, $"Events({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(_event), Encoding.UTF8, "application/json");

            OnUpdateEvent(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportEventAttendeesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/eventattendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/eventattendees/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportEventAttendeesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/eventmanagerdb/eventattendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/eventmanagerdb/eventattendees/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetEventAttendees(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.EventAttendee>> GetEventAttendees(Query query)
        {
            return await GetEventAttendees(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.EventAttendee>> GetEventAttendees(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"EventAttendees");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetEventAttendees(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<EventManager.Server.Models.EventManagerDb.EventAttendee>>(response);
        }

        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> CreateEventAttendee(EventManager.Server.Models.EventManagerDb.EventAttendee eventAttendee = default(EventManager.Server.Models.EventManagerDb.EventAttendee))
        {
            var uri = new Uri(baseUri, $"EventAttendees");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(eventAttendee), Encoding.UTF8, "application/json");

            OnCreateEventAttendee(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<EventManager.Server.Models.EventManagerDb.EventAttendee>(response);
        }

        partial void OnCreateEventAttendee(HttpRequestMessage requestMessage);

        public async Task<List<EventManager.Server.Models.EventManagerDb.EventAttendee>> CreateEventAttendees(IQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee> eventAttendees)
        {
            var uri = new Uri(baseUri, $"EventAttendees/AddList");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(eventAttendees), Encoding.UTF8, "application/json");

            OnCreateEventAttendee(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<List<EventManager.Server.Models.EventManagerDb.EventAttendee>>(response);
        }

        partial void OnDeleteEventAttendee(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteEventAttendee(int eventAttendeeId = default(int))
        {
            var uri = new Uri(baseUri, $"EventAttendees({eventAttendeeId})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteEventAttendee(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetEventAttendeeByEventAttendeeId(HttpRequestMessage requestMessage);

        public async Task<EventManager.Server.Models.EventManagerDb.EventAttendee> GetEventAttendeeByEventAttendeeId(string expand = default(string), int eventAttendeeId = default(int))
        {
            var uri = new Uri(baseUri, $"EventAttendees({eventAttendeeId})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetEventAttendeeByEventAttendeeId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<EventManager.Server.Models.EventManagerDb.EventAttendee>(response);
        }

        partial void OnUpdateEventAttendee(HttpRequestMessage requestMessage);

        public async Task<List<EventManager.Server.Models.EventManagerDb.EventAttendee>> UpdateEventAttendee(IQueryable<EventManager.Server.Models.EventManagerDb.EventAttendee> eventAttendees)
        {
            var uri = new Uri(baseUri, $"EventAttendees/UpdateList");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(eventAttendees), Encoding.UTF8, "application/json");

            OnCreateEventAttendee(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<List<EventManager.Server.Models.EventManagerDb.EventAttendee>>(response);
        }

        public async Task<List<Attendee>> GetAttendeesByEvent(int id)
        {
            var uri = new Uri(baseUri, $"GetEventAttendeeByEventId(Event_id={id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<List<Attendee>>(response);
        }
    }
}