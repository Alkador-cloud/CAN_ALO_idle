using UnityEngine;
using System;

namespace IdleGame
{
    public class CropState : MonoBehaviour
    {
        [SerializeField] private CropType cropType;
        [SerializeField] private Currency currency;
        [SerializeField] private float[] stageDurations = new float[6];
        [SerializeField] private float manualProgressIncrement = 0.1f;

        private ProductionStage currentStage = ProductionStage.Planting;
        private float stageTimer = 0f;
        private bool isProgressing = true;

        public event Action<ProductionStage> OnStageChanged;
        public event Action OnStageDurationUpdated;
        public event Action<double> OnCropSold;

        public ProductionStage CurrentStage => currentStage;
        public float CurrentStageDuration => stageDurations[(int)currentStage];
        public float ElapsedTime => stageTimer;
        public float RemainingTime => CurrentStageDuration - stageTimer;
        public float ProgressionPercentage => CurrentStageDuration > 0 ? stageTimer / CurrentStageDuration : 0f;

        void Start()
        {
            InitializeStageDurations();
            Debug.Log($"[CropState] Cycle commencé - Étape initiale: {GetStageName()}");
        }

        void Update()
        {
            if (!isProgressing)
                return;

            stageTimer += Time.deltaTime;

            if (stageTimer >= CurrentStageDuration)
            {
                ProgressStage();
            }

            OnStageDurationUpdated?.Invoke();
        }

        public void ProgressStage()
        {
            if (currentStage == ProductionStage.Selling)
            {
                SellCrop();
                Debug.Log($"[CropState] Étape actuelle: {GetStageName()} | Progression: {ProgressionPercentage:P2} | Cycle en cours de réinitialisation");
                ResetCycle();
            }
            else
            {
                currentStage++;
                stageTimer = 0f;
                Debug.Log($"[CropState] Étape actuelle: {GetStageName()} | Progression: {ProgressionPercentage:P2}");
                OnStageChanged?.Invoke(currentStage);
            }
        }

        public void IncreaseProgressManually()
        {
            if (stageTimer < CurrentStageDuration)
            {
                stageTimer += manualProgressIncrement * CurrentStageDuration;
                stageTimer = Mathf.Min(stageTimer, CurrentStageDuration);
                
                Debug.Log($"[CropState] Progression manuelle - Étape: {GetStageName()} | Progression: {ProgressionPercentage:P2}");

                if (stageTimer >= CurrentStageDuration)
                {
                    ProgressStage();
                }

                OnStageDurationUpdated?.Invoke();
            }
        }

        public void ResetCycle()
        {
            currentStage = ProductionStage.Planting;
            stageTimer = 0f;
            Debug.Log($"[CropState] Cycle réinitialisé - Étape actuelle: {GetStageName()} | Progression: {ProgressionPercentage:P2}");
            OnStageChanged?.Invoke(currentStage);
        }

        public void SetIsProgressing(bool value)
        {
            isProgressing = value;
        }

        private void SellCrop()
        {
            if (currency == null || cropType == null)
            {
                Debug.LogError("[CropState] Currency ou CropType non assigné");
                return;
            }

            double revenue = CalculateRevenue();
            currency.AddMoney(revenue);
            OnCropSold?.Invoke(revenue);
            Debug.Log($"[CropState] Récolte vendue - Revenu: {revenue}");
        }

        private double CalculateRevenue()
        {
            return (double)cropType.SellPrice * GetPriceMultiplier();
        }

        private float GetPriceMultiplier()
        {
            return 1f;
        }

        public double GetEstimatedRevenue()
        {
            if (cropType == null)
                return 0;

            return CalculateRevenue();
        }

        private void InitializeStageDurations()
        {
            if (cropType != null)
            {
                stageDurations = cropType.GetStageDurations();
            }
            else
            {
                stageDurations = new float[6] { 5f, 5f, 10f, 5f, 5f, 2f };
                Debug.LogWarning("CropType non assigné. Utilisation des durées par défaut.", gameObject);
            }
        }

        public string GetStageName()
        {
            return currentStage switch
            {
                ProductionStage.Planting => "Planter",
                ProductionStage.Watering => "Arroser",
                ProductionStage.Growing => "Pousser",
                ProductionStage.Harvesting => "Cueillir",
                ProductionStage.Packaging => "Emballer",
                ProductionStage.Selling => "Vendre",
                _ => "Inconnu"
            };
        }
    }
}