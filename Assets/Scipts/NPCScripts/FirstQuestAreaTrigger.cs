using UnityEngine;

public class QuestAreaTrigger : MonoBehaviour
{
    public GameObject questText; // Ссылка на объект текста
    public GameObject quest1;    // Ссылка на объект Quest1
    public GameObject quest2;    // Ссылка на объект Quest2
    public GameObject quest2_2;  // Ссылка на объект Quest2.2
    public GameObject quest3;    // Ссылка на объект Quest3
    public GameObject quest3_3;  // Ссылка на объект Quest3.3
    public string areaTag = "Second_Quest_Area"; // Тег для Second_Quest_Area

    private bool mushroomsCleared = false; // Флаг, показывающий, уничтожены ли все Mushroom

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Проверяем, что вошел игрок
        {
            if (questText != null)
            {
                questText.SetActive(true); // Показываем текст
            }

            // Проверяем, что игрок вошел в Second_Quest_Area и все Mushroom уничтожены
            if (gameObject.CompareTag(areaTag) && mushroomsCleared)
            {
                if (quest2_2 != null && !quest2_2.activeSelf)
                {
                    quest2_2.SetActive(true); // Активируем Quest2.2
                    StartCoroutine(DestroyQuest2_2AfterDelay()); // Запускаем корутину для уничтожения
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Проверяем, что игрок вышел
        {
            if (questText != null)
            {
                questText.SetActive(false); // Скрываем текст
            }

            if (quest1 != null) // Уничтожаем Quest1 при выходе из зоны
            {
                Destroy(quest1);
            }
        }
    }

    private void Update()
    {
        // Проверяем, остались ли объекты с тегом "Mushroom"
        if (!mushroomsCleared && GameObject.FindGameObjectsWithTag("Mushroom").Length == 0)
        {
            mushroomsCleared = true; // Устанавливаем флаг, что все Mushroom уничтожены

            if (quest2 != null)
            {
                Destroy(quest2); // Уничтожаем Quest2
            }
        }
    }

    private System.Collections.IEnumerator DestroyQuest2_2AfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (quest2_2 != null)
        {
            Destroy(quest2_2); // Уничтожаем Quest2.2
        }

        if (quest3 != null)
        {
            Destroy(quest3); // Уничтожаем Quest3
        }

        if (quest3_3 != null && !quest3_3.activeSelf)
        {
            quest3_3.SetActive(true); // Активируем Quest3.3

            // Включаем скрипт Player_Bow на игроке
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player_Bow playerBow = player.GetComponent<Player_Bow>();
                if (playerBow != null)
                {
                    playerBow.enabled = true; // Включаем скрипт
                    Debug.Log("Player_Bow включен на игроке");
                }
                else
                {
                    Debug.LogWarning("Player_Bow не найден на объекте Player");
                }
            }
            else
            {
                Debug.LogWarning("Игрок с тегом Player не найден");
            }

            // Ждем 3 секунды перед уничтожением Quest3.3
            yield return new WaitForSeconds(3f);
            Destroy(quest3_3); // Уничтожаем Quest3.3
            Debug.Log("Quest3.3 уничтожен");
        }
    }
}