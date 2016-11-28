using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevDay2016SmartGallery.Extensions
{
    public static class EmotionExtensions
    {
        public static string Evaluate(this Emotion emotion)
        {
            Dictionary<string, double> emotions = new Dictionary<string, double>()
            {
                { "Anger", emotion.Scores.Anger },
                { "Contempt", emotion.Scores.Contempt },
                { "Disgust", emotion.Scores.Disgust },
                { "Fear", emotion.Scores.Fear },
                { "Happiness", emotion.Scores.Happiness },
                { "Neutral", emotion.Scores.Neutral },
                { "Sadness", emotion.Scores.Sadness },
                { "Surprise", emotion.Scores.Surprise },
            };
            return emotions.OrderByDescending(e => e.Value).First().Key;
        }
    }
}