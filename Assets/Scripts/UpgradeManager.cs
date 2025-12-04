using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// Gère les améliorations du jeu basées sur des ScriptableObjects configurables.
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [SerializeField] private List<UpgradeData> upgradeDataList = new();

        private Dictionary<string, UpgradeEntry> upgradeMap = new();
        private Currency currency;
        private bool isInitialized = false;

        [System.Serializable]
        private class UpgradeEntry
        {
            public UpgradeData upgradeData;
            public bool isPurchased;
            public bool isUnlocked;
        }

        public event Action<string> OnUpgradeUnlocked;
        public event Action<string> OnUpgradePurchased;
        public event Action OnUpgradesUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // Déplacer le GameObject à la racine avant d'appeler DontDestroyOnLoad
            transform.SetParent(null, worldPositionStays: false);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (isInitialized)
                return;

            // Initialiser les améliorations depuis la liste assignée
            InitializeUpgradesFromData();

            currency = Currency.FindFirstObjectByType<Currency>();
            if (currency == null)
                Debug.LogError("[UpgradeManager] Currency non trouvé dans la scène!");

            // Abonner aux événements d'expérience pour vérifier les déblocages
            if (ExperienceManager.Instance != null)
            {
                ExperienceManager.Instance.OnLevelUp += (_, __) => CheckUnlockedUpgrades();
                ExperienceManager.Instance.OnStageLevelUp += (_, _, __) => CheckUnlockedUpgrades();
                ExperienceManager.Instance.OnTotalXPGained += (_, __) => CheckUnlockedUpgrades();
                ExperienceManager.Instance.OnStageXPGained += (_, _, __) => CheckUnlockedUpgrades();
            }

            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// Initialise les améliorations à partir des ScriptableObjects fournis.
        private void InitializeUpgradesFromData()
        {
            upgradeMap.Clear();

            if (upgradeDataList == null || upgradeDataList.Count == 0)
            {
                Debug.LogWarning("[UpgradeManager] Aucun UpgradeData assigné dans l'inspecteur!");
                return;
            }

            foreach (var upgradeData in upgradeDataList)
            {
                if (upgradeData == null)
                {
                    Debug.LogWarning("[UpgradeManager] Un UpgradeData est null!");
                    continue;
                }

                var entry = new UpgradeEntry
                {
                    upgradeData = upgradeData,
                    isPurchased = upgradeData.IsPurchased,
                    isUnlocked = upgradeData.IsUnlocked
                };

                upgradeMap[upgradeData.UpgradeId] = entry;
            }

            Debug.Log($"[UpgradeManager] {upgradeMap.Count} améliorations initialisées.");
        }

        /// Ajoute une amélioration au gestionnaire (pour la configuration dynamique).
        public void AddUpgrade(Upgrade upgrade, List<UpgradeCondition> conditions = null)
        {
            var upgradeData = ScriptableObject.CreateInstance<UpgradeData>();
            upgradeData.name = upgrade.UpgradeId;

            var entry = new UpgradeEntry
            {
                upgradeData = upgradeData,
                isPurchased = false,
                isUnlocked = false
            };

            upgradeMap[upgrade.UpgradeId] = entry;
            Debug.Log($"[UpgradeManager] Amélioration '{upgrade.UpgradeId}' ajoutée dynamiquement.");
        }

        /// Obtient une amélioration par son ID.
        public UpgradeData GetUpgrade(string upgradeId)
        {
            if (upgradeMap.TryGetValue(upgradeId, out var entry))
                return entry.upgradeData;

            return null;
        }

        /// Vérifie si une amélioration est déverrouillée.
        public bool IsUpgradeUnlocked(string upgradeId)
        {
            if (upgradeMap.TryGetValue(upgradeId, out var entry))
                return entry.isUnlocked;

            return false;
        }

        /// Vérifie si une amélioration a été achetée.
        public bool IsUpgradePurchased(string upgradeId)
        {
            if (upgradeMap.TryGetValue(upgradeId, out var entry))
                return entry.isPurchased;

            return false;
        }

        /// Définit le statut de déverrouillage d'une amélioration.
        public void SetUpgradeUnlocked(string upgradeId, bool isUnlocked)
        {
            if (!upgradeMap.TryGetValue(upgradeId, out var entry))
            {
                Debug.LogWarning($"[UpgradeManager] Amélioration '{upgradeId}' non trouvée.");
                return;
            }

            if (entry.isUnlocked == isUnlocked)
                return;

            entry.isUnlocked = isUnlocked;
            entry.upgradeData.SetIsUnlocked(isUnlocked);

            if (isUnlocked)
                OnUpgradeUnlocked?.Invoke(upgradeId);

            OnUpgradesUpdated?.Invoke();
            Debug.Log($"[UpgradeManager] Amélioration '{upgradeId}' déverrouillée : {isUnlocked}");
        }

        /// Définit le statut d'achat d'une amélioration.
        public void SetUpgradePurchased(string upgradeId, bool isPurchased)
        {
            if (!upgradeMap.TryGetValue(upgradeId, out var entry))
            {
                Debug.LogWarning($"[UpgradeManager] Amélioration '{upgradeId}' non trouvée.");
                return;
            }

            if (entry.isPurchased == isPurchased)
                return;

            entry.isPurchased = isPurchased;
            entry.upgradeData.SetIsPurchased(isPurchased);

            if (isPurchased)
                OnUpgradePurchased?.Invoke(upgradeId);

            OnUpgradesUpdated?.Invoke();
            Debug.Log($"[UpgradeManager] Amélioration '{upgradeId}' achetée : {isPurchased}");
        }

        /// Achète une amélioration si possible.
        public bool PurchaseUpgrade(string upgradeId)
        {
            if (!upgradeMap.TryGetValue(upgradeId, out var entry))
            {
                Debug.LogWarning($"[UpgradeManager] Amélioration '{upgradeId}' non trouvée.");
                return false;
            }

            if (entry.isPurchased)
            {
                Debug.LogWarning($"[UpgradeManager] L'amélioration '{upgradeId}' a déjà été achetée.");
                return false;
            }

            if (!entry.isUnlocked)
            {
                Debug.LogWarning($"[UpgradeManager] L'amélioration '{upgradeId}' n'est pas déverrouillée.");
                return false;
            }

            if (currency == null || !currency.HasEnoughMoney(entry.upgradeData.Cost))
            {
                Debug.LogWarning($"[UpgradeManager] Fonds insuffisants pour acheter '{upgradeId}'.");
                return false;
            }

            currency.RemoveMoney(entry.upgradeData.Cost);
            entry.isPurchased = true;
            entry.upgradeData.SetIsPurchased(true);
            OnUpgradePurchased?.Invoke(upgradeId);
            OnUpgradesUpdated?.Invoke();

            Debug.Log($"[UpgradeManager] Amélioration '{upgradeId}' achetée avec succès!");
            return true;
        }

        /// Vérifie et met à jour le statut de déverrouillage de toutes les améliorations.
        private void CheckUnlockedUpgrades()
        {
            bool anyChanged = false;

            foreach (var entry in upgradeMap.Values)
            {
                if (entry.isUnlocked || entry.isPurchased)
                    continue;

                if (entry.upgradeData.AreAllConditionsMet())
                {
                    entry.isUnlocked = true;
                    entry.upgradeData.SetIsUnlocked(true);
                    OnUpgradeUnlocked?.Invoke(entry.upgradeData.UpgradeId);
                    anyChanged = true;

                    Debug.Log($"[UpgradeManager] Amélioration '{entry.upgradeData.UpgradeId}' déverrouillée!");
                }
            }

            if (anyChanged)
                OnUpgradesUpdated?.Invoke();
        }

        /// Obtient toutes les améliorations.
        public List<UpgradeData> GetAllUpgrades()
        {
            return new List<UpgradeData>(upgradeDataList);
        }

        /// Obtient les améliorations déverrouillées.
        public List<UpgradeData> GetUnlockedUpgrades()
        {
            var result = new List<UpgradeData>();
            foreach (var entry in upgradeMap.Values)
            {
                if (entry.isUnlocked && !entry.isPurchased)
                    result.Add(entry.upgradeData);
            }
            return result;
        }

        /// Obtient les améliorations achetées.
        public List<UpgradeData> GetPurchasedUpgrades()
        {
            var result = new List<UpgradeData>();
            foreach (var entry in upgradeMap.Values)
            {
                if (entry.isPurchased)
                    result.Add(entry.upgradeData);
            }
            return result;
        }

        /// Réinitialise tous les achats d'améliorations.
        public void ResetAllUpgrades()
        {
            foreach (var entry in upgradeMap.Values)
            {
                entry.isPurchased = false;
                entry.isUnlocked = false;
                entry.upgradeData.SetIsPurchased(false);
                entry.upgradeData.SetIsUnlocked(false);
            }

            CheckUnlockedUpgrades();
            Debug.Log("[UpgradeManager] Toutes les améliorations ont été réinitialisées.");
        }
    }
}