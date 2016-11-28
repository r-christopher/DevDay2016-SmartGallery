using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using FaceContracts = Microsoft.ProjectOxford.Face.Contract;


namespace DevDay2016SmartGallery.Services
{
    public class CognitiveService
    {
        private IFaceServiceClient _faceService;
        private VisionServiceClient _visionService;
        private EmotionServiceClient _emotionService;
        private string _personGroupId; 

        public CognitiveService()
        {
            _faceService = new FaceServiceClient(ConfigurationManager.AppSettings["FaceAPIKey"]);
            _visionService = new VisionServiceClient(ConfigurationManager.AppSettings["ComputerVisionAPIKey"]);
            _emotionService = new EmotionServiceClient(ConfigurationManager.AppSettings["EmotionAPIKey"]);
            _personGroupId = ConfigurationManager.AppSettings["PersonGroupId"];
        }

        public async Task<AnalysisResult> AnalaysePicture(string pictureUrl)
        { 
            return await _visionService.AnalyzeImageAsync(pictureUrl, new List<VisualFeature> {
                VisualFeature.Description,
                VisualFeature.Tags,
            });
        }

        // Step 2
        public async Task<FaceContracts.Face[]> AnalayseFaces(string pictureUrl)
        {   
            return await _faceService.DetectAsync(pictureUrl, true, false, new List<FaceAttributeType>
            {
                FaceAttributeType.Age, 
                FaceAttributeType.Gender, 
                FaceAttributeType.Glasses, 
                FaceAttributeType.Smile,
            });
        }
        
        public async Task<Emotion[]> AnalayseEmotions(string pictureUrl)
        {
            return await _emotionService.RecognizeAsync(pictureUrl);
        }

        #region FACE RECOGNITION

        // Step 1
        public async Task CreatePersonGroupIfNotExists()
        {
            try{
                await _faceService.GetPersonGroupAsync(_personGroupId);
            }
            catch (FaceAPIException){
                await _faceService.CreatePersonGroupAsync(_personGroupId, Guid.NewGuid().ToString());
            }
        }

        public async Task DeletePersonGroup()
        {
            await _faceService.DeletePersonGroupAsync(_personGroupId); 
        }

        // Step 3
        public async Task<CreatePersonResult> AddPersonToPersonGroup()
        {
            return await _faceService.CreatePersonAsync(_personGroupId, Guid.NewGuid().ToString()); 
        }

        // Step 4
        public async Task<AddPersistedFaceResult> AddFaceToPerson(Guid personId, string pictureUrl, FaceContracts.FaceRectangle faceRectangle)
        {
            return await _faceService.AddPersonFaceAsync(_personGroupId, personId, pictureUrl, targetFace: faceRectangle);
        }

        // Step 5
        public async Task<Status> TrainPersonGroupAsync()
        {
            return await Task.Run(async () =>
            {
                Status status;

                await _faceService.TrainPersonGroupAsync(_personGroupId);

                while (true)
                {
                    await Task.Delay(1000);
                    var trainningStatus = await _faceService.GetPersonGroupTrainingStatusAsync(_personGroupId);

                    status = trainningStatus.Status;

                    if (status != Status.Running)
                    {
                        break;
                    }
                }

                return status;
            });
        }

        // Step 6
        public async Task<IdentifyResult[]> Identify(FaceContracts.Face[] faces)
        {
            if(faces.Any() && await TrainPersonGroupAsync() == Status.Succeeded)
            {
                var faceIds = faces.Select(f => f.FaceId).ToArray();
                return await _faceService.IdentifyAsync(_personGroupId, faceIds);
            }

            return faces.Select(f => new IdentifyResult { FaceId = f.FaceId, Candidates = new Candidate[0] }).ToArray();
        }

        public async Task<IList<Guid>> ProcessIdentifyResults(string pictureUrl, FaceContracts.Face[] faces, IdentifyResult[] identifyResults)
        {
            var persistedPersonIds = new List<Guid>();
            Guid persistedPersonId; 

            for(int i = 0; i < identifyResults.Length; i++)
            {
                if(faces[i].FaceRectangle.Width > 72 && faces[i].FaceRectangle.Height > 72)
                {
                    if (identifyResults[i].Candidates.Any())
                        persistedPersonId = identifyResults[i].Candidates[0].PersonId;
                    else
                        persistedPersonId = (await AddPersonToPersonGroup()).PersonId;

                    await AddFaceToPerson(persistedPersonId, pictureUrl, faces[i].FaceRectangle);
                }
                else
                {
                    persistedPersonId = Guid.Empty; 
                }

                persistedPersonIds.Add(persistedPersonId);
            }

            return persistedPersonIds;
        }

        #endregion
    }
}