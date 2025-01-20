using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager instance_;

    private ReactiveProperty<bool> isInConversation_ = new ReactiveProperty<bool>(false);

    /// <summary>
    /// 第一初期化処理
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
    /// 第二初期化処理
    /// </summary>
    private void Start()
    {
        isInConversation_.Subscribe(isTalking =>
        {
            if (isTalking)
            {// 会話開始時処理

            }
            else
            {// 会話終了時処理

            }
        });
    }

    /// <summary>
    /// OnDestroyメソッド 
    /// </summary>
    private void OnDestroy()
    {
        isInConversation_.Dispose(); // ReactivePropertyを解放
    }

    /// <summary>
    /// 会話開始メソッド
    /// </summary>
    public void StartConversation()
    {
        isInConversation_.Value = true;
    }

    /// <summary>
    /// 会話終了メソッド
    /// </summary>
    public void EndConversation()
    {
        isInConversation_.Value = false;
    }
}
