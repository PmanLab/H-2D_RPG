using UnityEngine;
using UniRx;
using System;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("ポーズUI")] private GameObject pauseUI;
    [SerializeField, Header("PlayerInputをアタッチ")] private PlayerInput playerInput;
    [SerializeField, Header("Inventoryをアタッチ")] private Inventory inventory;

    //=== インスタンス ===
    public static GameStateManager instance;    // インスタンス用

    //=== 変数宣言 ===
    private ReactiveProperty<bool> isInPause = new ReactiveProperty<bool>(false);   // ポーズ用フラグ
    private IDisposable pausedSubscription;
    private InputAction pauseAction;    // "Pause"actionを保持する変数

    //=== メソッド ===
    /// <summary>
    /// ・シングルトン生成処理
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
    /// ・ESCキー押下自のポーズフラグ監視処理
    /// ・ポーズになった時の細かい処理(今後はここに追加する)
    /// </summary>
    private void Start()
    {
        //--- InputActionを取得 ---
        pauseAction = playerInput.actions["Pause"]; // PlayerInputから「Pause」アクションを取得

        // ESCキーが押されたときにポーズ状態をトグル（Dead状態以外）
        pausedSubscription = Observable.EveryUpdate()
            .Where(_ => pauseAction.triggered)
            .Where(_ => !PlayerStateManager.instance.GetConversation() && !inventory.isShowInventoryUI) // 非会話状態のみ処理
            .Where(_ => PlayerStateManager.instance.GetPlayerState() != PlayerStateManager.PlayerState.Dead) // Dead以外のときのみ処理
            .Subscribe(_ =>
            {
                isInPause.Value = !isInPause.Value; // ポーズ状態をトグル
            })
            .AddTo(this);

        // ポーズ時の処理
        isInPause.Subscribe(isPaused =>
        {
            if (isPaused)
            {// ポーズ時の処理
                Debug.Log("ポーズ状態：" + isInPause.Value);
                pauseUI.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {// 非ポーズ時の処理
                Debug.Log("ポーズ状態：" + isInPause.Value);
                pauseUI.SetActive(false);
                Time.timeScale = 1.0f;
            }
        })
        .AddTo(this);
    }

    /// <summary>
    /// ・ReactivePropertyを解放
    /// </summary>
    private void OnDestroy()
    {
        isInPause.Dispose();
    }

    /// <summary>
    /// ・ポーズフラグをONにする処理
    /// ※ポーズ開始時
    /// </summary>
    public void StartPaused()
    {
        isInPause.Value = true;
    }

    /// <summary>
    /// ・ポーズフラグをOFFにする処理
    /// ※ポーズ終了時
    /// </summary>
    public void EndPaused()
    {
        isInPause.Value = false;
    }

    /// <summary>
    /// ・現在ポーズ状態中かどうかを返す
    /// </summary>
    /// <returns>現在のポーズ状態を取得</returns>
    public bool GetPaused()
    {
        return  isInPause.Value;
    }
}
