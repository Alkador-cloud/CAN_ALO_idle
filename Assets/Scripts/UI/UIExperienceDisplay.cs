using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using IdleGame;

public class UIExperienceDisplay : MonoBehaviour
{
    [System.Serializable]
    private class XPTypeDisplay
    {
        public XPType xpType;
        public TextMeshProUGUI xpValueText;
        public TextMeshProUGUI levelText;
        public Image progressBar;
    }

    [SerializeField] private CropState cropState;
    [SerializeField] private CropType cropType;
    
    // Affichage XP Total
    [SerializeField] private TextMeshProUGUI totalXPDisplay;
    [SerializeField] private TextMeshProUGUI totalLevelDisplay;
    [SerializeField] private Image totalXPProgressBar;
    
    // Affichages par Type d'XP
    [SerializeField] private List<XPTypeDisplay> xpTypeDisplays = new List<XPTypeDisplay>();
    
    private ExperienceManager experienceManager;
    private string currentCropName;
    private float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0f;
    private Dictionary<XPType, XPTypeDisplay> xpDisplayMap = new Dictionary<XPType, XPTypeDisplay>();

    private void Start()
    {
        ValidateReferences();
        InitializeExperienceManager();
        InitializeXPTypeDisplays();
        SubscribeToEvents();
        UpdateAllDisplays();
        Debug.Log("[UIExperienceDisplay] Initialisation complète");
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateAllDisplays();
            timeSinceLastUpdate = 0f;
        }
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
        Debug.Log($"[UIExperienceDisplay] Culture initialisée: {currentCropName}");
    }

    private void InitializeXPTypeDisplays()
    {
        xpDisplayMap.Clear();
        
        foreach (var xpDisplay in xpTypeDisplays)
        {
            if (xpDisplay != null)
            {
                xpDisplayMap[xpDisplay.xpType] = xpDisplay;
                ValidateXPTypeDisplay(xpDisplay);
            }
        }

        Debug.Log($"[UIExperienceDisplay] {xpDisplayMap.Count} affichages de types d'XP initialisés");
    }

    private void ValidateXPTypeDisplay(XPTypeDisplay display)
    {
        if (display.xpValueText == null)
            Debug.LogWarning($"XP Value Text non assigné pour {display.xpType}", gameObject);
        
        if (display.levelText == null)
            Debug.LogWarning($"Level Text non assigné pour {display.xpType}", gameObject);
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

        Debug.Log($"[UIExperienceDisplay] OnTotalXPGained: +{xpGained} XP");
        UpdateTotalXPDisplay();
    }

    private void OnStageXPGained(string cropName, XPType xpType, int xpGained)
    {
        if (cropName != currentCropName)
            return;

        Debug.Log($"[UIExperienceDisplay] OnStageXPGained: +{xpGained} XP pour {xpType}");
        UpdateXPTypeDisplay(xpType);
    }

    private void OnTotalLevelUp(string cropName, int newLevel)
    {
        if (cropName != currentCropName)
            return;

        UpdateTotalXPDisplay();
        Debug.Log($"[UIExperienceDisplay] {cropName} a atteint le niveau {newLevel}!");
    }

    private void OnStageLevelUp(string cropName, XPType xpType, int newLevel)
    {
        if (cropName != currentCropName)
            return;

        UpdateXPTypeDisplay(xpType);
        Debug.Log($"[UIExperienceDisplay] {cropName} - {xpType} a atteint le niveau {newLevel}!");
    }

    private void OnCropStageChanged(ProductionStage newStage)
    {
        Debug.Log($"[UIExperienceDisplay] Stage changé: {newStage}");
        XPType xpType = (XPType)newStage;
        UpdateXPTypeDisplay(xpType);
    }

    private void UpdateAllDisplays()
    {
        UpdateTotalXPDisplay();
        UpdateAllXPTypeDisplays();
    }

    private void UpdateTotalXPDisplay()
    {
        if (experienceManager == null)
            return;

        var cropExp = experienceManager.GetCropExperience(currentCropName);
        int totalThreshold = experienceManager.GetLevelThreshold(cropExp.TotalLevel + 1);
        int currentThreshold = experienceManager.GetLevelThreshold(cropExp.TotalLevel);

        if (totalXPDisplay != null)
        {
            totalXPDisplay.text = $"XP Total: {cropExp.TotalXP} / {totalThreshold}";
        }

        if (totalLevelDisplay != null)
        {
            totalLevelDisplay.text = $"Niveau: {cropExp.TotalLevel}";
        }

        if (totalXPProgressBar != null)
        {
            float xpInLevel = cropExp.TotalXP - currentThreshold;
            float xpNeededForLevel = totalThreshold - currentThreshold;
            totalXPProgressBar.fillAmount = xpNeededForLevel > 0 ? xpInLevel / xpNeededForLevel : 0f;
        }
    }

    private void UpdateAllXPTypeDisplays()
    {
        foreach (var xpType in xpDisplayMap.Keys)
        {
            UpdateXPTypeDisplay(xpType);
        }
    }

    private void UpdateXPTypeDisplay(XPType xpType)
    {
        if (experienceManager == null || !xpDisplayMap.ContainsKey(xpType))
            return;

        var display = xpDisplayMap[xpType];
        var stageExp = experienceManager.GetStageExperience(currentCropName, xpType);
        
        int nextLevelThreshold = experienceManager.GetLevelThreshold(stageExp.CurrentLevel + 1);
        int currentLevelThreshold = experienceManager.GetLevelThreshold(stageExp.CurrentLevel);

        // Mise à jour du texte XP
        if (display.xpValueText != null)
        {
            display.xpValueText.text = $"{xpType} XP: {stageExp.CurrentXP} / {nextLevelThreshold}";
        }

        // Mise à jour du texte Niveau
        if (display.levelText != null)
        {
            display.levelText.text = $"Niv. {xpType}: {stageExp.CurrentLevel}";
        }

        // Mise à jour de la barre de progression
        if (display.progressBar != null)
        {
            float xpInLevel = stageExp.CurrentXP - currentLevelThreshold;
            float xpNeededForLevel = nextLevelThreshold - currentLevelThreshold;
            display.progressBar.fillAmount = xpNeededForLevel > 0 ? xpInLevel / xpNeededForLevel : 0f;
        }
    }
}