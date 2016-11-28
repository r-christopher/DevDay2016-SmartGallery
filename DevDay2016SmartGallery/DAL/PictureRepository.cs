using DevDay2016SmartGallery.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using VisionContract = Microsoft.ProjectOxford.Vision.Contract;
using FaceContract = Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Emotion.Contract;
using DevDay2016SmartGallery.Extensions;
using System.Collections.Generic;

namespace DevDay2016SmartGallery.DAL
{
    public class PictureRepository
    {
        private AppDbContext _context; 

        public PictureRepository(AppDbContext context){
            _context = context; 
        }

        public Picture GetPictureById(int pictureId){
            return _context.Pictures.Find(pictureId);
        }

        public async Task<Picture> AddPicture(string name, string url){
            var picture = new Picture { Date = DateTime.Now, Name = name, PictureUrl = url };
            _context.Pictures.Add(picture);
            await _context.SaveChangesAsync(); 
            return picture; 
        }

        public async Task<Picture> AddAnalysisResult(Picture picture, VisionContract.AnalysisResult result)
        {
            picture.Description = result.Description.Captions[0].Text;

            picture.Tags = result.Tags
                .Select(t => new Tag { Name = t.Name, Confidence = t.Confidence })
                .ToList();

            picture.PictureAnalysed = true;

            _context.Entry(picture).State = System.Data.Entity.EntityState.Modified;
            await _context.SaveChangesAsync();
            return picture;
        }

        public async Task<Picture> AddFaceAnalysisResult(Picture picture, FaceContract.Face[] faces, Emotion[] emotions)
        {
            picture.Faces = faces.Select((f, index) => new Face
            {
                FaceAttributes = f.FaceAttributes,
                FaceRectangle = f.FaceRectangle,
                Emotion = emotions[index].Evaluate()
            }).ToList();
            picture.FaceAnalysed = true;
            _context.Entry(picture).State = System.Data.Entity.EntityState.Modified;
            await _context.SaveChangesAsync();
            return picture;
        }

        public async Task<Picture> ProcessIdentifyResults(Picture picture, IList<Guid> persistedPersonIds)
        {
            foreach (var item in persistedPersonIds.Select((pId, i) => new { pId, i }))
            {
                if (item.pId != Guid.Empty)
                {
                    Person person = _context.Persons.FirstOrDefault(p => p.PersonId == item.pId);
                    if (person == null)
                        picture.Faces[item.i].Person = new Person { PersonId = item.pId };
                    else
                        picture.Faces[item.i].PersonId = item.pId;
                }
            }
            picture.FaceAnalysed = true;
            _context.Entry(picture).State = System.Data.Entity.EntityState.Modified;
            await _context.SaveChangesAsync();

            return picture; 
        }
    }
}