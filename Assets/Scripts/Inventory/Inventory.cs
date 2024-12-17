using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのインベントリを管理するクラス
/// </summary>
public class Inventory : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("最大スロット数")] private int maxSlots = 20;
    [SerializeField, Header("インベントリ内容表示用テキスト")] private Text inventoryText; // インベントリ内容を表示するUIテキスト
    [SerializeField, Header("インベントリUI")] private GameObject inventoryUI;
    [SerializeField, Header("InteractBaseをアタッチ")] private InteractBase interactBase;

    //=== 変数宣言 ===
    private List<BaseItem> items = new List<BaseItem>(); // 所持アイテムリスト
    public bool isShowInventoryUI { get; set; } = false;
    public bool isConvertionActive { get; set; } = false;  // 会話検知用フラグ

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
            Debug.Log($"{item.itemName} x{item.currentStackCount}");
        }
    }


    /// <summary>
    /// UIテキストにインベントリ内容を表示
    /// </summary>
    public void UpdateInventoryUI()
    {
        if (inventoryText == null)
        {
            Debug.LogWarning("インベントリUIテキストが設定されていません！");
            return;
        }

        // インベントリ内容を文字列にまとめる
        string inventoryContent = "インベントリ:\n";
        foreach (var item in items)
        {
            inventoryContent += $"{item.itemName} x{item.currentStackCount}\n";
        }

        // テキストUIに反映
        inventoryText.text = inventoryContent;
    }

    /// <summary>
    /// ・インベントリUIの表示・非表示を切り替える処理
    /// </summary>
    public void ShowInventoryUI()
    {
        //--- 会話検知処理 ---
        if (!isConvertionActive && !GameStateManager.instance.GetPaused())
        {
            //--- インベントリ表示非表示処理 ---
            if (!isShowInventoryUI)
            {
                UpdateInventoryUI();
                inventoryUI.SetActive(true);
                isShowInventoryUI = true;
                Time.timeScale = 0.0f;
            }
            else
            {
                inventoryUI.SetActive(false);
                isShowInventoryUI = false;
                Time.timeScale = 1.0f;
            }
        }
        else if(isConvertionActive)
        {
            Time.timeScale = 1.0f;
            inventoryUI.SetActive(false);
            isShowInventoryUI = false;
        }
        else if(GameStateManager.instance.GetPaused())
        {
            inventoryUI.SetActive(false);
            isShowInventoryUI = false;
        }
    }
}
