using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using EventManager.Server.Models.EventManagerDb;

namespace EventManager.Client.Pages
{
    public partial class Attendees
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

        [Parameter]
        public List<Attendee>? EventAttendees { get; set; } = null;

        [Parameter]
        public int? EventId { get; set; } = null;

        protected IEnumerable<EventManager.Server.Models.EventManagerDb.Attendee> attendees;

        protected RadzenDataGrid<EventManager.Server.Models.EventManagerDb.Attendee> grid0;
        protected int count;

        protected async Task Grid0LoadData(LoadDataArgs args)
        {
            try
            {
                grid0.IsLoading = true;
                var result = await EventManagerDbService.GetAttendees(filter: $"{args.Filter}", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                attendees = result.Value.AsODataEnumerable();
                if(EventAttendees != null)
                {
                    attendees = EventAttendees;
                }
                grid0.IsLoading = false;
                StateHasChanged();
                count = result.Count;
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Attendees" });
            }
        }    

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            if(EventAttendees == null)
            {
                await DialogService.OpenAsync<AddAttendee>("Add Attendee", null);
            }
            else
            {
                await DialogService.OpenAsync<AddAttendee>("Add Attendee", new Dictionary<string, object> { { "Event", true }, { "EventId", EventId } });
            }
            await grid0.Reload();
        }

        protected async Task EditRow(EventManager.Server.Models.EventManagerDb.Attendee args)
        {
            await DialogService.OpenAsync<EditAttendee>("Edit Attendee", new Dictionary<string, object> { {"Id", args.Id} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, EventManager.Server.Models.EventManagerDb.Attendee attendee)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await EventManagerDbService.DeleteAttendee(id:attendee.Id);

                    if (deleteResult != null)
                    {
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
                    Detail = $"Unable to delete Attendee" 
                });
            }
        }
    }
}