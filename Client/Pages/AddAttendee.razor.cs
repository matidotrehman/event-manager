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
    public partial class AddAttendee
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
        public bool Event { get; set; } = false;
        [Parameter]
        public int? EventId { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            attendee = new EventManager.Server.Models.EventManagerDb.Attendee();
        }
        protected bool errorVisible;
        protected string errorMessage;
        protected EventManager.Server.Models.EventManagerDb.Attendee attendee;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                var resposne = await EventManagerDbService.CreateAttendee(attendee);
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Success", Detail = $"New contact is added" });
                if (Event)
                {
                    EventAttendee eventAttendee = new()
                    {
                        Event_id = (int)EventId,
                        Attendee_id = resposne.Id
                    };
                    await EventManagerDbService.CreateEventAttendee(eventAttendee);
                }
                DialogService.Close(attendee);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}