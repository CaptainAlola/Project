using TMPro;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class InventoryManager : MonoBehaviour
{
    public int gold;
    public TMP_Text goldText;
    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }


    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }
        else 
        {
            UpdateUI();
        }
    }

}
