using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// お金を保有するクラス
/// お金の要素をまとめてある
/// </summary>
[CreateAssetMenu(fileName = "NewGameData", menuName = "DataMoney/Money")]
public class DataMoney : ScriptableObject
{
    //=== シリアライズ ===
    [SerializeField, Header("初期所持金")]
    private int startingMoney = 100; // 初期所持金（デフォルト値：100）

    //=== 変数宣言 ===
    private const string MoneyKey = "PlayerMoney"; // PlayerPrefキー

    //=== プロパティ ===
    /// <summary>
    /// ・現在の所持金を取得、または設定する
    /// ・PlayerPrefで現在の所持金値を保存
    /// </summary>
    public int CurrentMoney
    {
        get => PlayerPrefs.GetInt(MoneyKey,startingMoney);  // デフォルト値を初期所持金に設定
        set
        {
            PlayerPrefs.SetInt(MoneyKey, value);
            PlayerPrefs.Save();
        }
    }

    //=== メソッド ===
    /// <summary>
    /// ・現在のお金を初期所持金にリセットする
    /// </summary>
    public void ResertMoney()
    {
        CurrentMoney = startingMoney;
    }
}
