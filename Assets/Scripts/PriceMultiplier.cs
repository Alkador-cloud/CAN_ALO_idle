using UnityEngine;

namespace IdleGame
{
    public static class PriceMultiplier
    {
        private static float cropTypeMultiplier = 1f;
        private static float bonusMultiplier = 1f;

        public static float GetTotalMultiplier()
        {
            return cropTypeMultiplier * bonusMultiplier;
        }

        public static void SetCropTypeMultiplier(float multiplier)
        {
            cropTypeMultiplier = Mathf.Max(0.1f, multiplier);
            Debug.Log($"[PriceMultiplier] Multiplicateur de type défini à: {cropTypeMultiplier}");
        }

        public static void SetBonusMultiplier(float multiplier)
        {
            bonusMultiplier = Mathf.Max(0.1f, multiplier);
            Debug.Log($"[PriceMultiplier] Multiplicateur de bonus défini à: {bonusMultiplier}");
        }

        public static void ResetMultipliers()
        {
            cropTypeMultiplier = 1f;
            bonusMultiplier = 1f;
        }
    }
}