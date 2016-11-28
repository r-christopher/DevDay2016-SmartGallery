using DevDay2016SmartGallery.Models;
using System.Collections.Generic;

namespace DevDay2016SmartGallery.DTOs
{
    public class PictureDataDto
    {
        public int PictureId { get; set; }
        public string Description { get; set; }
        public virtual List<string> Tags { get; set; }
    }
}