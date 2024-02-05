using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData item;

    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", item.displayName); // 프롬프트 받아오도록
    }

    public void OnInteract()
    {
        Inventory.instance.AddItem(item);
        Destroy(gameObject); // 상호작용 했을 때 어떻게 동작할지
    }
}
