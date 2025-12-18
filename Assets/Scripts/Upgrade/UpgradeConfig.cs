using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// Configure les améliorations avec leurs conditions de déverrouillage dans l'inspecteur Unity.
    [System.Serializable]
    public class UpgradeConfig
    {
        [SerializeField] public string upgradeId;
        [SerializeField] public UpgradeType upgradeType;
        [SerializeField] public string displayName;
        [SerializeField] public string description;
        [SerializeField] public double cost;
        [SerializeField] public float bonusValue;

        [SerializeField] public List<ConditionConfig> unlockConditions = new();

        /// Configuration sérialisable pour une condition de déverrouillage.
        [System.Serializable]
        public class ConditionConfig
        {
            [SerializeField] public UpgradeCondition.ConditionType conditionType;
            [SerializeField] public string cropName;
            [SerializeField] public XPType xpType = XPType.Planting;
            [SerializeField] public int requiredValue;

            public UpgradeCondition ToUpgradeCondition()
            {
                return new UpgradeCondition(conditionType, cropName, requiredValue, xpType);
            }
        }
    }

    /// Gestionnaire de configuration des améliorations à partir de l'inspecteur Unity.
    public class UpgradeConfigManager : MonoBehaviour
    {
        [SerializeField] private List<UpgradeConfig> upgradeConfigs = new();

        private void Start()
        {
            if (UpgradeManager.Instance == null)
            {
                Debug.LogError("[UpgradeConfigManager] UpgradeManager non trouvé!");
                return;
            }

            LoadUpgradesFromConfig();
        }

        /// Charge toutes les améliorations depuis les configurations de l'inspecteur.
        private void LoadUpgradesFromConfig()
        {
            foreach (var config in upgradeConfigs)
            {
                var upgrade = new Upgrade(
                    config.upgradeId,
                    config.upgradeType,
                    config.displayName,
                    config.description,
                    config.cost,
                    config.bonusValue
                );

                var conditions = new List<UpgradeCondition>();
                foreach (var conditionConfig in config.unlockConditions)
                {
                    conditions.Add(conditionConfig.ToUpgradeCondition());
                }

                UpgradeManager.Instance.AddUpgrade(upgrade, conditions);
                Debug.Log($"[UpgradeConfigManager] Amélioration '{config.upgradeId}' chargée avec {conditions.Count} condition(s).");
            }
        }
    }
}