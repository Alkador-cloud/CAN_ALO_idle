using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IdleGame
{
    public class SaveManager : MonoBehaviour
    {
        private const string SaveKey = "CAN_ALO_idle_Save_v1";
        private const string SaveFileName = SaveKey + ".json";
        public static SaveManager Instance { get; private set; }

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            var go = new GameObject("SaveManager");
            DontDestroyOnLoad(go);
            go.AddComponent<SaveManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.quitting += OnApplicationQuitting;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.quitting -= OnApplicationQuitting;
            if (Instance == this) Instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TryLoadAndApply();

            var crop = UnityEngine.Object.FindFirstObjectByType<CropState>();
            if (crop != null)
            {
                crop.OnStageChanged += OnCropStageChanged;
            }
        }

        private void OnCropStageChanged(UnityEngine.SceneManagement.Scene _) { }

        private void OnCropStageChanged(ProductionStage newStage)
        {
            SaveNow();
        }

        public void SaveNow()
        {
            var crop = UnityEngine.Object.FindFirstObjectByType<CropState>();
            if (crop == null)
            {
                Debug.LogWarning("[SaveManager] Aucun CropState trouvé, sauvegarde annulée.");
                return;
            }

            var currency = UnityEngine.Object.FindFirstObjectByType<Currency>();
            var experienceManager = ExperienceManager.Instance;

            var data = new GameData
            {
                CurrentStageIndex = (int)crop.CurrentStage,
                StageTimer = crop.ElapsedTime,
                StageDurations = null,
                CurrencyBalance = currency != null ? currency.GetBalance() : 0.0,
                ExperienceData = experienceManager != null ? experienceManager.SaveExperienceData() : null
            };

            try
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[SaveManager] Sauvegarde effectuée dans : {SaveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Erreur lors de la sauvegarde (File IO) : {ex}");
            }
        }

        public bool Load(out GameData data)
        {
            data = null;
            if (!File.Exists(SaveFilePath))
            {
                Debug.Log("[SaveManager] Aucune sauvegarde trouvée.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                data = JsonUtility.FromJson<GameData>(json);
                Debug.Log("[SaveManager] Données chargées depuis le fichier de sauvegarde.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Erreur lors du chargement (File IO) : {ex}");
                return false;
            }
        }

        private void TryLoadAndApply()
        {
            if (!Load(out var data))
                return;

            var crop = UnityEngine.Object.FindFirstObjectByType<CropState>();
            if (crop == null)
            {
                Debug.LogWarning("[SaveManager] CropState introuvable lors de l'application de la sauvegarde.");
                return;
            }

            crop.LoadState(data);
            Debug.Log("[SaveManager] État appliqué au CropState.");

            var currency = UnityEngine.Object.FindFirstObjectByType<Currency>();
            if (currency != null)
            {
                currency.SetBalance(data.CurrencyBalance);
                Debug.Log($"[SaveManager] Solde appliqué au Currency: {data.CurrencyBalance}");
            }

            var experienceManager = ExperienceManager.Instance;
            if (experienceManager != null && data.ExperienceData != null)
            {
                experienceManager.LoadExperienceData(data.ExperienceData);
                Debug.Log("[SaveManager] Données d'expérience appliquées.");
            }
        }

        private void OnApplicationQuitting()
        {
            SaveNow();
        }

        /// Réinitialise complètement la sauvegarde.
        public void ResetSave()
        {
            try
            {
                // Réinitialiser les composants EN MÉMOIRE en premier
                var experienceManager = ExperienceManager.Instance;
                if (experienceManager != null)
                {
                    experienceManager.ClearAllExperience();
                    Debug.Log("[SaveManager] Données d'expérience réinitialisées à 0.");
                }
                else
                {
                    Debug.LogWarning("[SaveManager] ExperienceManager non disponible pour reset.");
                }

                var crop = UnityEngine.Object.FindFirstObjectByType<CropState>();
                if (crop != null)
                {
                    crop.ResetCycle();
                    Debug.Log("[SaveManager] CropState réinitialisé.");
                }
                else
                {
                    Debug.LogWarning("[SaveManager] CropState non trouvé pour reset.");
                }

                var currency = UnityEngine.Object.FindFirstObjectByType<Currency>();
                if (currency != null)
                {
                    currency.SetBalance(0.0f);
                    Debug.Log($"[SaveManager] Solde réinitialisé à: 0.0");
                }
                else
                {
                    Debug.LogWarning("[SaveManager] Currency non trouvée pour reset.");
                }

                // Supprimer le fichier de sauvegarde APRÈS réinitialisation des variables
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log($"[SaveManager] Fichier de sauvegarde supprimé: {SaveFilePath}");
                }

                // Sauvegarder les données réinitialisées (état vierge)
                SaveNow();

                Debug.Log("[SaveManager] ✅ Sauvegarde complètement réinitialisée!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Erreur lors de la réinitialisation: {ex}");
            }
        }

        /// Affiche l'état actuel de la sauvegarde dans la console.
        public void PrintSaveState()
        {
            if (!Load(out var data))
            {
                Debug.Log("[SaveManager] Aucune sauvegarde disponible.");
                return;
            }

            Debug.Log("========== ÉTAT DE LA SAUVEGARDE ==========");
            Debug.Log($"Stage Index: {data.CurrentStageIndex}");
            Debug.Log($"Stage Timer: {data.StageTimer:F2}s");
            Debug.Log($"Currency Balance: {data.CurrencyBalance}");
            
            if (data.ExperienceData != null && data.ExperienceData.crops.Length > 0)
            {
                foreach (var crop in data.ExperienceData.crops)
                {
                    Debug.Log($"\n  Crop: {crop.cropName}");
                    Debug.Log($"    Total XP: {crop.totalXP}");
                    Debug.Log($"    Total Level: {crop.totalLevel}");
                    Debug.Log($"    Stages: {crop.stageExperiences.Length}");
                }
            }
            Debug.Log("==========================================");
        }

        /// Modifie le solde dans la sauvegarde.
        public void SetBalance(double newBalance)
        {
            if (!Load(out var data))
            {
                data = new GameData();
            }

            data.CurrencyBalance = newBalance;
            
            var currency = UnityEngine.Object.FindFirstObjectByType<Currency>();
            if (currency != null)
            {
                currency.SetBalance(newBalance);
            }

            SaveGameData(data);
            Debug.Log($"[SaveManager] Solde modifié à: {newBalance}");
        }

        /// Modifie l'étape de production dans la sauvegarde.
        public void SetStageIndex(int stageIndex)
        {
            if (!Load(out var data))
            {
                data = new GameData();
            }

            data.CurrentStageIndex = Mathf.Clamp(stageIndex, 0, 5);
            data.StageTimer = 0f;

            SaveGameData(data);
            Debug.Log($"[SaveManager] Étape modifiée à: {(ProductionStage)data.CurrentStageIndex}");
        }

        /// Réinitialise l'XP d'une culture spécifique.
        public void ResetCropExperience(string cropName)
        {
            var experienceManager = ExperienceManager.Instance;
            if (experienceManager != null)
            {
                experienceManager.ClearCropExperience(cropName);
                SaveNow();
                Debug.Log($"[SaveManager] XP de '{cropName}' réinitialisé.");
            }
        }

        private void SaveGameData(GameData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[SaveManager] Sauvegarde modifiée et écrite.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Erreur lors de la sauvegarde: {ex}");
            }
        }
    }
}