using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int ExpReward = 3;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    public int currentHealth;
    public int maxHealth;

    [Header("Loot")]
    public GameObject LootPrefab;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }

        if (currentHealth <= 0)
        {
            // Выдача опыта (без ошибки, если никто не подписан на событие)
            OnMonsterDefeated?.Invoke(ExpReward);

            DropLootAtDeathPosition();

            Destroy(gameObject);
        }
    }

    private void DropLootAtDeathPosition()
    {
        if (LootPrefab == null) return;

        // Спавним лут прямо на месте смерти врага
        GameObject loot = Instantiate(LootPrefab, transform.position, transform.rotation);

        // Переводим лут на слой Default (0)
        loot.layer = LayerMask.NameToLayer("Default");

        // Если у лута есть дочерние объекты — тоже переводим их на Default
        foreach (Transform child in loot.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
