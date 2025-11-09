using UnityEngine;
using UnityEngine.UI;     // << важно
using System.Collections;

public class QuestAreaTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactPrompt;
    public KeyCode interactKey = KeyCode.F;

    [Header("Quest окна (диалоги)")]
    public GameObject quest1;
    public GameObject quest2;
    public GameObject quest2_2;
    public GameObject quest3;
    public GameObject quest3_3;

    public string areaTag = "Second_Quest_Area";

    private bool mushroomsCleared = false;
    private bool isPlayerInside = false;
    private bool dialogueOpen = false;
    private GameObject currentWindow;   // активное окно, для отключения и отвязки

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // диалоговые окна по умолчанию скрыты
        SetActiveSafe(quest1, false);
        SetActiveSafe(quest2, false);
        SetActiveSafe(quest2_2, false);
        SetActiveSafe(quest3, false);
        SetActiveSafe(quest3_3, false);

        // базовые проверки на EventSystem/GraphicRaycaster
        EnsureUIPrereqs();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        isPlayerInside = true;

        if (!dialogueOpen && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        isPlayerInside = false;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        // старое поведение: уничтожить quest1 при выходе
        if (quest1 != null) Destroy(quest1);
    }

    private void Update()
    {
        if (isPlayerInside && !dialogueOpen && Input.GetKeyDown(interactKey))
        {
            OpenDialogue();
        }

        if (!mushroomsCleared && GameObject.FindGameObjectsWithTag("Mushroom").Length == 0)
        {
            mushroomsCleared = true;
            if (quest2 != null) Destroy(quest2);
        }
    }

    private void OpenDialogue()
    {
        dialogueOpen = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // Включаем нужное окно для этой зоны (как у тебя задумано)
        if (quest1 != null && !quest1.activeSelf) currentWindow = quest1;
        else if (quest2 != null && !quest2.activeSelf) currentWindow = quest2;
        else if (quest3 != null && !quest3.activeSelf) currentWindow = quest3;

        SetActiveSafe(currentWindow, true);

        // >>> Автовязка кнопок внутри выбранного окна
        WireCloseButtons(currentWindow);

        // Спец-логика Second_Quest_Area
        if (gameObject.CompareTag(areaTag) && mushroomsCleared)
        {
            if (quest2_2 != null && !quest2_2.activeSelf)
            {
                quest2_2.SetActive(true);
                WireCloseButtons(quest2_2);
                StartCoroutine(DestroyQuest2_2AfterDelay());
            }
        }
    }

    /// <summary>Вызывается кнопкой внутри окна, но мы ещё и автовешаем её сами.</summary>
    public void CloseDialogue()
    {
        dialogueOpen = false;

        // Выключаем текущее окно
        if (currentWindow != null) currentWindow.SetActive(false);
        currentWindow = null;

        // На всякий случай выключим и остальные окна
        SetActiveSafe(quest1, false);
        SetActiveSafe(quest2, false);
        SetActiveSafe(quest2_2, false);
        SetActiveSafe(quest3, false);
        SetActiveSafe(quest3_3, false);

        // Если игрок ещё в зоне — снова показать подсказку F
        if (isPlayerInside && interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private IEnumerator DestroyQuest2_2AfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (quest2_2 != null) Destroy(quest2_2);
        if (quest3 != null) Destroy(quest3);

        if (quest3_3 != null && !quest3_3.activeSelf)
        {
            quest3_3.SetActive(true);
            WireCloseButtons(quest3_3);

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var bow = player.GetComponent<Player_Bow>();
                if (bow != null) bow.enabled = true;
            }

            yield return new WaitForSeconds(3f);
            Destroy(quest3_3);
        }
    }

    // --------- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ----------

    private void WireCloseButtons(GameObject windowRoot)
    {
        if (windowRoot == null) return;

        // найдём ВСЕ Button внутри, даже если часть была неактивна при старте
        var buttons = windowRoot.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            // чистим старые слушатели, чтобы не набивать дубликаты
            btn.onClick.RemoveAllListeners();
            // фикс захвата переменной в лямбде не нужен — у всех кнопок действие одно:
            btn.onClick.AddListener(CloseDialogue);
        }
    }

    private void SetActiveSafe(GameObject go, bool state)
    {
        if (go != null) go.SetActive(state);
    }

    private void EnsureUIPrereqs()
    {
        // EventSystem должен быть в сцене
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            Debug.LogWarning("В сцене нет EventSystem — кнопки не будут получать клики.");

        // У корня Canvas, где лежат окна диалогов, должен быть GraphicRaycaster.
        // Если клики не проходят — проверьте, нет ли поверх фуллскрин-изображений с Raycast Target.
    }
}
