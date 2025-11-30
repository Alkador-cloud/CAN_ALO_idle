using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// Gère l'expérience des joueurs par type de culture et par étape de production.
    public class ExperienceManager : MonoBehaviour
    {
        public static ExperienceManager Instance { get; private set; }

        [SerializeField] private int[] experienceLevelThresholds = new int[]
        {
            0, 100, 250, 450, 700, 1000, 1350, 1750, 2200, 2700, 3250
        };

        /// Structure pour stocker l'XP d'une étape de production pour une culture.
        [System.Serializable]
        public class StageExperience
        {
            public int CurrentXP;
            public int CurrentLevel = 1;
        }

        /// Structure pour stocker toutes les données d'XP d'une culture.
        [System.Serializable]
        public class CropExperience
        {
            public int TotalXP;
            public int TotalLevel = 1;
            public Dictionary<ProductionStage, StageExperience> StageExperiences = new();
        }

        private Dictionary<string, CropExperience> cropExperiences = new();

        public event Action<string, int> OnTotalXPGained;
        public event Action<string, ProductionStage, int> OnStageXPGained;
        public event Action<string, int> OnLevelUp;
        public event Action<string, ProductionStage, int> OnStageLevelUp;

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

        /// Initialise les données d'XP pour une culture.
        public void InitializeCrop(string cropName)
        {
            if (cropExperiences.ContainsKey(cropName))
                return;

            var cropExp = new CropExperience();
            foreach (ProductionStage stage in System.Enum.GetValues(typeof(ProductionStage)))
            {
                cropExp.StageExperiences[stage] = new StageExperience();
            }

            cropExperiences[cropName] = cropExp;
        }

        /// Ajoute de l'XP à l'étape courante et au total.
        public void GainStageXP(string cropName, ProductionStage stage, int xpAmount)
        {
            InitializeCrop(cropName);
            var cropExp = cropExperiences[cropName];

            cropExp.StageExperiences[stage].CurrentXP += xpAmount;
            OnStageXPGained?.Invoke(cropName, stage, xpAmount);

            CheckStageLevelUp(cropName, stage);

            GainTotalXP(cropName, xpAmount);
        }

        /// Ajoute de l'XP au total (lors de la complétion d'un cycle complet).
        private void GainTotalXP(string cropName, int xpAmount)
        {
            InitializeCrop(cropName);
            var cropExp = cropExperiences[cropName];

            cropExp.TotalXP += xpAmount;
            OnTotalXPGained?.Invoke(cropName, xpAmount);

            // Vérifier les niveaux globaux
            CheckTotalLevelUp(cropName);
        }

        /// Vérifie si l'étape a progressé de niveau.
        private void CheckStageLevelUp(string cropName, ProductionStage stage)
        {
            var stageExp = cropExperiences[cropName].StageExperiences[stage];
            int nextThreshold = GetLevelThreshold(stageExp.CurrentLevel + 1);

            if (stageExp.CurrentXP >= nextThreshold)
            {
                stageExp.CurrentLevel++;
                OnStageLevelUp?.Invoke(cropName, stage, stageExp.CurrentLevel);
                Debug.Log($"[ExperienceManager] {cropName} - {stage}: Niveau {stageExp.CurrentLevel}!");
            }
        }

        /// Vérifie si le niveau total a progressé.
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

        /// Obtient le seuil d'XP pour un niveau donné.
        public int GetLevelThreshold(int level)
        {
            if (level <= 0) return 0;
            if (level - 1 >= experienceLevelThresholds.Length)
                return experienceLevelThresholds[experienceLevelThresholds.Length - 1];
            return experienceLevelThresholds[level - 1];
        }

        /// Obtient l'XP restant pour le prochain niveau.
        public int GetXPToNextLevel(string cropName, bool isStage = false, ProductionStage stage = ProductionStage.Planting)
        {
            if (!cropExperiences.TryGetValue(cropName, out var cropExp))
                return GetLevelThreshold(2);

            if (isStage)
            {
                var stageExp = cropExp.StageExperiences[stage];
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

        /// Obtient les données d'XP d'une culture.
        public CropExperience GetCropExperience(string cropName)
        {
            InitializeCrop(cropName);
            return cropExperiences[cropName];
        }

        /// Obtient les données d'XP d'une étape.
        public StageExperience GetStageExperience(string cropName, ProductionStage stage)
        {
            InitializeCrop(cropName);
            return cropExperiences[cropName].StageExperiences[stage];
        }

        /// Charge les données d'XP depuis GameData.
        public void LoadExperienceData(ExperienceData expData)
        {
            if (expData == null)
                return;

            cropExperiences = expData.DeserializeCropExperiences();
        }

        /// Sauvegarde les données d'XP dans un format sérialisable.
        public ExperienceData SaveExperienceData()
        {
            var data = new ExperienceData();
            data.SerializeCropExperiences(cropExperiences);
            return data;
        }
    }
}