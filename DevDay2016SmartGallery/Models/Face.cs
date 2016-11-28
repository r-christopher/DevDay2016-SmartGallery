using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevDay2016SmartGallery.Models
{
    public class Face
    {
        public int FaceId { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public FaceAttributes FaceAttributes { get; set; }
        public string Emotion { get; set; }

        // Foreign key
        [ForeignKey("Person")]
        public Guid? PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
