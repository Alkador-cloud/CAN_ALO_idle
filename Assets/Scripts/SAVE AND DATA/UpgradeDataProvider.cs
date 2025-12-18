using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    public class UpgradeDataProvider : MonoBehaviour
    {
        [SerializeField] private List<UpgradeData> upgradeDataList = new();

        private void Start()
        {
            // Appeler après que la scène soit complètement chargée
            if (UpgradeManager.Instance != null)
            {
                foreach (var upgrade in upgradeDataList)
                {
                    if (upgrade != null)
                        UpgradeManager.Instance.AddUpgrade(
                            new Upgrade(
                                upgrade.UpgradeId,
                                upgrade.UpgradeType,
                                upgrade.DisplayName,
                                upgrade.Description,
                                upgrade.Cost,
                                upgrade.BonusValue),
                            upgrade.Conditions);
                }
            }
        }
    }
}