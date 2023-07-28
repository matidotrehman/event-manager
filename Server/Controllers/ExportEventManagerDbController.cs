using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using EventManager.Server.Data;

namespace EventManager.Server.Controllers
{
    public partial class ExportEventManagerDbController : ExportController
    {
        private readonly EventManagerDbContext context;
        private readonly EventManagerDbService service;

        public ExportEventManagerDbController(EventManagerDbContext context, EventManagerDbService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/EventManagerDb/attendees/csv")]
        [HttpGet("/export/EventManagerDb/attendees/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAttendeesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAttendees(), Request.Query), fileName);
        }

        [HttpGet("/export/EventManagerDb/attendees/excel")]
        [HttpGet("/export/EventManagerDb/attendees/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAttendeesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAttendees(), Request.Query), fileName);
        }

        [HttpGet("/export/EventManagerDb/events/csv")]
        [HttpGet("/export/EventManagerDb/events/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEventsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEvents(), Request.Query), fileName);
        }

        [HttpGet("/export/EventManagerDb/events/excel")]
        [HttpGet("/export/EventManagerDb/events/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEventsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEvents(), Request.Query), fileName);
        }

        [HttpGet("/export/EventManagerDb/eventattendees/csv")]
        [HttpGet("/export/EventManagerDb/eventattendees/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEventAttendeesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEventAttendees(), Request.Query), fileName);
        }

        [HttpGet("/export/EventManagerDb/eventattendees/excel")]
        [HttpGet("/export/EventManagerDb/eventattendees/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEventAttendeesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEventAttendees(), Request.Query), fileName);
        }
    }
}
