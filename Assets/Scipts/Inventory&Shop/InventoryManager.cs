using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] itemSlots;
    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;

    private void Start()
    {
        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }
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
            // Стакинг предметов
            foreach (var slot in itemSlots)
            {
                if (slot.itemSO == itemSO && slot.quantity < itemSO.maxStack)
                {
                    int spaceLeft = itemSO.maxStack - slot.quantity;
                    int toAdd = Mathf.Min(spaceLeft, quantity);
                    slot.quantity += toAdd;
                    slot.UpdateUI();
                    quantity -= toAdd;
                    if (quantity <= 0) return;
                }
            }

            // Добавление в пустой слот
            foreach (var slot in itemSlots)
            {
                if (slot.itemSO == null)
                {
                    int toAdd = Mathf.Min(itemSO.maxStack, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = toAdd;
                    slot.UpdateUI();
                    quantity -= toAdd;
                    if (quantity <= 0) return;
                }
            }
        }
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSO != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffects(slot.itemSO);

            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }

    // Новый метод для удаления всех грибов из инвентаря
    public void RemoveAllMushrooms()
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemSO != null && slot.itemSO.itemName == "Mushroom")
            {
                slot.itemSO = null;
                slot.quantity = 0;
                slot.UpdateUI();
            }
        }
    }
}

