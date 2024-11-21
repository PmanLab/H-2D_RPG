using UnityEngine;

/// <summary>
/// エネミーステータスマネージャー(StatusManagerBaseを継承)
/// エネミーのステータスをまとめてある
/// </summary>
public class EnemyStatusManager : StatusManagerBase
{
    //=== シリアライズ ===
    //[SerializeField, Header("敵の特別なステータス")] private int aggressionLevel = 5;

    /// <summary>
    /// ・敵固有の死亡処理
    /// </summary>
    protected override void Die()
    {
        base.Die();
        // 敵用の死亡処理
    }
}
