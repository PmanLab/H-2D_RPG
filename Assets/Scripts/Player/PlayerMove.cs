using UnityEngine;
using UniRx;

public class PlayerMove : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("RigidBody")] private Rigidbody rb;
    [SerializeField, Header("移動速度値")] private float moveSpeed = 5.0f;

    /// <summary>
    /// ・入力情報に基づき、指定した速度で移動する処理
    /// </summary>
    /// <param name="moveInput">入力情報</param>
    public void Move(Vector3 moveInput)
    {
        // 水平移動
        rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.z * moveSpeed);  // Vector2からVector3に変更
    }

    /// <summary>
    /// ・後で拡張可能
    /// ※攻撃処理
    /// </summary>
    public void Attack()
    {
        Debug.Log("Attack!");
    }
}
