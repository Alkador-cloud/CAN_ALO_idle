using UnityEngine;
using UnityEngine.InputSystem;
using IdleGame;

public class ExperienceDebugger : MonoBehaviour
{
    [SerializeField] private CropState cropState;
    [SerializeField] private CropType cropType;
    private ExperienceManager experienceManager;
    private string currentCropName;
    private int lastStageXPCount = 0;
    private int lastTotalXPCount = 0;

    private void Start()
    {
        Debug.Log("[ExperienceDebugger] Initialisation du débogage XP");
        
        experienceManager = ExperienceManager.Instance;
        currentCropName = cropType != null ? cropType.CropName : "Unknown";

        if (experienceManager == null)
        {
            Debug.LogError("[ExperienceDebugger] ExperienceManager est NULL!");
            return;
        }

        Debug.Log($"[ExperienceDebugger] ExperienceManager trouvé: {experienceManager.gameObject.name}");

        // Initialiser la culture
        experienceManager.InitializeCrop(currentCropName);
        Debug.Log($"[ExperienceDebugger] Culture initialisée: {currentCropName}");

        // S'abonner aux événements avec logs détaillés
        experienceManager.OnStageXPGained += (cropName, xpType, amount) =>
        {
            Debug.Log($"[ExperienceDebugger] EVENT DÉCLENCHÉ - OnStageXPGained: crop={cropName}, type={xpType}, amount={amount}");
            lastStageXPCount++;
        };

        experienceManager.OnTotalXPGained += (cropName, amount) =>
        {
            Debug.Log($"[ExperienceDebugger] EVENT DÉCLENCHÉ - OnTotalXPGained: crop={cropName}, amount={amount}");
            lastTotalXPCount++;
        };

        experienceManager.OnStageLevelUp += (cropName, xpType, level) =>
        {
            Debug.Log($"[ExperienceDebugger] EVENT DÉCLENCHÉ - OnStageLevelUp: crop={cropName}, type={xpType}, level={level}");
        };

        experienceManager.OnLevelUp += (cropName, level) =>
        {
            Debug.Log($"[ExperienceDebugger] EVENT DÉCLENCHÉ - OnLevelUp: crop={cropName}, level={level}");
        };

        if (cropState != null)
        {
            cropState.OnStageChanged += (stage) =>
            {
                Debug.Log($"[ExperienceDebugger] Stage changé: {stage}");
            };
        }
        else
        {
            Debug.LogWarning("[ExperienceDebugger] CropState est NULL!");
        }

        Debug.Log("[ExperienceDebugger] Tous les événements abonnés");
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        // Appuyer sur 'X' pour tester manuellement un gain d'XP
        if (keyboard.xKey.wasPressedThisFrame)
        {
            TestManualXPGain();
        }

        // Appuyer sur 'P' pour afficher l'état actuel
        if (keyboard.pKey.wasPressedThisFrame)
        {
            PrintCurrentState();
        }
    }

    private void TestManualXPGain()
    {
        if (experienceManager == null)
        {
            Debug.LogError("[ExperienceDebugger] Impossible de tester - ExperienceManager est NULL");
            return;
        }

        Debug.Log($"[ExperienceDebugger] TEST MANUEL - Ajout de 50 XP à {currentCropName} - Planting");
        experienceManager.GainStageXP(currentCropName, XPType.Planting, 50);

        PrintCurrentState();
    }

    private void PrintCurrentState()
    {
        if (experienceManager == null)
        {
            Debug.LogError("[ExperienceDebugger] Impossible d'afficher l'état - ExperienceManager est NULL");
            return;
        }

        var cropExp = experienceManager.GetCropExperience(currentCropName);
        Debug.Log($"[ExperienceDebugger] === ÉTAT ACTUEL DE {currentCropName} ===");
        Debug.Log($"  Total XP: {cropExp.TotalXP}");
        Debug.Log($"  Total Level: {cropExp.TotalLevel}");
        Debug.Log($"  Nombre d'étapes: {cropExp.StageExperiences.Count}");
        
        foreach (var xpType in cropExp.StageExperiences.Keys)
        {
            var stageExp = cropExp.StageExperiences[xpType];
            Debug.Log($"  - {xpType}: XP={stageExp.CurrentXP}, Level={stageExp.CurrentLevel}");
        }

        Debug.Log($"[ExperienceDebugger] Total events OnStageXPGained: {lastStageXPCount}");
        Debug.Log($"[ExperienceDebugger] Total events OnTotalXPGained: {lastTotalXPCount}");
    }
}