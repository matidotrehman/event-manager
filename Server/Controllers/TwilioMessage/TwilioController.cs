﻿using DocumentFormat.OpenXml.Office2016.Excel;
using EventManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.SignalR;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;

namespace EventManager.Server.Controllers.TwilioMessage
{
    public class TwilioController : ODataController
    {
        private EventManager.Server.Data.EventManagerDbContext context;

        public TwilioController(Data.EventManagerDbContext context)
        {
            this.context = context;
        }

        [HttpPost("receive-sms")]
        public async Task<IActionResult> ReceiveSms()
        {
            try
            {
                var requestBody = Request.Form["Body"].ToString();
                var senderPhoneNumber = Request.Form["From"].ToString();

                var attendee = this.context.Attendees.FirstOrDefault(x => x.Number == senderPhoneNumber);
                if (attendee != null)
                {
                    var response = new MessagingResponse();


                    var eventAttendee = this.context.EventAttendees.OrderBy(x => x.Event_attendee_id).LastOrDefault(x => x.Attendee_id == attendee.Id);

                    if (!eventAttendee.Response_Received)
                    {
                        var evnt = this.context.Events.FirstOrDefault(x => x.Id == eventAttendee.Event_id);
                        string lowercaseResponse = requestBody.ToLower();

                        if (lowercaseResponse.Contains("yes") || lowercaseResponse == "y")
                        {
                            evnt.Attending += 1;
                            eventAttendee.User_Response = requestBody;
                            eventAttendee.Status = 1;
                            eventAttendee.Response_Received = true;
                            response.Message("Thank you for your response!");
                        }
                        else if (lowercaseResponse.Contains("no") || lowercaseResponse == "n")
                        {
                            evnt.Declined += 1;
                            eventAttendee.User_Response = requestBody;
                            eventAttendee.Status = 2;
                            eventAttendee.Response_Received = true;
                            response.Message("Thank you for your response!");
                        }
                        else
                        {
                            // If the response doesn't match "Yes" or "No," handle it as needed
                            response.Message("Invalid response. Please reply with either 'Yes' or 'No'.");
                        }

                        this.context.EventAttendees.Update(eventAttendee);
                        this.context.Events.Update(evnt);
                        this.context.SaveChanges();

                        return new ContentResult
                        {
                            Content = response.ToString(),
                            ContentType = "application/xml",
                            StatusCode = 200
                        };
                    }
                }

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
