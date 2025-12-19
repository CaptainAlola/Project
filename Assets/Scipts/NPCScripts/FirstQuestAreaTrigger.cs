using UnityEngine;
using UnityEngine.UI;
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
    public GameObject quest_second_area_1;
    public GameObject quest_second_area_1_2;
    public GameObject tree_second_area_1_2;

    public string areaTag = "Second_Quest_Area";
    public string SecondAreaTag = "Fourth_Quest_Area";

    private bool mushroomsCleared = false;
    private bool mushroomsBrownCleared = false;
    private bool branchesCleared = false;

    private bool isPlayerInside = false;
    private bool dialogueOpen = false;
    private GameObject currentWindow;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);

        SetActiveSafe(quest1, false);
        SetActiveSafe(quest2, false);
        SetActiveSafe(quest2_2, false);
        SetActiveSafe(quest3, false);
        SetActiveSafe(quest3_3, false);
        SetActiveSafe(quest_second_area_1, false);
        SetActiveSafe(quest_second_area_1_2, false);

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

        if (!mushroomsBrownCleared && !branchesCleared && GameObject.FindGameObjectsWithTag("Mushroom_brown").Length == 0 && GameObject.FindGameObjectsWithTag("branch").Length == 0)
        {
            mushroomsBrownCleared = true;
            branchesCleared = true;
            if (quest_second_area_1 != null) Destroy(quest_second_area_1);
        }
    }

    private void OpenDialogue()
    {
        dialogueOpen = true;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (quest1 != null && !quest1.activeSelf) currentWindow = quest1;
        else if (quest2 != null && !quest2.activeSelf) currentWindow = quest2;
        else if (quest3 != null && !quest3.activeSelf) currentWindow = quest3;
        else if (quest_second_area_1 != null && !quest_second_area_1.activeSelf) currentWindow = quest_second_area_1;

        SetActiveSafe(currentWindow, true);
        WireCloseButtons(currentWindow);

        // Если это Second_Quest_Area и все грибы собраны — удаляем их из инвентаря
        if (gameObject.CompareTag(areaTag) && mushroomsCleared)
        {
            var inv = Object.FindFirstObjectByType<InventoryManager>();
            if (inv != null)
                inv.RemoveAllMushrooms();

            if (quest2_2 != null && !quest2_2.activeSelf)
            {
                quest2_2.SetActive(true);
                WireCloseButtons(quest2_2);
                StartCoroutine(DestroyQuest2_2AfterDelay());
            }
        }

        if (gameObject.CompareTag(SecondAreaTag) && mushroomsBrownCleared && branchesCleared)
        {
            var inv = Object.FindFirstObjectByType<InventoryManager>();
            if (inv != null)
            {
                inv.RemoveAllMushroomsBrown();
                inv.RemoveAllBranches();
            }

            if (quest_second_area_1_2 != null && !quest_second_area_1_2.activeSelf)
            {
                quest_second_area_1_2.SetActive(true);
                WireCloseButtons(quest_second_area_1_2);
                StartCoroutine(DestroyTree_AfterCompleteQuest());
            }
        }
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;

        if (currentWindow != null) currentWindow.SetActive(false);
        currentWindow = null;

        SetActiveSafe(quest1, false);
        SetActiveSafe(quest2, false);
        SetActiveSafe(quest2_2, false);
        SetActiveSafe(quest3, false);
        SetActiveSafe(quest3_3, false);
        SetActiveSafe(quest_second_area_1, false);
        SetActiveSafe(quest_second_area_1_2, false);


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

    private IEnumerator DestroyTree_AfterCompleteQuest()
    {
        yield return new WaitForSeconds(2f);
        if (quest_second_area_1_2 != null) Destroy(quest_second_area_1_2);

        if (tree_second_area_1_2 != null)
        {
            var sprite = tree_second_area_1_2.GetComponent<SpriteRenderer>();
            var color = sprite.color;
            int steps = 10;
            float delay = 0.2f;
            for (int i = 0; i < steps; i++)
            {
                color.a = 1f-(i+1)/(float)steps;
                sprite.color = color;
                yield return new WaitForSeconds(delay);
            }

            Destroy(tree_second_area_1_2);
        }
    }

    private void WireCloseButtons(GameObject windowRoot)
    {
        if (windowRoot == null) return;

        var buttons = windowRoot.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(CloseDialogue);
        }
    }

    private void SetActiveSafe(GameObject go, bool state)
    {
        if (go != null) go.SetActive(state);
    }

    private void EnsureUIPrereqs()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            Debug.LogWarning("В сцене нет EventSystem — кнопки не будут получать клики.");
    }
}