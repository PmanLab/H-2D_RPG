using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Runtime.CompilerServices;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance_;

    private ReactiveProperty<bool> isInPause = new ReactiveProperty<bool>(false);

    /// <summary>
    /// 第一初期化メソッド
    /// </summary>
    private void Awake()
    {
        //--- シングルトン ---
        if(instance_ == null)
        {
            instance_ = this;
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
        isInPause.Subscribe(isPaused =>
        
        {
            if(isPaused)
            {// ポーズ時の処理

            }
            else
            {// 非ポーズ時の処理
                
            }
        });
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
    public void StartPause()
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
