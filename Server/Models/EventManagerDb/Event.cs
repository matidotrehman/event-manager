using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventManager.Server.Models.EventManagerDb
{
    [Table("Event", Schema = "dbo")]
    public partial class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;

        [Required]
        public string Description { get; set; }

        public string Note { get; set; }

        [Required]
        public int Attending { get; set; }

        [Required]
        public int Declined { get; set; }

        [Required]
        public int Maybe { get; set; }
    }
}