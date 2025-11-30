using System;

namespace IdleGame
{
    [Serializable]
    public class GameData
    {
        // Index de l'étape de production (ProductionStage as int)
        public int CurrentStageIndex;

        // Temps écoulé dans l'étape courante
        public float StageTimer;

        // Durées des étapes (optionnel, permet restaurer custom durations)
        public float[] StageDurations;

        // Solde de la monnaie (double demandé)
        public double CurrencyBalance;

        // Données d'expérience (T7.1 - T7.3)
        public ExperienceData ExperienceData;
    }
}