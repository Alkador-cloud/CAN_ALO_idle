using UnityEngine;
using System;

namespace IdleGame
{
    public class Currency : MonoBehaviour
    {
        [SerializeField] private double initialBalance = 0f;

        private double currentBalance;

        public event Action<double> OnBalanceChanged;
        public event Action<double> OnMoneyAdded;

        public double CurrentBalance => currentBalance;

        void Start()
        {
            currentBalance = initialBalance;
            Debug.Log($"[Currency] Solde initial: {currentBalance}");
        }

        public void AddMoney(double amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[Currency] Montant invalide: {amount}");
                return;
            }

            currentBalance += amount;
            Debug.Log($"[Currency] Argent ajouté: +{amount} | Nouveau solde: {currentBalance}");
            
            OnMoneyAdded?.Invoke(amount);
            OnBalanceChanged?.Invoke(currentBalance);
        }

        public void RemoveMoney(double amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[Currency] Montant invalide: {amount}");
                return;
            }

            if (currentBalance < amount)
            {
                Debug.LogWarning($"[Currency] Solde insuffisant: {currentBalance} < {amount}");
                return;
            }

            currentBalance -= amount;
            Debug.Log($"[Currency] Argent retiré: -{amount} | Nouveau solde: {currentBalance}");
            OnBalanceChanged?.Invoke(currentBalance);
        }

        public bool HasEnoughMoney(double amount)
        {
            return currentBalance >= amount;
        }

        public void SetBalance(double amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"[Currency] Solde négatif non autorisé: {amount}");
                return;
            }

            currentBalance = amount;
            Debug.Log($"[Currency] Solde défini à: {currentBalance}");
            OnBalanceChanged?.Invoke(currentBalance);
        }
    }
}