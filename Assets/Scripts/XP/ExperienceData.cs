using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleGame
{
    /// Conteneur sérialisable pour les données d'expérience.
    [Serializable]
    public class ExperienceData
    {
        [Serializable]
        public class SerializedStageXP
        {
            public int xpType;
            public int currentXP;
            public int currentLevel;
        }

        [Serializable]
        public class SerializedCropXP
        {
            public string cropName;
            public int totalXP;
            public int totalLevel;
            public SerializedStageXP[] stageExperiences;
        }

        public SerializedCropXP[] crops = new SerializedCropXP[0];

        public void SerializeCropExperiences(Dictionary<string, ExperienceManager.CropExperience> cropExperiences)
        {
            crops = cropExperiences.Select(kvp => new SerializedCropXP
            {
                cropName = kvp.Key,
                totalXP = kvp.Value.TotalXP,
                totalLevel = kvp.Value.TotalLevel,
                stageExperiences = kvp.Value.StageExperiences
                    .Select(s => new SerializedStageXP
                    {
                        xpType = (int)s.Key,
                        currentXP = s.Value.CurrentXP,
                        currentLevel = s.Value.CurrentLevel
                    })
                    .ToArray()
            }).ToArray();
        }

        public Dictionary<string, ExperienceManager.CropExperience> DeserializeCropExperiences()
        {
            var result = new Dictionary<string, ExperienceManager.CropExperience>();

            foreach (var cropData in crops)
            {
                var cropExp = new ExperienceManager.CropExperience
                {
                    TotalXP = cropData.totalXP,
                    TotalLevel = cropData.totalLevel
                };

                foreach (var stageData in cropData.stageExperiences)
                {
                    var xpType = (XPType)stageData.xpType;
                    cropExp.StageExperiences[xpType] = new ExperienceManager.StageExperience
                    {
                        CurrentXP = stageData.currentXP,
                        CurrentLevel = stageData.currentLevel
                    };
                }

                result[cropData.cropName] = cropExp;
            }

            return result;
        }
    }
}