using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーステータスマネージャー(StatusManagerBaseを継承)
/// プレイヤーのステータス（HP、所持金など）をまとめて管理
/// </summary>
public class PlayerStatusManager : StatusManagerBase
{
    //=== シリアライズ ===
    [SerializeField, Header("所持金データ")] private DataMoney moneyData;
    [SerializeField, Header("所持金表示用のUI")] private GameObject currentMoneyUI;            // 所持金を表示するUIまとめ
    [SerializeField, Header("所持金表示用のテキスト")] private Text currentMoneyDisplayText;  // 所持金を表示するTextコンポーネント

    //=== プロパティ ===
    /// <summary>
    /// ・現在の所持金を表示用テキストに設定する処理
    /// </summary>
    public void SetCurrentMoney() => currentMoneyDisplayText.text = moneyData.CurrentMoney.ToString();

    /// <summary>
    /// ・所持金UI表示・非表示を切り替える処理
    /// </summary>
    /// <param name="isVisible">所持金UIの有効・無効</param>
    public void ShowCurrentMoney(bool isVisible) => currentMoneyUI.SetActive(isVisible);

    /// <summary>
    /// ・自分のお金と他を比べて結果を返す
    /// └ 所持金が足りているかを確認するのに使う
    /// </summary>
    /// <param name="amount">比べる他対象のお金情報</param>
    /// <returns></returns>
    public bool CanAfford(int amount) => moneyData.CurrentMoney >= amount;

    //=== メソッド ===
    /// <summary>
    /// ・親クラスのAwakeを呼び出し(StatusManagerBase)
    /// └ 最大HPを設定
    /// └ 所持金を初期化
    /// 
    /// ・所持金の表示
    /// </summary>
    protected override void Awake()
    {
        base.Awake();  // 親クラスのAwakeを呼び出してHPを初期化
        moneyData.CurrentMoney = moneyData.CurrentMoney;
        SetCurrentMoney();  // 初期所持金をUIテキストで表示
        Debug.Log($"初期所持金: {moneyData.CurrentMoney}円");
    }

    /// <summary>
    /// ・所持金から指定した値分だけ減らす
    /// </summary>
    /// <param name="amount">減らす金額</param>
    /// <returns>減らす金額が所持金以上ならtrue、足りない場合はfalse</returns>
    public bool SpendMoney(int amount)
    {
        if (moneyData.CurrentMoney >= amount)
        {
            moneyData.CurrentMoney -= amount;
            SetCurrentMoney();
            Debug.Log($"所持金: {moneyData.CurrentMoney}円");
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
        moneyData.CurrentMoney += amount;
        SetCurrentMoney();
        Debug.Log($"所持金: {moneyData.CurrentMoney}円");
    }

    /// <summary>
    /// ・プレイヤー用の死亡処理
    /// 今後に期待
    /// </summary>
    protected override void Die()
    {
        base.Die();
        moneyData.CurrentMoney = 0;       // 所持金全てを失う
        Debug.Log("プレイヤーが死亡しました！");
    }
}
