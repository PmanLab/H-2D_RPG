using UnityEngine;
using UniRx;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameStateManager : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("ポーズUI")] private GameObject pauseUI;
    [SerializeField, Header("PlayerInputをアタッチ")] private PlayerInput playerInput;
    [SerializeField, Header("Inventoryをアタッチ")] private Inventory inventory;
    [SerializeField, Header("最初に選択状態にするボタンをアタッチ")] private GameObject firstPickButton;

    //=== 変数宣言 ===
    private ReactiveProperty<bool> isInPause = new ReactiveProperty<bool>(false);   // ポーズ用フラグ
    private IDisposable pausedSubscription;
    private InputAction pauseAction;    // "Pause"actionを保持する変数

    //=== プロパティ ===
    public bool IsInPause
    {
        get => isInPause.Value;
        set => isInPause.Value = value;
    }

    //=== メソッド ===
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
            .Where(_ => !PlayerStateManager.instance.IsInConversation && !inventory.isShowInventoryUI) // 非会話状態のみ処理
            .Where(_ => PlayerStateManager.instance.CurrentPlayerState != PlayerStateManager.PlayerState.Dead) // Dead以外のときのみ処理
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
                EventSystem.current.SetSelectedGameObject(firstPickButton);
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
}
