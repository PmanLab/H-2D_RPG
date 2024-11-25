using UnityEngine;
using UniRx;

/// <summary>
/// ステータスマネージャーベース(抽象クラス)
/// キャラのステータスにおける共通処理をまとめてある
/// </summary>
public abstract class StatusManagerBase : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("最大HP")] protected int maxHP = 100;
    [SerializeField, Header("攻撃力")] protected int attackPower = 10;

    //=== 変数宣言 ===
    protected ReactiveProperty<int> currentHP; // 現在のHP

    //=== プロパティ ===
    public int MaxHP { get; set; }
    public int AttackPower { get; set; }
    public IReadOnlyReactiveProperty<int> CurrentHP { get; set; }


    //=== メソッド ===
    /// <summary>
    /// ・最大HPを設定する処理
    /// </summary>
    protected virtual void Awake()
    {
        currentHP = new ReactiveProperty<int>(maxHP);
    }


    /// <summary>
    ///　・指定した値分だけ
    ///　　ダメージを受ける処理
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    public void TakeDamage(int damage)
    {
        currentHP.Value = Mathf.Max(currentHP.Value - damage, 0);
        if (currentHP.Value <= 0)
        {
            Die();
        }
    }

    /// <summary>
    ///　・指定した値だけ
    ///　　回復する処理
    /// </summary>
    /// <param name="amount">回復する量</param>
    public void Heal(int amount)
    {
        currentHP.Value = Mathf.Min(currentHP.Value + amount, maxHP);
    }

    /// <summary>
    /// ・体力を全回復する処理
    /// </summary>
    public void MaxHeal()
    {
        currentHP.Value = maxHP;
    }

    /// <summary>
    /// ・死亡時の処理
    /// (各クラスでオーバーライド可能）
    /// </summary>
    protected virtual void Die()
    {
        // 死亡ロジック（子クラスで処理をオーバーライド可能）
    }
}
