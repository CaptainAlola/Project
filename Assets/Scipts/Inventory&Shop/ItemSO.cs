using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite icon;

    public bool isGold;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("Stack Settings")]
    [Min(1)]
    public int maxStack = 1; // ƒл€ стакаемых предметов, например Mushroom Ч 64

    [Header("For Temporary Items")]
    public float duration;
}
