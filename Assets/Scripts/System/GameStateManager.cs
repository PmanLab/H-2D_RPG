using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Runtime.CompilerServices;
using System;

public class GameStateManager : MonoBehaviour
{
    //=== インスタンス ===
    public static GameStateManager instance;

    //=== 変数宣言 ===
    private ReactiveProperty<bool> isInPause = new ReactiveProperty<bool>(false);
    private IDisposable pausedSubscription;

    /// <summary>
    /// 第一初期化メソッド
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
    /// 第二初期化メソッド
    /// </summary>
    private void Start()
    {
        // ESCキーが押されたときにポーズ状態をトグル（Dead状態以外）
        pausedSubscription = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Escape))
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
                Time.timeScale = 0.0f;
            }
            else
            {// 非ポーズ時の処理
                Debug.Log("ポーズ状態：" + isInPause.Value);
                Time.timeScale = 1.0f;
            }
        })
        .AddTo(this);
    }

    /// <summary>
    /// OnDestroyメソッド 
    /// </summary>
    private void OnDestroy()
    {
        isInPause.Dispose(); // ReactivePropertyを解放
    }

    /// <summary>
    /// ポーズ開始時
    /// </summary>
    public void StartPaused()
    {
        isInPause.Value = true;
    }

    /// <summary>
    /// ポーズ終了時
    /// </summary>
    public void EndPaused()
    {
        isInPause.Value = false;
    }
}
