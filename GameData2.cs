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

        // Solde de la monnaie (valeur négative => pas de donnée de monnaie présente)
        public double CurrencyBalance = -1.0;
    }
}