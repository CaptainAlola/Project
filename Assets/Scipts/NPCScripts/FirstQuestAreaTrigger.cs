using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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

    [Header("Квест кузнеца")]
    public string blacksmithAreaTag = "Fifth_Quest_Area";
    public string oreItemName = "IronOre"; // <-- ВАЖНО: поставь точное имя из ItemSO.itemName
    public int oreNeed = 8;

    public GameObject quest_second_area_2;     // начальный диалог
    public GameObject quest_second_area_2_2;   // финальный диалог
    public GameObject blacksmithObject;        // кузнец в сцене
    public GameObject notePrefab;              // префаб записки

    private bool blacksmithQuestCompleted = false;

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

        // ? НОВОЕ
        SetActiveSafe(quest_second_area_2, false);
        SetActiveSafe(quest_second_area_2_2, false);

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

        if (!mushroomsBrownCleared && !branchesCleared
            && GameObject.FindGameObjectsWithTag("Mushroom_brown").Length == 0
            && GameObject.FindGameObjectsWithTag("branch").Length == 0)
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

        // Базовый выбор окна (как у тебя)
        if (quest1 != null && !quest1.activeSelf) currentWindow = quest1;
        else if (quest2 != null && !quest2.activeSelf) currentWindow = quest2;
        else if (quest3 != null && !quest3.activeSelf) currentWindow = quest3;
        else if (quest_second_area_1 != null && !quest_second_area_1.activeSelf) currentWindow = quest_second_area_1;

        // ? НОВОЕ: если это зона кузнеца — используем его окна, а не общие
        if (gameObject.CompareTag(blacksmithAreaTag))
        {
            HandleBlacksmithQuest();
            return; // важно: чтобы не включалось старое окно ниже
        }

        SetActiveSafe(currentWindow, true);
        WireCloseButtons(currentWindow);

        // Твой квест с грибами (Second_Quest_Area)
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

        // Твой квест с деревом (Fourth_Quest_Area)
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

    // ==========================
    // ? КВЕСТ КУЗНЕЦА
    // ==========================
    private void HandleBlacksmithQuest()
    {
        if (blacksmithQuestCompleted)
        {
            CloseDialogue();
            return;
        }

        var inv = Object.FindFirstObjectByType<InventoryManager>();
        if (inv == null)
        {
            // если инвентарь не найден — показываем стартовый диалог
            quest_second_area_2.SetActive(true);
            WireCloseButtons(quest_second_area_2);
            return;
        }

        int oreCount = inv.CountItemByName(oreItemName);

        if (oreCount < oreNeed)
        {
            quest_second_area_2.SetActive(true);
            WireCloseButtons(quest_second_area_2);
            return;
        }

        // хватает руды -> снимаем 8
        bool removed = inv.RemoveItemByName(oreItemName, oreNeed);
        if (!removed)
        {
            // на всякий случай
            quest_second_area_2.SetActive(true);
            WireCloseButtons(quest_second_area_2);
            return;
        }

        // финальный диалог
        quest_second_area_2_2.SetActive(true);
        WireCloseButtons(quest_second_area_2_2);

        blacksmithQuestCompleted = true;
        StartCoroutine(CompleteBlacksmithQuestSequence());
    }

    private IEnumerator CompleteBlacksmithQuestSequence()
    {
        yield return new WaitForSeconds(2f);

        // убираем кузнеца
        Vector3 pos = blacksmithObject != null ? blacksmithObject.transform.position : transform.position;
        if (blacksmithObject != null) Destroy(blacksmithObject);

        // оставляем записку
        if (notePrefab != null) Instantiate(notePrefab, pos, Quaternion.identity);

        // можно уничтожить окна, как ты делал в других квестах
        if (quest_second_area_2_2 != null) Destroy(quest_second_area_2_2);
        if (quest_second_area_2_2 != null) Destroy(quest_second_area_2_2);

        CloseDialogue();
    }
    // ==========================
    // Твой существующий код
    // ==========================
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

        // ? НОВОЕ
        SetActiveSafe(quest_second_area_2, false);
        SetActiveSafe(quest_second_area_2_2, false);

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
                color.a = 1f - (i + 1) / (float)steps;
                sprite.color = color;
                yield return new WaitForSeconds(delay);
            }

            Destroy(tree_second_area_1_2);
            gameObject.SetActive(false);
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

    // ==========================
    // ? Универсальный подсчёт/удаление Ore из инвентаря
    // ==========================

    private int CountInventoryItemsWithTag(InventoryManager inv, string tag)
    {
        if (inv == null) return 0;

        // 1) Попробуем найти поле items (List<GameObject> / List<Transform>)
        var t = inv.GetType();
        var itemsField = t.GetField("items", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (itemsField != null)
        {
            object val = itemsField.GetValue(inv);

            if (val is IList list)
            {
                int c = 0;
                foreach (var obj in list)
                {
                    if (obj is GameObject go && go != null && go.CompareTag(tag)) c++;
                    else if (obj is Transform tr && tr != null && tr.CompareTag(tag)) c++;
                    else if (obj is Component comp && comp != null && comp.CompareTag(tag)) c++;
                }
                return c;
            }
        }

        // 2) Попробуем itemsRoot (Transform)
        var rootField = t.GetField("itemsRoot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (rootField != null)
        {
            object val = rootField.GetValue(inv);
            if (val is Transform root && root != null)
            {
                int c = 0;
                foreach (Transform child in root)
                    if (child != null && child.CompareTag(tag)) c++;
                return c;
            }
        }

        Debug.LogWarning("Не смог найти список предметов в InventoryManager. " +
                         "Добавь поле items (List<GameObject>) или itemsRoot (Transform), " +
                         "чтобы квест кузнеца мог посчитать Ore.");
        return 0;



    }

    private void RemoveInventoryItemsWithTag(InventoryManager inv, string tag, int amount)
    {
        if (inv == null) return;

        var t = inv.GetType();

        // 1) items (IList) — удаляем элементы из списка (как “8 штук”)
        var itemsField = t.GetField("items", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (itemsField != null)
        {
            object val = itemsField.GetValue(inv);
            if (val is IList list)
            {
                int left = amount;
                for (int i = list.Count - 1; i >= 0 && left > 0; i--)
                {
                    var obj = list[i];

                    GameObject go = null;
                    if (obj is GameObject g) go = g;
                    else if (obj is Transform tr) go = tr.gameObject;
                    else if (obj is Component comp) go = comp.gameObject;

                    if (go != null && go.CompareTag(tag))
                    {
                        list.RemoveAt(i);
                        left--;
                    }
                }

                // если в инвентаре есть метод Refresh/UpdateUI — попробуем вызвать
                TryInvoke(inv, "Refresh");
                TryInvoke(inv, "UpdateUI");
                TryInvoke(inv, "Rebuild");
                return;
            }
        }

        // 2) itemsRoot — удаляем дочерние GameObject с нужным тегом
        var rootField = t.GetField("itemsRoot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (rootField != null)
        {
            object val = rootField.GetValue(inv);
            if (val is Transform root && root != null)
            {
                int left = amount;
                for (int i = root.childCount - 1; i >= 0 && left > 0; i--)
                {
                    var child = root.GetChild(i);
                    if (child != null && child.CompareTag(tag))
                    {
                        Destroy(child.gameObject);
                        left--;
                    }
                }
                return;
            }
        }

        Debug.LogWarning("Не смог удалить Ore из InventoryManager: не найдено items/itemsRoot.");
    }

    private void TryInvoke(object obj, string methodName)
    {
        var m = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (m != null) m.Invoke(obj, null);
    }
}
