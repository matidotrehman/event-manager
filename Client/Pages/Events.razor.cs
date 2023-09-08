using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace EventManager.Client.Pages
{
    public partial class Events
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public EventManagerDbService EventManagerDbService { get; set; }

        protected IEnumerable<EventManager.Server.Models.EventManagerDb.Event> events;

        protected List<EventManager.Server.Models.EventManagerDb.Attendee> attendee;

        protected RadzenDataGrid<EventManager.Server.Models.EventManagerDb.Event> grid0;
        protected int count;
        protected bool loader = false;

        [Inject]
        protected SecurityService Security { get; set; }


        protected async Task Grid0LoadData(LoadDataArgs args)
        {
            try
            {
                grid0.IsLoading = true;
                var result = await EventManagerDbService.GetEvents(filter: $"{args.Filter}", orderby: $"Date", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                events = result.Value.AsODataEnumerable();
                grid0.IsLoading = false;
                StateHasChanged();
                count = result.Count;
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Events" });
            }
        }    

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddEvent>("Add Event", null);
            await grid0.Reload();
        }

        protected async Task EditRow(EventManager.Server.Models.EventManagerDb.Event args)
        {
            await DialogService.OpenAsync<EditEvent>("Edit Event", new Dictionary<string, object> { {"Id", args.Id} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, EventManager.Server.Models.EventManagerDb.Event _event)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await EventManagerDbService.DeleteEvent(id:_event.Id);

                    if (deleteResult != null)
                    {
                        NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Event is deleted successfuly" });
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                { 
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error", 
                    Detail = $"Unable to delete Event" 
                });
            }
        }

        protected async Task OpenAttendees(int eventId)
        {
            attendee = await EventManagerDbService.GetAttendeesByEvent(eventId);
            await DialogService.OpenAsync<Attendees>("Attendees", new Dictionary<string, object> { { "EventAttendees", attendee }, { "EventId", eventId} }, new DialogOptions { Width = "60%", Height="60%"});
        }
    }
}