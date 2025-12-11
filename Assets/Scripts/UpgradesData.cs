using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleGame
{
    /// Conteneur sérialisable pour les données d'améliorations.
    [Serializable]
    public class UpgradesData
    {
        [Serializable]
        public class SerializedUpgrade
        {
            public string upgradeId;
            public bool isPurchased;
            public bool isUnlocked;
        }

        public SerializedUpgrade[] upgrades = new SerializedUpgrade[0];

        public void SerializeUpgrades(Dictionary<string, UpgradeManager.UpgradeEntry> upgradeMap)
        {
            upgrades = upgradeMap.Values.Select(entry => new SerializedUpgrade
            {
                upgradeId = entry.upgradeData.UpgradeId,
                isPurchased = entry.isPurchased,
                isUnlocked = entry.isUnlocked
            }).ToArray();
        }

        public Dictionary<string, (bool isPurchased, bool isUnlocked)> DeserializeUpgrades()
        {
            var result = new Dictionary<string, (bool, bool)>();

            foreach (var upgradeData in upgrades)
            {
                result[upgradeData.upgradeId] = (upgradeData.isPurchased, upgradeData.isUnlocked);
            }

            return result;
        }
    }
}