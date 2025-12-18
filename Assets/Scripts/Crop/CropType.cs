using UnityEngine;

namespace IdleGame
{
    [CreateAssetMenu(fileName = "CropType_", menuName = "Idle Game/Crop Type")]
    public class CropType : ScriptableObject
    {
        [SerializeField] private string cropName = "Cannabis";
        [SerializeField] private float sellPrice = 100f;
        [SerializeField] private float[] stageDurations = new float[6] { 5f, 5f, 10f, 5f, 5f, 2f };

        public string CropName => cropName;
        public float SellPrice => sellPrice;

        public float[] GetStageDurations()
        {
            return (float[])stageDurations.Clone();
        }

        public float GetStageDuration(ProductionStage stage)
        {
            return stageDurations[(int)stage];
        }
    }
}
