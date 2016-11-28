using DevDay2016SmartGallery.Services;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DevDay2016SmartGallery.Controllers
{
    public class AdminController : Controller
    {
        private CognitiveService _cognitiveService; 

        public AdminController()
        {
            _cognitiveService = new CognitiveService(); 
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreatePersonGroup()
        {
            await _cognitiveService.CreatePersonGroupIfNotExists();
            return RedirectToAction("Index"); 
        }

        public async Task<ActionResult> AddJhonDoe()
        {
            var picture = "https://goo.gl/G81UTH";
            var faces = await _cognitiveService.AnalayseFaces(picture);
            var result = await _cognitiveService.AddPersonToPersonGroup();
            await _cognitiveService.AddFaceToPerson(result.PersonId, picture, faces[0].FaceRectangle);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeletePersonGroup()
        {
            await _cognitiveService.DeletePersonGroup();
            return RedirectToAction("Index"); 
        }

        public async Task<ActionResult> AnalaysePicture(){
            return Json(
                await _cognitiveService.AnalaysePicture("https://goo.gl/LSk5kh"), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AnalayseFaces()
        {
            return Json(
                await _cognitiveService.AnalayseFaces("https://goo.gl/gbNSS0"), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> TrainGroup()
        {
            var status = await _cognitiveService.TrainPersonGroupAsync();
            return Json(status.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}