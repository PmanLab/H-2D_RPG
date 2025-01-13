using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //=== 格納用インスタンス ===
    [SerializeField, Header("PlayerInputをアタッチ")]private PlayerInput playerInput;
    [SerializeField, Header("PlayerMoveをアタッチ")] private PlayerMove playerMove;

    // PlayerInputのアクションマッピング
    private InputAction moveForwardAction;      // 前 移動用
    private InputAction moveBackwardAction;     // 後 移動用
    private InputAction moveLeftAction;         // 左 移動用
    private InputAction moveRightAction;        // 右 移動用
    private InputAction attackAction;           // 攻撃 用

    //=== 変数宣言 ===
    private ReactiveProperty<Vector3> moveInput = new ReactiveProperty<Vector3>();  // 移動検知用
    private ReactiveProperty<bool> isAttacking = new ReactiveProperty<bool>(false); // 攻撃検知用フラグ
    private bool isConversationActive = false;  // 会話中かどうかのフラグ

    //=== プロパティ ===
    public bool IsMoving => moveInput.Value != Vector3.zero; // 移動中かどうかを判定するプロパティ

    //=== メソッド ===
    /// <summary>
    /// ・PlayerInputのAction情報を取得し、
    /// 取得した情報を変数に格納
    /// ・入力をReactivePropertyにバインド
    /// ・攻撃監視処理と攻撃処理の内容
    /// </summary>
    private void Start()
    {
        //--- Actionの取得 ---
        var playerControls = playerInput.actions;
        moveForwardAction = playerControls["MoveForward"];
        moveBackwardAction = playerControls["MoveBackward"];
        moveLeftAction = playerControls["MoveLeft"];
        moveRightAction = playerControls["MoveRight"];
        attackAction = playerControls["Attack"];

        //--- 入力をReactivePropertyにバインド ---
        moveForwardAction.performed += ctx => UpdateMoveInput();
        moveBackwardAction.performed += ctx => UpdateMoveInput();
        moveLeftAction.performed += ctx => UpdateMoveInput();
        moveRightAction.performed += ctx => UpdateMoveInput();

        moveForwardAction.canceled += ctx => UpdateMoveInput();
        moveBackwardAction.canceled += ctx => UpdateMoveInput();
        moveLeftAction.canceled += ctx => UpdateMoveInput();
        moveRightAction.canceled += ctx => UpdateMoveInput();

        attackAction.performed += _ => isAttacking.Value = true;

        //--- プレイヤーの状態に応じた動作 ---
        isAttacking.Subscribe(attacking =>
        {
            if (attacking)
            {
                // 攻撃処理
                playerMove.Attack();
            }
        });
    }

    /// <summary>
    /// 
    /// 入力行動更新メソッド
    /// 
    /// 会話していない時、
    /// インプット時の移動判定処理(ここでは入力検知のみ)
    /// 
    /// </summary>
    private void UpdateMoveInput()
    {
        // 会話中は移動しないようにする
        if (isConversationActive) return;

        // 各キー入力に基づいて移動方向を決定
        float x = 0f;
        float z = 0f;

        // Wキー (前進)
        if (moveForwardAction.ReadValue<float>() > 0f) z = 1f;
        // Sキー (後退)
        if (moveBackwardAction.ReadValue<float>() > 0f) z = -1f;
        // Aキー (左移動)
        if (moveLeftAction.ReadValue<float>() > 0f) x = -1f;
        // Dキー (右移動)
        if (moveRightAction.ReadValue<float>() > 0f) x = 1f;

        // 斜め移動を考慮するため、ベクトルを正規化
        Vector3 direction = new Vector3(x, 0f, z);

        // ベクトルの大きさを1にして、斜め移動でも一定速度にする
        if (direction.magnitude > 1f){ direction.Normalize(); }

        // 移動方向をVector3として設定
        moveInput.Value = direction;  // 3D移動に合わせたVector3
    }


    /// <summary>
    /// 
    /// 更新処理メソッド
    /// 
    /// 入力情報を検知し、
    /// プレイヤーが移動する処理を実行する
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        playerMove.Move(moveInput.Value);   // 移動の処理
        Debug.Log("現在のゲーム内時間状況⇒（"+ Time.timeScale + ")");
        Debug.Log("現在の会話状況⇒ (" + PlayerStateManager.instance.IsInConversation+ ")");
    }


    /// <summary>
    /// 
    /// (会話フラグON)プレイヤー移動停止メソッド
    /// 
    /// 会話開始時に移動を停止する処理
    /// 
    /// </summary>
    public void StopMovement()
    {
        isConversationActive = true;
        moveInput.Value = Vector3.zero;  // 移動を強制的に停止
    }

    // 会話終了時に移動を再開
    public void ResumeMovement()
    {
        isConversationActive = false;
    }

    /// <summary>
    /// OnDestroyメソッド 
    /// 
    /// ReactivePropertyを解放
    /// 
    /// </summary>
    private void OnDestroy()
    {
        moveInput.Dispose();
        isAttacking.Dispose();
    }
}
