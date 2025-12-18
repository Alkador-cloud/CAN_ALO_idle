using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// G�re l'exp�rience des joueurs par type de culture et par �tape de production.
    public class ExperienceManager : MonoBehaviour
    {
        public static ExperienceManager Instance { get; private set; }

        [SerializeField] private int[] experienceLevelThresholds = new int[]
        {
            0, 5, 15, 40, 100, 250, 750, 2000, 5000, 12500, 30000
        };

        /// Structure pour stocker l'XP d'une �tape de production pour une culture.
        [System.Serializable]
        public class StageExperience
        {
            public int CurrentXP;
            public int CurrentLevel = 1;
        }

        /// Structure pour stocker toutes les donn�es d'XP d'une culture.
        [System.Serializable]
        public class CropExperience
        {
            public int TotalXP;
            public int TotalLevel = 1;
            public Dictionary<XPType, StageExperience> StageExperiences = new();
        }

        private Dictionary<string, CropExperience> cropExperiences = new();

        public event Action<string, int> OnTotalXPGained;
        public event Action<string, XPType, int> OnStageXPGained;
        public event Action<string, int> OnLevelUp;
        public event Action<string, XPType, int> OnStageLevelUp;
        public event Action<UpgradeConfig> OnUpgradeUnlocked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            var go = new GameObject("ExperienceManager");
            DontDestroyOnLoad(go);
            go.AddComponent<ExperienceManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// Initialise les donn�es d'XP pour une culture.
        public void InitializeCrop(string cropName)
        {
            if (cropExperiences.ContainsKey(cropName))
                return;

            var cropExp = new CropExperience();
            foreach (XPType xpType in System.Enum.GetValues(typeof(XPType)))
            {
                cropExp.StageExperiences[xpType] = new StageExperience();
            }

            cropExperiences[cropName] = cropExp;
        }

        /// Ajoute de l'XP � l'�tape courante et au total.
        public void GainStageXP(string cropName, ProductionStage stage, int xpAmount)
        {
            XPType xpType = (XPType)stage;
            GainStageXP(cropName, xpType, xpAmount);
        }

        /// Ajoute de l'XP � l'�tape courante (version surcharg�e avec XPType).
        public void GainStageXP(string cropName, XPType xpType, int xpAmount)
        {
            InitializeCrop(cropName);
            var cropExp = cropExperiences[cropName];

            cropExp.StageExperiences[xpType].CurrentXP += xpAmount;
            OnStageXPGained?.Invoke(cropName, xpType, xpAmount);

            CheckStageLevelUp(cropName, xpType);

            GainTotalXP(cropName, xpAmount);
        }

        /// Ajoute de l'XP au total (lors de la compl�tion d'un cycle complet).
        private void GainTotalXP(string cropName, int xpAmount)
        {
            InitializeCrop(cropName);
            var cropExp = cropExperiences[cropName];

            cropExp.TotalXP += xpAmount;
            OnTotalXPGained?.Invoke(cropName, xpAmount);

            // V�rifier les niveaux globaux
            CheckTotalLevelUp(cropName);
        }

        /// V�rifie si l'�tape a progress� de niveau.
        private void CheckStageLevelUp(string cropName, XPType xpType)
        {
            var stageExp = cropExperiences[cropName].StageExperiences[xpType];
            int nextThreshold = GetLevelThreshold(stageExp.CurrentLevel + 1);

            if (stageExp.CurrentXP >= nextThreshold)
            {
                stageExp.CurrentLevel++;
                OnStageLevelUp?.Invoke(cropName, xpType, stageExp.CurrentLevel);
                Debug.Log($"[ExperienceManager] {cropName} - {xpType}: Niveau {stageExp.CurrentLevel}!");
            }
        }

        /// V�rifie si le niveau total a progress�.
        private void CheckTotalLevelUp(string cropName)
        {
            var cropExp = cropExperiences[cropName];
            int nextThreshold = GetLevelThreshold(cropExp.TotalLevel + 1);

            if (cropExp.TotalXP >= nextThreshold)
            {
                cropExp.TotalLevel++;
                OnLevelUp?.Invoke(cropName, cropExp.TotalLevel);
                Debug.Log($"[ExperienceManager] {cropName}: Niveau total {cropExp.TotalLevel}!");
            }
        }

        /// Obtient le seuil d'XP pour un niveau donn�.
        public int GetLevelThreshold(int level)
        {
            if (level <= 0) return 0;
            if (level - 1 >= experienceLevelThresholds.Length)
                return experienceLevelThresholds[experienceLevelThresholds.Length - 1];
            return experienceLevelThresholds[level - 1];
        }

        /// Obtient l'XP restant pour le prochain niveau.
        public int GetXPToNextLevel(string cropName, bool isStage = false, XPType xpType = XPType.Planting)
        {
            if (!cropExperiences.TryGetValue(cropName, out var cropExp))
                return GetLevelThreshold(2);

            if (isStage)
            {
                var stageExp = cropExp.StageExperiences[xpType];
                int currentThreshold = GetLevelThreshold(stageExp.CurrentLevel);
                int nextThreshold = GetLevelThreshold(stageExp.CurrentLevel + 1);
                return Mathf.Max(0, nextThreshold - stageExp.CurrentXP);
            }
            else
            {
                int currentThreshold = GetLevelThreshold(cropExp.TotalLevel);
                int nextThreshold = GetLevelThreshold(cropExp.TotalLevel + 1);
                return Mathf.Max(0, nextThreshold - cropExp.TotalXP);
            }
        }

        /// Obtient les donn�es d'XP d'une culture.
        public CropExperience GetCropExperience(string cropName)
        {
            InitializeCrop(cropName);
            return cropExperiences[cropName];
        }

        /// Obtient les donn�es d'XP d'une �tape.
        public StageExperience GetStageExperience(string cropName, XPType xpType)
        {
            InitializeCrop(cropName);
            return cropExperiences[cropName].StageExperiences[xpType];
        }

        /// Charge les donn�es d'XP depuis GameData.
        public void LoadExperienceData(ExperienceData expData)
        {
            if (expData == null)
                return;

            cropExperiences = expData.DeserializeCropExperiences();
        }

        /// Sauvegarde les donn�es d'XP dans un format s�rialisable.
        public ExperienceData SaveExperienceData()
        {
            var data = new ExperienceData();
            data.SerializeCropExperiences(cropExperiences);
            return data;
        }

        /// R�initialise toutes les donn�es d'exp�rience.
        public void ClearAllExperience()
        {
            cropExperiences.Clear();
            Debug.Log("[ExperienceManager] Toutes les donn�es d'exp�rience ont �t� r�initialis�es.");
        }

        /// R�initialise l'exp�rience d'une culture sp�cifique.
        public void ClearCropExperience(string cropName)
        {
            if (cropExperiences.ContainsKey(cropName))
            {
                cropExperiences.Remove(cropName);
                Debug.Log($"[ExperienceManager] Exp�rience de '{cropName}' r�initialis�e.");
            }
        }

        protected virtual void OnLevelUpEvent(string cropName, int newLevel)
        {
            OnLevelUp?.Invoke(cropName, newLevel);
            Debug.Log($"[ExperienceManager] �v�nement OnLevelUp d�clench� - Niveau: {newLevel}");
        }

        // Dans la m�thode o� un upgrade est d�bloqu�:
        private void UnlockUpgrade(UpgradeConfig upgrade)
        {
            OnUpgradeUnlocked?.Invoke(upgrade);
            Debug.Log($"[UpgradeManager] �v�nement OnUpgradeUnlocked d�clench� - {upgrade.displayName}");
        }
    }
}
