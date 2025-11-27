using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IdleGame;

public class PlantClickHandler : MonoBehaviour
{
    [SerializeField] private CropState cropState;
    [SerializeField] private Image plantImage;
    [SerializeField] private float clickFeedbackDuration = 0.25f;
    [SerializeField] private Color clickFeedbackColor = Color.yellow;
    [SerializeField] private Button autoProgressToggleButton;
    [SerializeField] private TextMeshProUGUI autoProgressToggleButtonText;

    private Color originalColor;
    private Button plantButton;
    private bool isAutoProgressEnabled = true;

    void Start()
    {
        ValidateReferences();
        SetupButton();
        SetupAutoProgressToggle();
        originalColor = plantImage.color;
    }

    void Update()
    {
        
    }

    private void ValidateReferences()
    {
        if (cropState == null)
        {
            Debug.LogError("CropState reference manquante sur PlantClickHandler", gameObject);
        }

        if (plantImage == null)
        {
            plantImage = GetComponent<Image>();
            if (plantImage == null)
            {
                Debug.LogError("Aucun composant Image trouvé sur le bouton Plant", gameObject);
            }
        }
    }

    private void SetupButton()
    {
        plantButton = GetComponent<Button>();
        if (plantButton == null)
        {
            Debug.LogError("PlantClickHandler doit être attaché à un bouton UI Button", gameObject);
            return;
        }

        plantButton.onClick.AddListener(OnPlantClicked);
    }

    private void SetupAutoProgressToggle()
    {
        if (autoProgressToggleButton == null)
        {
            Debug.LogWarning("Bouton de désactivation du timer auto non assigné", gameObject);
            return;
        }

        autoProgressToggleButton.onClick.AddListener(OnToggleAutoProgress);
        UpdateToggleButtonDisplay();
    }

    public void OnPlantClicked()
    {
        if (cropState == null)
            return;

        // Progression manuelle au clic
        cropState.IncreaseProgressManually();

        // Feedback visuel
        StartCoroutine(PlayClickFeedback());
    }

    public void OnToggleAutoProgress()
    {
        isAutoProgressEnabled = !isAutoProgressEnabled;
        cropState.SetIsProgressing(isAutoProgressEnabled);
        UpdateToggleButtonDisplay();

        Debug.Log($"[PlantClickHandler] Auto-progression: {(isAutoProgressEnabled ? "Activée" : "Désactivée")}");
    }

    private void UpdateToggleButtonDisplay()
    {
        if (autoProgressToggleButtonText != null)
        {
            autoProgressToggleButtonText.text = isAutoProgressEnabled ? "Pause" : "Reprendre";
        }
    }

    private System.Collections.IEnumerator PlayClickFeedback()
    {
        plantImage.color = clickFeedbackColor;
        yield return new WaitForSeconds(clickFeedbackDuration);
        plantImage.color = originalColor;
    }
}