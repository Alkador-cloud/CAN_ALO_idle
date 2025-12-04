using UnityEngine;

namespace IdleGame
{
    /// Définit une condition de déverrouillage pour une amélioration.
    [System.Serializable]
    public class UpgradeCondition
    {
        public enum ConditionType
        {
            TotalLevel,         // Niveau global d'une culture
            StageLevel,         // Niveau d'une étape spécifique
            TotalXP,            // XP global d'une culture
            StageXP             // XP d'une étape spécifique
        }

        [SerializeField] private ConditionType conditionType;
        [SerializeField] private string cropName;
        [SerializeField] private XPType xpType;
        [SerializeField] private int requiredValue;

        public ConditionType Type => conditionType;
        public string CropName => cropName;
        public XPType XpType => xpType;
        public int RequiredValue => requiredValue;

        public UpgradeCondition(ConditionType type, string crop, int required, XPType xpT = XPType.Planting)
        {
            conditionType = type;
            cropName = crop;
            requiredValue = required;
            xpType = xpT;
        }

        /// Vérifie si la condition est remplie.
        public bool IsMet()
        {
            if (ExperienceManager.Instance == null)
                return false;

            switch (conditionType)
            {
                case ConditionType.TotalLevel:
                    {
                        var cropExp = ExperienceManager.Instance.GetCropExperience(cropName);
                        return cropExp.TotalLevel >= requiredValue;
                    }

                case ConditionType.StageLevel:
                    {
                        var stageExp = ExperienceManager.Instance.GetStageExperience(cropName, xpType);
                        return stageExp.CurrentLevel >= requiredValue;
                    }

                case ConditionType.TotalXP:
                    {
                        var cropExp = ExperienceManager.Instance.GetCropExperience(cropName);
                        return cropExp.TotalXP >= requiredValue;
                    }

                case ConditionType.StageXP:
                    {
                        var stageExp = ExperienceManager.Instance.GetStageExperience(cropName, xpType);
                        return stageExp.CurrentXP >= requiredValue;
                    }

                default:
                    return false;
            }
        }
    }
}