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
    public int price;                // アイテムの価格
    public Sprite icon;              // アイテムのアイコン

    //=== メソッド ===
    /// <summary>
    /// ・現在のスタック数をPlayerPrefsに保存、そして取得できる
    /// </summary>
    public int CurrentStackCount
    {
        get => PlayerPrefs.GetInt($"Stack_{itemName}", 0);
        set
        {
            PlayerPrefs.SetInt($"Stack_{itemName}", Mathf.Clamp(value, 0, maxStack));
            PlayerPrefs.Save();
        }
    }


    /// <summary>
    /// ・アイテムをスタック可能かどうかを判定する
    /// </summary>
    /// <returns>
    /// スタック可能ならtrue、スタック上限に達している場合はfalseを返す
    /// </returns>
    public bool CanStack()
    {
        return CurrentStackCount + 1 <= maxStack;
    }

    /// <summary>
    /// ・アイテムを1つスタックする
    /// </summary>
    public void StackItem()
    {
        if (CanStack())
        {
            CurrentStackCount++;
        }
        else
        {
            Debug.LogWarning("スタック数が上限に達しています");
        }
    }

    /// <summary>
    /// ・アイテムのスタックをリセットする
    /// </summary>
    public void ResetStack()
    {
        CurrentStackCount = 0;
    }
}
