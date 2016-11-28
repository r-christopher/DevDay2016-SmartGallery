using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevDay2016SmartGallery.DAL
{
    public class TagRepository
    {
        private AppDbContext _context;
        public TagRepository(AppDbContext context){
            _context = context;
        }
    }
}