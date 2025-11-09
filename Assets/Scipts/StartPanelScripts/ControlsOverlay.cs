using UnityEngine;
using UnityEngine.UI;

public class ControlsOverlay : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject overlayPanel; // Панель с управлением
    public Button closeButton;      // Кнопка "Понял"

    [Header("Настройки")]
    [Tooltip("Запоминать, что игрок уже видел плашку?")]
    public bool rememberChoice = true;

    private const string prefsKey = "ControlsOverlaySeen";

    void Start()
    {
        overlayPanel.SetActive(true);

        // Привязываем кнопку, если не привязана вручную
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
        else
        {
            Debug.LogWarning("Кнопка закрытия не назначена в инспекторе!");
        }
    }

    public void ClosePanel()
    {
        if (overlayPanel != null)
        {
            overlayPanel.SetActive(false);
        }

        if (rememberChoice)
        {
            PlayerPrefs.SetInt(prefsKey, 1);
            PlayerPrefs.Save();
        }
    }
}
