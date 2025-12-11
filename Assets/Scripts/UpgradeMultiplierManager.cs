using UnityEngine;
using System.Collections.Generic;

namespace IdleGame
{
    /// Gère les multiplicateurs appliqués aux différentes méchaniques du jeu selon les upgrades achetés
    public class UpgradeMultiplierManager : MonoBehaviour
    {
        public static UpgradeMultiplierManager Instance { get; private set; }

        // Multiplicateurs par type d'upgrade (base = 1.0f)
        [SerializeField]
        private float clickPercentageMultiplier = 1f;
        [SerializeField]
        private float sellPriceMultiplier = 1f;
        [SerializeField]
        private float automationSpeedMultiplier = 1f;
        [SerializeField]
        private float automationPowerMultiplier = 1f;
        [SerializeField]
        private float xpGainMultiplier = 1f;
        [SerializeField]
        private float resourceProductionMultiplier = 1f;

        private UpgradeManager upgradeManager;

        public event System.Action OnMultipliersUpdated;

        public float ClickPercentageMultiplier => clickPercentageMultiplier;
        public float SellPriceMultiplier => sellPriceMultiplier;
        public float AutomationSpeedMultiplier => automationSpeedMultiplier;
        public float AutomationPowerMultiplier => automationPowerMultiplier;
        public float XPGainMultiplier => xpGainMultiplier;
        public float ResourceProductionMultiplier => resourceProductionMultiplier;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.SetParent(null, worldPositionStays: false);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            upgradeManager = UpgradeManager.Instance;
            
            if (upgradeManager != null)
            {
                upgradeManager.OnUpgradePurchased += OnUpgradePurchased;
                upgradeManager.OnUpgradesUpdated += RecalculateMultipliers;
                
                // Calculer les multiplicateurs initiaux
                RecalculateMultipliers();
            }
            else
            {
                Debug.LogWarning("[UpgradeMultiplierManager] UpgradeManager non trouvé!");
            }
        }

        private void OnDestroy()
        {
            if (upgradeManager != null)
            {
                upgradeManager.OnUpgradePurchased -= OnUpgradePurchased;
                upgradeManager.OnUpgradesUpdated -= RecalculateMultipliers;
            }
        }

        /// Appelé quand un upgrade est acheté
        private void OnUpgradePurchased(string upgradeId)
        {
            Debug.Log($"[UpgradeMultiplierManager] Upgrade '{upgradeId}' acheté, recalcul des multiplicateurs...");
            RecalculateMultipliers();
        }

        /// Recalcule tous les multiplicateurs en fonction des upgrades achetés
        public void RecalculateMultipliers()
        {
            if (upgradeManager == null)
                return;

            // Réinitialiser tous les multiplicateurs
            clickPercentageMultiplier = 1f;
            sellPriceMultiplier = 1f;
            automationSpeedMultiplier = 1f;
            automationPowerMultiplier = 1f;
            xpGainMultiplier = 1f;
            resourceProductionMultiplier = 1f;

            // Parcourir tous les upgrades achetés
            var purchasedUpgrades = upgradeManager.GetPurchasedUpgrades();
            
            foreach (var upgradeData in purchasedUpgrades)
            {
                float bonusValue = upgradeData.BonusValue;
                
                switch (upgradeData.UpgradeType)
                {
                    case UpgradeType.ClickPercentage:
                        clickPercentageMultiplier += bonusValue;
                        break;
                    
                    case UpgradeType.SellPrice:
                        sellPriceMultiplier += bonusValue;
                        break;
                    
                    case UpgradeType.AutomationSpeed:
                        automationSpeedMultiplier += bonusValue;
                        break;
                    case UpgradeType.AutomationPower:
                        automationPowerMultiplier += bonusValue;
                        break;
                    
                    case UpgradeType.XPGain:
                        xpGainMultiplier += bonusValue;
                        break;
                    
                    case UpgradeType.ResourceProduction:
                        resourceProductionMultiplier += bonusValue;
                        break;
                }
            }

            Debug.Log($"[UpgradeMultiplierManager] Multiplicateurs recalculés:" +
                $"\n  Click: {clickPercentageMultiplier}" +
                $"\n  SellPrice: {sellPriceMultiplier}" +
                $"\n  AutoSpeed: {automationSpeedMultiplier}" +
                $"\n  AutoPower: {automationPowerMultiplier}" +
                $"\n  XPGain: {xpGainMultiplier}" +
                $"\n  ResourceProd: {resourceProductionMultiplier}");

            OnMultipliersUpdated?.Invoke();
        }

        /// Définit le multiplicateur de clic en jeu
        public void SetClickPercentageMultiplier(float value)
        {
            clickPercentageMultiplier = Mathf.Max(0f, value);
            OnMultipliersUpdated?.Invoke();
        }

        /// Définit le multiplicateur de prix de vente en jeu
        public void SetSellPriceMultiplier(float value)
        {
            sellPriceMultiplier = Mathf.Max(0f, value);
            OnMultipliersUpdated?.Invoke();
        }

        /// Définit le multiplicateur de vitesse d'automatisation en jeu
        public void SetAutomationSpeedMultiplier(float value)
        {
            automationSpeedMultiplier = Mathf.Max(0f, value);
            OnMultipliersUpdated?.Invoke();
        }

        /// Définit le multiplicateur de puissance d'automatisation en jeu
        public void SetAutomationPowerMultiplier(float value)
        {
            automationPowerMultiplier = Mathf.Max(0f, value);
            OnMultipliersUpdated?.Invoke();
        }

        /// Définit le multiplicateur de gain XP en jeu
        public void SetXPGainMultiplier(float value)
        {
            xpGainMultiplier = Mathf.Max(0f, value);
            OnMultipliersUpdated?.Invoke();
        }

        /// Définit le multiplicateur de production de ressources en jeu
        public void SetResourceProductionMultiplier(float value)
        {
            resourceProductionMultiplier = Mathf.Max(0f, value);
            OnMultipliersUpdated?.Invoke();
        }
    }
}