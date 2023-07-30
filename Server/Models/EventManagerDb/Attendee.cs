using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventManager.Server.Models.EventManagerDb
{
    [Table("Contacts", Schema = "dbo")]
    public partial class Attendee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Number { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string Key { get { return this.Id.ToString(); } }

    }
}