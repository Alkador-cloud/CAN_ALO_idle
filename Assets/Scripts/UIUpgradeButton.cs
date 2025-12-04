using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IdleGame
{
    /// Représente un bouton d'amélioration dans l'UI.
    public class UIUpgradeButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI displayNameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        private UpgradeData upgradeData;
        private UpgradeManager upgradeManager;
        private Currency currency;

        /// Assigne les données d'amélioration au bouton.
        public void SetUpgradeData(UpgradeData data)
        {
            upgradeData = data;
            InitializeButton();
        }

        private void InitializeButton()
        {
            upgradeManager = UpgradeManager.Instance;
            currency = Currency.FindFirstObjectByType<Currency>();

            if (upgradeManager == null || currency == null)
            {
                Debug.LogError("[UIUpgradeButton] UpgradeManager ou Currency non trouvé!");
                return;
            }

            if (upgradeData == null)
            {
                Debug.LogError("[UIUpgradeButton] UpgradeData non assigné!");
                return;
            }

            purchaseButton.onClick.AddListener(OnPurchaseClicked);
            upgradeManager.OnUpgradesUpdated += RefreshUI;
            currency.OnBalanceChanged += (balance) => RefreshUI();

            RefreshUI();
        }

        private void OnDisable()
        {
            if (upgradeManager != null)
                upgradeManager.OnUpgradesUpdated -= RefreshUI;

            if (currency != null)
                currency.OnBalanceChanged -= (balance) => RefreshUI();

            if (purchaseButton != null)
                purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
        }

        /// Met à jour l'affichage du bouton.
        private void RefreshUI()
        {
            if (upgradeData == null)
                return;

            // Afficher les informations depuis UpgradeData
            if (displayNameText != null)
                displayNameText.text = upgradeData.DisplayName;

            if (descriptionText != null)
                descriptionText.text = upgradeData.Description;

            if (costText != null)
                costText.text = $"Coût: {upgradeData.Cost:F0}";

            // Afficher l'image du bouton si disponible
            if (buttonImage != null && upgradeData.ButtonImage != null)
                buttonImage.sprite = upgradeData.ButtonImage;

            // Mettre à jour l'état du bouton
            bool isUnlocked = upgradeManager.IsUpgradeUnlocked(upgradeData.UpgradeId);
            bool isPurchased = upgradeManager.IsUpgradePurchased(upgradeData.UpgradeId);
            bool canAfford = currency != null && currency.HasEnoughMoney(upgradeData.Cost);

            // Bouton désactivé si déjà acheté
            if (isPurchased)
            {
                purchaseButton.interactable = false;
                if (buttonImage != null)
                    buttonImage.color = lockedColor;

                if (costText != null)
                    costText.text = "ACHETÉ";
                return;
            }

            // Bouton actif seulement si déverrouillé et argent disponible
            bool isInteractable = isUnlocked && canAfford;
            purchaseButton.interactable = isInteractable;

            if (buttonImage != null)
                buttonImage.color = isUnlocked ? unlockedColor : lockedColor;
        }

        /// Appelé quand le bouton d'achat est cliqué.
        private void OnPurchaseClicked()
        {
            if (upgradeManager.PurchaseUpgrade(upgradeData.UpgradeId))
            {
                RefreshUI();
            }
        }
    }
}