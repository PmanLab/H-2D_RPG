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
    public int MaxHP => maxHP;
    public int AttackPower => attackPower;
    public IReadOnlyReactiveProperty<int> CurrentHP => currentHP;

    /// <summary>
    /// 第一初期化処理
    /// </summary>
    protected virtual void Awake()
    {
        currentHP = new ReactiveProperty<int>(maxHP);
    }

    /// <summary>
    /// ダメージを受ける処理メソッド
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHP.Value = Mathf.Max(currentHP.Value - damage, 0);
        if (currentHP.Value <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 回復する処理メソッド
    /// </summary>
    public void Heal(int amount)
    {
        currentHP.Value = Mathf.Min(currentHP.Value + amount, maxHP);
    }

    /// <summary>
    /// 死亡時の処理メソッド（各クラスでオーバーライド可能）
    /// </summary>
    protected virtual void Die()
    {
        // 死亡ロジック（子クラスで処理をオーバーライド可能）
    }
}
