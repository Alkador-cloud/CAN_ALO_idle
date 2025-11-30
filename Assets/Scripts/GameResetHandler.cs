using UnityEngine;
using UnityEngine.InputSystem;

namespace IdleGame
{
    public class GameResetHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log("[GameResetHandler] Script activé et prêt à écouter les entrées clavier.");
        }

        private void OnDisable()
        {
            Debug.Log("[GameResetHandler] Script désactivé.");
        }

        private void Update()
        {
            if (Keyboard.current == null)
            {
                Debug.LogWarning("[GameResetHandler] Keyboard non disponible! L'InputSystem n'est peut-être pas activé.");
                return;
            }

            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                Debug.Log("[GameResetHandler] ✅ Touche N détectée!");
                ExecuteReset();
            }
        }

        private void ExecuteReset()
        {
            Debug.Log("[GameResetHandler] Réinitialisation du jeu via touche N...");

            if (SaveManager.Instance == null)
            {
                Debug.LogError("[GameResetHandler] SaveManager.Instance est null!");
                return;
            }

            SaveManager.Instance.ResetSave();
        }
    }
}