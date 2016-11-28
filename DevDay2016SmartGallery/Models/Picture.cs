using System;
using System.Collections.Generic;

namespace DevDay2016SmartGallery.Models
{
    public class Picture
    {
        public int PictureId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool FaceAnalysed { get; set; }
        public bool PictureAnalysed { get; set; }

        // Navigation properties
        public virtual List<Face> Faces { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
}
