using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// Représente une amélioration débloquable dans le jeu.
    [System.Serializable]
    public class Upgrade
    {
        [SerializeField] private string upgradeId;
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private double cost;
        [SerializeField] private float bonusValue;  // Valeur du bonus (ex: 1.25 pour +25%)
        [SerializeField] private bool isPurchased;

        public string UpgradeId => upgradeId;
        public UpgradeType UpgradeType => upgradeType;
        public string DisplayName => displayName;
        public string Description => description;
        public double Cost => cost;
        public float BonusValue => bonusValue;
        public bool IsPurchased => isPurchased;

        public event Action OnPurchased;

        public Upgrade(string id, UpgradeType type, string name, string desc, double upgradeC, float bonus)
        {
            upgradeId = id;
            upgradeType = type;
            displayName = name;
            description = desc;
            cost = upgradeC;
            bonusValue = bonus;
            isPurchased = false;
        }

        public void Purchase()
        {
            if (isPurchased)
            {
                Debug.LogWarning($"[Upgrade] L'amélioration '{upgradeId}' a déjà été achetée.");
                return;
            }

            isPurchased = true;
            Debug.Log($"[Upgrade] Amélioration '{upgradeId}' achetée!");
            OnPurchased?.Invoke();
        }

        public void ResetPurchaseStatus()
        {
            isPurchased = false;
        }
    }
}