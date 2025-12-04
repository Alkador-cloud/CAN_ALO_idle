using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGame
{
    /// ScriptableObject configurable pour les améliorations avec leurs conditions.
    [CreateAssetMenu(fileName = "UpgradeData_", menuName = "Idle Game/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [SerializeField] private string upgradeId;
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private string displayName;
        [TextArea(2, 4)]
        [SerializeField] private string description;
        [SerializeField] private double cost;
        [SerializeField] private float bonusValue;
        [SerializeField] private Sprite buttonImage;
        [SerializeField] private List<UpgradeCondition> conditions = new();
        [SerializeField] private bool isUnlocked;
        [SerializeField] private bool isPurchased;

        public string UpgradeId => upgradeId;
        public UpgradeType UpgradeType => upgradeType;
        public string DisplayName => displayName;
        public string Description => description;
        public double Cost => cost;
        public float BonusValue => bonusValue;
        public Sprite ButtonImage => buttonImage;
        public List<UpgradeCondition> Conditions => conditions;
        public bool IsUnlocked => isUnlocked;
        public bool IsPurchased => isPurchased;

        /// Vérifie si toutes les conditions sont remplies.
        public bool AreAllConditionsMet()
        {
            if (conditions == null || conditions.Count == 0)
                return true;

            foreach (var condition in conditions)
            {
                if (!condition.IsMet())
                    return false;
            }

            return true;
        }
    }
}