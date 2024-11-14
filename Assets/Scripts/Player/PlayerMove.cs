using UnityEngine;
using UniRx;

public class PlayerMove : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("RigidBody")] private Rigidbody rb;
    [SerializeField, Header("移動速度値")] private float moveSpeed = 5.0f;

    /// <summary>
    /// 移動処理メソッド
    /// </summary>
    /// <param name="moveInput"></param>
    public void Move(Vector3 moveInput)
    {
        // 水平移動
        rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.z * moveSpeed);  // Vector2からVector3に変更
    }

    /// <summary>
    /// 攻撃処理（後で拡張可能）
    /// </summary>
    public void Attack()
    {
        Debug.Log("Attack!");
    }
}
