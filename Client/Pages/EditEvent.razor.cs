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
    public partial class EditEvent
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
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _event = await EventManagerDbService.GetEventById(id:Id);
            var result = await EventManagerDbService.GetAttendees();
            _attendees = result.Value.ToList();
            var eventAttendees = await GetEventAttendee(_event.Id);
            values = eventAttendees.Select(x => x.Id.ToString()).ToList();
        }
        protected bool errorVisible;
        protected EventManager.Server.Models.EventManagerDb.Event _event;
        protected List<EventManager.Server.Models.EventManagerDb.Attendee> _attendees;

        IEnumerable<string> values = new List<string>();
        RadzenDropDownDataGrid<IEnumerable<string>> grid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await EventManagerDbService.UpdateEvent(id:Id, _event);

                List<EventAttendee> eventAttendees = new();

                values.ToList().ForEach(x =>
                {
                    EventAttendee eventAttendee = new()
                    {
                        Event_id = Id,
                        Attendee_id = int.Parse(x)
                    };
                    eventAttendees.Add(eventAttendee);
                });
                if(eventAttendees.Count == 0)
                {

                    await EventManagerDbService.DeleteEventAttendeeByEventId(Id);
                }
                else
                {
                    var response = await EventManagerDbService.UpdateEventAttendee(eventAttendees.AsQueryable());
                }
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"Event is updated" });
                DialogService.Close(_event);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task<List<Attendee>> GetEventAttendee(int id)
        {
            return await EventManagerDbService.GetAttendeesByEvent(id);
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}