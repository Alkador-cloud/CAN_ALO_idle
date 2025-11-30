using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IdleGame;

public class UIExperienceDisplay : MonoBehaviour
{
    [SerializeField] private CropState cropState;
    [SerializeField] private CropType cropType;
    
    // Affichage XP Total
    [SerializeField] private TextMeshProUGUI totalXPDisplay;
    [SerializeField] private TextMeshProUGUI totalLevelDisplay;
    [SerializeField] private Image totalXPProgressBar;
    
    // Affichage XP par Étape
    [SerializeField] private TextMeshProUGUI stageXPDisplay;
    [SerializeField] private TextMeshProUGUI stageLevelDisplay;
    [SerializeField] private Image stageXPProgressBar;
    
    private ExperienceManager experienceManager;
    private string currentCropName;

    private void Start()
    {
        ValidateReferences();
        InitializeExperienceManager();
        SubscribeToEvents();
        UpdateAllDisplays();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void ValidateReferences()
    {
        if (cropState == null)
            Debug.LogError("CropState non assigné à UIExperienceDisplay", gameObject);
        
        if (cropType == null)
            Debug.LogError("CropType non assigné à UIExperienceDisplay", gameObject);
        
        if (totalXPDisplay == null)
            Debug.LogWarning("Total XP Display non assigné à UIExperienceDisplay", gameObject);
        
        if (totalLevelDisplay == null)
            Debug.LogWarning("Total Level Display non assigné à UIExperienceDisplay", gameObject);
        
        if (stageXPDisplay == null)
            Debug.LogWarning("Stage XP Display non assigné à UIExperienceDisplay", gameObject);
        
        if (stageLevelDisplay == null)
            Debug.LogWarning("Stage Level Display non assigné à UIExperienceDisplay", gameObject);
    }

    private void InitializeExperienceManager()
    {
        experienceManager = ExperienceManager.Instance;
        if (experienceManager == null)
        {
            Debug.LogError("[UIExperienceDisplay] ExperienceManager non trouvé");
            return;
        }

        currentCropName = cropType != null ? cropType.CropName : "Unknown";
        experienceManager.InitializeCrop(currentCropName);
    }

    private void SubscribeToEvents()
    {
        if (experienceManager == null)
            return;

        experienceManager.OnTotalXPGained += OnTotalXPGained;
        experienceManager.OnStageXPGained += OnStageXPGained;
        experienceManager.OnLevelUp += OnTotalLevelUp;
        experienceManager.OnStageLevelUp += OnStageLevelUp;

        if (cropState != null)
        {
            cropState.OnStageChanged += OnCropStageChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (experienceManager == null)
            return;

        experienceManager.OnTotalXPGained -= OnTotalXPGained;
        experienceManager.OnStageXPGained -= OnStageXPGained;
        experienceManager.OnLevelUp -= OnTotalLevelUp;
        experienceManager.OnStageLevelUp -= OnStageLevelUp;

        if (cropState != null)
        {
            cropState.OnStageChanged -= OnCropStageChanged;
        }
    }

    private void OnTotalXPGained(string cropName, int xpGained)
    {
        if (cropName != currentCropName)
            return;

        UpdateTotalXPDisplay();
    }

    private void OnStageXPGained(string cropName, ProductionStage stage, int xpGained)
    {
        if (cropName != currentCropName)
            return;

        UpdateStageXPDisplay();
    }

    private void OnTotalLevelUp(string cropName, int newLevel)
    {
        if (cropName != currentCropName)
            return;

        UpdateTotalXPDisplay();
        Debug.Log($"[UIExperienceDisplay] {cropName} a atteint le niveau {newLevel}!");
    }

    private void OnStageLevelUp(string cropName, ProductionStage stage, int newLevel)
    {
        if (cropName != currentCropName)
            return;

        UpdateStageXPDisplay();
        Debug.Log($"[UIExperienceDisplay] {cropName} - {stage} a atteint le niveau {newLevel}!");
    }

    private void OnCropStageChanged(ProductionStage newStage)
    {
        UpdateStageXPDisplay();
    }

    private void UpdateAllDisplays()
    {
        UpdateTotalXPDisplay();
        UpdateStageXPDisplay();
    }

    private void UpdateTotalXPDisplay()
    {
        if (experienceManager == null)
            return;

        var cropExp = experienceManager.GetCropExperience(currentCropName);
        int xpToNext = experienceManager.GetXPToNextLevel(currentCropName, isStage: false);
        int totalThreshold = experienceManager.GetLevelThreshold(cropExp.TotalLevel + 1);
        int currentThreshold = experienceManager.GetLevelThreshold(cropExp.TotalLevel);

        if (totalXPDisplay != null)
            totalXPDisplay.text = $"Total XP: {cropExp.TotalXP} / {totalThreshold}";

        if (totalLevelDisplay != null)
            totalLevelDisplay.text = $"Niveau: {cropExp.TotalLevel}";

        if (totalXPProgressBar != null)
        {
            float xpInLevel = cropExp.TotalXP - currentThreshold;
            float xpNeededForLevel = totalThreshold - currentThreshold;
            totalXPProgressBar.fillAmount = xpNeededForLevel > 0 ? xpInLevel / xpNeededForLevel : 0f;
        }
    }

    private void UpdateStageXPDisplay()
    {
        if (experienceManager == null || cropState == null)
            return;

        ProductionStage currentStage = cropState.CurrentStage;
        var stageExp = experienceManager.GetStageExperience(currentCropName, currentStage);
        int stageThreshold = experienceManager.GetLevelThreshold(stageExp.CurrentLevel + 1);
        int stageCurrentThreshold = experienceManager.GetLevelThreshold(stageExp.CurrentLevel);

        if (stageXPDisplay != null)
            stageXPDisplay.text = $"{currentStage} XP: {stageExp.CurrentXP} / {stageThreshold}";

        if (stageLevelDisplay != null)
            stageLevelDisplay.text = $"Niv. {currentStage}: {stageExp.CurrentLevel}";

        if (stageXPProgressBar != null)
        {
            float stageXpInLevel = stageExp.CurrentXP - stageCurrentThreshold;
            float stageXpNeededForLevel = stageThreshold - stageCurrentThreshold;
            stageXPProgressBar.fillAmount = stageXpNeededForLevel > 0 ? stageXpInLevel / stageXpNeededForLevel : 0f;
        }
    }
}