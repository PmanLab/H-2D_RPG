using UnityEngine;
using UniRx;

public class PlayerStateManager : MonoBehaviour
{
    //=== インスタンス ===
    public static PlayerStateManager instance;

    //=== 列挙型定義 ===
    public enum PlayerState
    {// プレイヤーの状態を表すenum
        Idle,         // 待機
        Walking,      // 移動(歩き)
        Dashing,      // 移動(走り)
        Attacking,    // 攻撃中
        TakingDamage, // 被ダメージ中
        Dead,         // 死亡状態
    }

    //=== 変数宣言 ===
    private ReactiveProperty<bool> bIsInConversation = new ReactiveProperty<bool>(false);   // 会話フラグ
    private ReactiveProperty<PlayerState> eCurrentPlayerState = new ReactiveProperty<PlayerState>(PlayerState.Idle); // プレイヤー状態

    /// <summary>
    /// 第一初期化処理
    /// </summary>
    private void Awake()
    {
        //--- シングルトン ---
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 第二初期化処理
    /// </summary>
    private void Start()
    {
        //--- 状態の変更を監視(各状態ごとの処理を設定) --- 
        bIsInConversation.Subscribe(isTalking =>
        {
            if (isTalking)
            {// 会話開始時処理

            }
            else
            {// 会話了時処理

            }
        });

        eCurrentPlayerState.Subscribe(state =>
        {
            switch (state)
            {
                case PlayerState.Idle:
                    // 待機状態の処理
                    break;
                case PlayerState.Walking:
                // 移動状態の処理
                case PlayerState.Dashing:
                    // 移動状態の処理
                    break;
                case PlayerState.Attacking:
                    // 攻撃中の処理
                    break;
                case PlayerState.TakingDamage:
                    // 被ダメージ中の処理
                    break;
                case PlayerState.Dead:
                    // 死亡状態の処理
                    break;
            }
        });
    }
        

    /// <summary>
    /// OnDestroyメソッド 
    /// </summary>
    private void OnDestroy()
    {
        // ReactivePropertyを解放
        bIsInConversation.Dispose();
        eCurrentPlayerState.Dispose();
    }

    /// <summary>
    /// 会話開始メソッド
    /// </summary>
    public void StartConversation()
    {
        bIsInConversation.Value = true;
    }

    /// <summary>
    /// 会話終了メソッド
    /// </summary>
    public void EndConversation()
    {
        bIsInConversation.Value = false;
    }

    /// <summary>
    /// プレイヤー状態設定メソッド
    /// </summary>
    /// <param name="playerState"></param>
    public void ChangePlayeState(PlayerState playerState)
    {
        eCurrentPlayerState.Value = playerState;
    }

    /// <summary>
    /// プレイヤー状態取得メソッド
    /// </summary>
    /// <returns></returns>
    public PlayerState GetPlayerState()
    {
        return eCurrentPlayerState.Value;
    }
}
