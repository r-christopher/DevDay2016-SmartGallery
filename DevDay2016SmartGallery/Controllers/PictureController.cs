using DevDay2016SmartGallery.DAL;
using DevDay2016SmartGallery.DTOs;
using DevDay2016SmartGallery.Models;
using DevDay2016SmartGallery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DevDay2016SmartGallery.Controllers
{
    public class PictureController : Controller
    {
        private UnitOfWork _unit;
        private CognitiveService _cognitiveService; 

        public PictureController()
        {
            _unit = new UnitOfWork();
            _cognitiveService = new CognitiveService(); 
        }

        // GET: Picture
        public ActionResult Index()
        {
            return View(_unit.Context.Pictures.AsEnumerable()); 
        }

        [HttpPost]
        public async Task<ActionResult> Upload(IEnumerable<HttpPostedFileBase> files)
        {
            var storageService = new AlbumStorageService();
            var acceptedTypes = new[] { "image/jpeg", "image/png" };
            var pictures = new List<Picture>(); 

            foreach(var file in files)
            {
                if(file.ContentLength > 0 && acceptedTypes.Contains(file.ContentType))
                {
                    string name = Guid.NewGuid().ToString();
                    string url = await storageService.SavePicture("gallery", name,file.InputStream);
                    var picture = await _unit.PictureRepository.AddPicture(name, url); 
                    pictures.Add(picture); 
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult View(int id)
        {
            var picture = _unit.PictureRepository.GetPictureById(id);
            return View(picture);
        }

        public async Task<ActionResult> AnalysePicture(int id)
        {
            var picture = _unit.PictureRepository.GetPictureById(id);
            
            if (!picture.PictureAnalysed){
                var result = await _cognitiveService.AnalaysePicture(picture.PictureUrl);
                picture = await _unit.PictureRepository.AddAnalysisResult(picture, result);
            }

            return Json(new PictureDataDto {
                PictureId = id,
                Description = picture.Description,
                Tags = picture.Tags.Select(t => t.Name).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AnalyseFaces(int id)
        {
            var picture = _unit.PictureRepository.GetPictureById(id);

            if (!picture.FaceAnalysed)
            {
                var faces = (await _cognitiveService.AnalayseFaces(picture.PictureUrl)).Take(10).ToArray();
                var emotionResult =  await _cognitiveService.AnalayseEmotions(picture.PictureUrl);
                picture = await _unit.PictureRepository.AddFaceAnalysisResult(picture, faces, emotionResult);

                var identifyResults = await _cognitiveService.Identify(faces);
                var result = await _cognitiveService.ProcessIdentifyResults(picture.PictureUrl, faces, identifyResults);
                picture = await _unit.PictureRepository.ProcessIdentifyResults(picture, result);
            }

            return Json(picture.Faces.Select(f => f.Person?.Name), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdatePersonName(int faceId, string name)
        {
            await _unit.FaceRepository.UpdatePersonName(faceId, name);
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unit?.Dispose(); 
            }
            base.Dispose(disposing);
        }
    }
}