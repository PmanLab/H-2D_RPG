using UnityEngine;

/// <summary>
/// プレイヤーステータスマネージャー(StatusManagerBaseを継承)
/// プレイヤーのステータスをまとめてある
/// </summary>
public class PlayerStatusManager : StatusManagerBase
{
    //=== シリアライズ ===
    //[SerializeField, Header("プレイヤーの特別なステータス")] private int specialAbilityPower = 20;

    /// <summary>
    /// プレイヤー固有の死亡処理メソッド
    /// </summary>
    protected override void Die()
    {
        base.Die();
        // プレイヤー用の死亡処理
    }
}

