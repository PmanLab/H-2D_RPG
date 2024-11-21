using UnityEngine;
using UniRx;

/// <summary>
/// プレイヤーステータスマネージャー(StatusManagerBaseを継承)
/// プレイヤーのステータス（HP、所持金など）をまとめて管理
/// </summary>
public class PlayerStatusManager : StatusManagerBase
{
    //=== シリアライズ ===
    //[SerializeField, Header("最大HP")] private int maxHealth = 100;
    [SerializeField, Header("初期所持金")] private int startingMoney = 100;

    //=== 所持金関連 ===
    private int currentMoney; // 所持金

    //=== プロパティ ===
    public int CurrentMoney => currentMoney; // 現在の所持金

    //=== 初期化処理 ===
    /// <summary>
    /// ・親クラスのAwakeを呼び出し(StatusManagerBase)
    /// └ 最大HPを設定
    /// ※所持金の表示
    /// </summary>
    protected override void Awake()
    {
        base.Awake();  // 親クラスのAwakeを呼び出してHPを初期化
        currentMoney = startingMoney;  // 所持金を初期化
        Debug.Log($"初期所持金: {currentMoney}円");
    }

    //=== 所持金関連 ===
    /// <summary>
    /// ・所持金から指定した値分だけ減らす
    /// </summary>
    /// <param name="amount">減らす金額</param>
    /// <returns>減らす金額が所持金以上ならtrue、足りない場合はfalse</returns>
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"所持金: {currentMoney}円");
            return true;
        }
        else
        {
            Debug.Log("お金が足りません！");
            return false;
        }
    }

    /// <summary>
    /// ・所持金に指定した値分だけ与える
    /// </summary>
    /// <param name="amount">追加する金額</param>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"所持金: {currentMoney}円");
    }

    /// <summary>
    /// ・プレイヤー用の死亡処理
    /// 今後に期待
    /// </summary>
    protected override void Die()
    {
        base.Die();
        Debug.Log("プレイヤーが死亡しました！");
    }

    /// <summary>
    /// ・自分のお金と他を比べて結果を返す
    /// </summary>
    /// <param name="amount">比べる他対象のお金情報</param>
    /// <returns></returns>
    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;  // 所持金が足りているかを確認
    }
}
