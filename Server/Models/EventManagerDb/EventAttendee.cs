using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventManager.Server.Models.EventManagerDb
{
    [Table("Event_Attendees", Schema = "dbo")]
    public partial class EventAttendee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Event_attendee_id { get; set; }

        [Required]
        public int Event_id { get; set; }

        [Required]
        public int Attendee_id { get; set; }

    }
}