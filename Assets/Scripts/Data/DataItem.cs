using UnityEngine;

/// <summary>
/// アイテムデータを保有するクラス
/// アイテムの要素をまとめてある
/// </summary>
[CreateAssetMenu(fileName = "NewGameData", menuName = "DataItem/Item")]
public class BaseItem : ScriptableObject
{
    public string itemName;          // アイテム名
    public string description;       // アイテム説明
    public int maxStack;             // 最大スタック数
    public int currentStackCount;    // 現在スタック数
    public int price;                // アイテムの価格
    public Sprite icon;              // アイテムのアイコン

    //=== メソッド ===

    /// <summary>
    /// アイテムをスタック可能かどうかを判定する
    /// </summary>
    /// <returns>
    /// スタック可能ならtrue、スタック上限に達している場合はfalseを返す
    /// </returns>
    public bool CanStack()
    {
        return currentStackCount + 1 <= maxStack;
    }

    /// <summary>
    /// アイテムを1つスタックする
    /// </summary>
    public void StackItem()
    {
        if (CanStack())
        {
            currentStackCount++;
        }
        else
        {
            Debug.LogWarning("スタック数が上限に達しています");
        }
    }
}
