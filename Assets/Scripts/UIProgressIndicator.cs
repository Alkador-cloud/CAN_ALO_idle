using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IdleGame;

public class UIProgressIndicator : MonoBehaviour
{
    [SerializeField] private CropState cropState;
    [SerializeField] private Currency currency;
    
    // Affichage du solde
    [SerializeField] private TextMeshProUGUI balanceDisplay;
    
    // Affichage de l'étape
    [SerializeField] private TextMeshProUGUI currentStageDisplay;
    
    // Affichage de la progression
    [SerializeField] private TextMeshProUGUI progressionPercentageDisplay;
    
    // Barre de progression (optionnel)
    [SerializeField] private Image progressBar;

    void Start()
    {
        ValidateReferences();
        SubscribeToEvents();
        UpdateAllDisplays();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void ValidateReferences()
    {
        if (cropState == null)
            Debug.LogError("CropState non assigné à UIProgressIndicator", gameObject);
        
        if (currency == null)
            Debug.LogError("Currency non assignée à UIProgressIndicator", gameObject);
        
        if (balanceDisplay == null)
            Debug.LogWarning("Affichage du solde non assigné à UIProgressIndicator", gameObject);
        
        if (currentStageDisplay == null)
            Debug.LogWarning("Affichage de l'étape non assigné à UIProgressIndicator", gameObject);
        
        if (progressionPercentageDisplay == null)
            Debug.LogWarning("Affichage du pourcentage non assigné à UIProgressIndicator", gameObject);
    }

    private void SubscribeToEvents()
    {
        if (currency != null)
            currency.OnBalanceChanged += UpdateBalanceDisplay;
        
        if (cropState != null)
        {
            cropState.OnStageChanged += OnStageChanged;
            cropState.OnStageDurationUpdated += UpdateProgressionDisplay;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (currency != null)
            currency.OnBalanceChanged -= UpdateBalanceDisplay;
        
        if (cropState != null)
        {
            cropState.OnStageChanged -= OnStageChanged;
            cropState.OnStageDurationUpdated -= UpdateProgressionDisplay;
        }
    }

    private void UpdateAllDisplays()
    {
        UpdateBalanceDisplay(currency?.CurrentBalance ?? 0);
        UpdateCurrentStageDisplay();
        UpdateProgressionDisplay();
    }

    private void UpdateBalanceDisplay(double balance)
    {
        if (balanceDisplay != null)
            balanceDisplay.text = $"Solde: {balance:F2}€";
    }

    private void OnStageChanged(ProductionStage stage)
    {
        UpdateCurrentStageDisplay();
        UpdateProgressionDisplay();
    }

    private void UpdateCurrentStageDisplay()
    {
        if (currentStageDisplay == null || cropState == null)
            return;
        
        currentStageDisplay.text = $"Étape: {cropState.GetStageName()}";
    }

    private void UpdateProgressionDisplay()
    {
        if (cropState == null)
            return;
        
        float progressionPercentage = cropState.ProgressionPercentage * 100f;
        
        if (progressionPercentageDisplay != null)
            progressionPercentageDisplay.text = $"Progression: {progressionPercentage:F1}%";
        
        if (progressBar != null)
            progressBar.fillAmount = cropState.ProgressionPercentage;
    }
}