using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IdleGame
{
    public class NotificationUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeInDuration = 0.2f;
        [SerializeField] private float fadeOutDuration = 0.3f;

        private Color infoColor = new Color(0.2f, 0.5f, 0.8f, 0.9f);
        private Color successColor = new Color(0.2f, 0.8f, 0.4f, 0.9f);
        private Color warningColor = new Color(0.9f, 0.7f, 0.2f, 0.9f);
        private Color errorColor = new Color(0.9f, 0.3f, 0.3f, 0.9f);

        private Action onNotificationComplete;
        private Coroutine displayCoroutine;

        public void Initialize(NotificationData data, Action onComplete)
        {
            messageText.text = data.Message;
            onNotificationComplete = onComplete;

            SetBackgroundColor(data.Type);
            SetupCanvasGroup();

            displayCoroutine = StartCoroutine(DisplayCoroutine(data.Duration));
        }

        private void SetBackgroundColor(NotificationType type)
        {
            Color targetColor = type switch
            {
                NotificationType.Success => successColor,
                NotificationType.Warning => warningColor,
                NotificationType.Error => errorColor,
                _ => infoColor
            };

            backgroundImage.color = targetColor;
        }

        private void SetupCanvasGroup()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
        }

        private IEnumerator DisplayCoroutine(float duration)
        {
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(duration);
            yield return StartCoroutine(FadeOut());

            onNotificationComplete?.Invoke();
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = 1;
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = 1 - Mathf.Clamp01(elapsed / fadeOutDuration);
                yield return null;
            }

            canvasGroup.alpha = 0;
        }
    }
}