using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevDay2016SmartGallery.DAL
{
    public class PersonRepository
    {
        private AppDbContext _context;

        public PersonRepository(AppDbContext context){
            _context = context;
        }
    }
}