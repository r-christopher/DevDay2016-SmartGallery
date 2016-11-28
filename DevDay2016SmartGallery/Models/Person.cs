using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevDay2016SmartGallery.Models
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PersonId { get; set; }
        public string Name { get; set; } = "Unknown";

        // Navigation properties
        public virtual List<Face> Faces { get; set; }
    }
}
