using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager instance_;

    private ReactiveProperty<bool> isInConversation_ = new ReactiveProperty<bool>(false);

    /// <summary>
    /// ��ꏉ��������
    /// </summary>
    private void Awake()
    {
        //--- �V���O���g�� ---
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
    /// ��񏉊�������
    /// </summary>
    private void Start()
    {
        isInConversation_.Subscribe(isTalking =>
        {
            if (isTalking)
            {// ��b�J�n������

            }
            else
            {// ��b�I��������

            }
        });
    }

    /// <summary>
    /// OnDestroy���\�b�h 
    /// </summary>
    private void OnDestroy()
    {
        isInConversation_.Dispose(); // ReactiveProperty�����
    }

    /// <summary>
    /// ��b�J�n���\�b�h
    /// </summary>
    public void StartConversation()
    {
        isInConversation_.Value = true;
    }

    /// <summary>
    /// ��b�I�����\�b�h
    /// </summary>
    public void EndConversation()
    {
        isInConversation_.Value = false;
    }
}
