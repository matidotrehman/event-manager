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
using EventManager.Client.Enums;

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
        protected IEnumerable<EventAttendee> eventAtt;

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
                    var resp = await EventManagerDbService.GetEventAttendees();
                    eventAtt = resp.Value.ToList();
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

        public string GetStatusString(int attendeeId)
        {
            var statusId = this.eventAtt.FirstOrDefault(x => x.Event_id == EventId && x.Attendee_id == attendeeId).Status;
            if (Enum.IsDefined(typeof(Statuses), statusId))
            {
                var enumNumber = (Statuses)statusId;
                return enumNumber.ToString();
            }

            return string.Empty;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, EventManager.Server.Models.EventManagerDb.Attendee attendee)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    HttpResponseMessage deleteResult = null;
                    if (EventAttendees == null)
                    {
                        deleteResult = await EventManagerDbService.DeleteAttendee(id: attendee.Id);
                    }
                    else
                    {
                        attendees.ToList().Remove(attendee);
                    }

                    if (deleteResult != null)
                    {
                        NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Contact deleted successfuly" });
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