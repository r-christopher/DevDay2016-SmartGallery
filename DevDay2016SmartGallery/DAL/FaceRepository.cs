using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DevDay2016SmartGallery.DAL
{
    public class FaceRepository
    {
        private AppDbContext _context;

        public FaceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdatePersonName(int faceId, string name)
        {
            var face = _context.Faces.Find(faceId); 
            if(face != null && face.Person != null)
            {
                face.Person.Name = name;
                _context.Entry(face).State = System.Data.Entity.EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}