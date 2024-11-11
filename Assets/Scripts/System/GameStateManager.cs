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
    /// ��ꏉ�������\�b�h
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
    /// ��񏉊������\�b�h
    /// </summary>
    private void Start()
    {
        isInPause.Subscribe(isPaused =>
        
        {
            if(isPaused)
            {// �|�[�Y���̏���

            }
            else
            {// ��|�[�Y���̏���
                
            }
        });
    }

    /// <summary>
    /// OnDestroy���\�b�h 
    /// </summary>
    private void OnDestroy()
    {
        isInPause.Dispose(); // ReactiveProperty�����
    }

    /// <summary>
    /// �|�[�Y�J�n��
    /// </summary>
    public void StartPause()
    {
        isInPause.Value = true;
    }

    /// <summary>
    /// �|�[�Y�I����
    /// </summary>
    public void EndPaused()
    {
        isInPause.Value = false;
    }
}
