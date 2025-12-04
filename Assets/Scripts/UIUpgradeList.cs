using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// Gère l'affichage scrollable de tous les boutons d'amélioration.
    public class UIUpgradeList : MonoBehaviour
    {
        [SerializeField] private Transform contentPanel;
        [SerializeField] private UIUpgradeButton upgradeButtonPrefab;

        private List<UIUpgradeButton> spawnedButtons = new();
        private UpgradeManager upgradeManager;

        private void Start()
        {
            upgradeManager = UpgradeManager.Instance;

            if (upgradeManager == null)
            {
                Debug.LogError("[UIUpgradeList] UpgradeManager non trouvé!");
                return;
            }

            if (contentPanel == null)
            {
                Debug.LogError("[UIUpgradeList] ContentPanel non assigné!");
                return;
            }

            if (upgradeButtonPrefab == null)
            {
                Debug.LogError("[UIUpgradeList] UIUpgradeButton Prefab non assigné!");
                return;
            }

            PopulateUpgradeList();
        }

        /// Crée un bouton pour chaque UpgradeData.
        private void PopulateUpgradeList()
        {
            List<UpgradeData> allUpgrades = upgradeManager.GetAllUpgrades();

            foreach (var upgradeData in allUpgrades)
            {
                if (upgradeData == null)
                    continue;

                // Instancier le préfab du bouton
                UIUpgradeButton buttonInstance = Instantiate(upgradeButtonPrefab, contentPanel);
                buttonInstance.SetUpgradeData(upgradeData);

                spawnedButtons.Add(buttonInstance);
            }

            Debug.Log($"[UIUpgradeList] {spawnedButtons.Count} boutons d'amélioration créés.");
        }
    }
}