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
    public partial class AddEvent
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

        protected override async Task OnInitializedAsync()
        {
            _event = new EventManager.Server.Models.EventManagerDb.Event();
            var result = await EventManagerDbService.GetAttendees();
            _attendees = result.Value;
        }
        protected bool errorVisible;
        protected EventManager.Server.Models.EventManagerDb.Event _event;
        protected IEnumerable<EventManager.Server.Models.EventManagerDb.Attendee> _attendees = new List<Attendee>();
        IEnumerable<string> values = new List<string>();
        RadzenDropDownDataGrid<IEnumerable<string>> grid;

        protected async Task FormSubmit()
        {
            try
            {
                List<EventAttendee> eventAttendees = new();
                var result = await EventManagerDbService.CreateEvent(_event);

                values.ToList().ForEach(x =>
                {
                    EventAttendee eventAttendee = new()
                    {
                        Event_id = result.Id,
                        Attendee_id = int.Parse(x)
                    };
                    eventAttendees.Add(eventAttendee);
                });
                var res = await EventManagerDbService.CreateEventAttendees(eventAttendees.AsQueryable());
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Event is created successfuly" });
                DialogService.Close(_event);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}