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

    //=== プロパティ ===
    /// <summary>
    /// 初期所持金を取得する
    /// 外部からは値を変更できない
    /// </summary>
    public int StartingMoney => startingMoney;
}
