using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのインベントリを管理するクラス
/// </summary>
public class Inventory : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("最大スロット数")] private int maxSlots = 20;
    
    private List<BaseItem> items = new List<BaseItem>(); // 所持アイテムリスト

    /// <summary>
    /// インベントリにアイテムを追加する
    /// </summary>
    public bool AddItem(BaseItem item)
    {
        // 同じアイテムが既に存在するかをチェック
        BaseItem existingItem = items.Find(i => i.itemName == item.itemName);

        if (existingItem != null)
        {
            // アイテムが存在して、Stack可能ならばスタック
            if (existingItem.CanStack())
            {
                existingItem.StackItem();
                Debug.Log($"アイテム {item.itemName} をスタックしました。");
                return true;
            }
            else
            {
                Debug.Log("アイテムのスタック数が最大です！");
                return false;
            }
        }
        else
        {
            // アイテムが存在しない場合、インベントリに追加
            if (items.Count < maxSlots)
            {
                items.Add(item);
                Debug.Log($"アイテム {item.itemName} を追加しました。");
                return true;
            }
            else
            {
                Debug.Log("インベントリがいっぱいです！");
                return false;
            }
        }
    }

    /// <summary>
    /// 指定したアイテムをインベントリから削除する
    /// </summary>
    public void RemoveItem(BaseItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"アイテム {item.itemName} を削除しました。");
        }
    }

    /// <summary>
    /// インベントリ内のアイテムを全て表示
    /// </summary>
    public void DisplayInventory()
    {
        Debug.Log("現在のインベントリ:");
        foreach (var item in items)
        {
            Debug.Log(item.itemName);
        }
    }
}
