using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class InteractBase : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("オブジェクト名")] private string interactableName;
    [SerializeField, Header("表示するインタラクト可能UI")] private GameObject interactUI;  // インタラクト可能UIを表示するためのGameObject

    //=== プロパティ ===
    public string InteractableName => interactableName;
    
    /// <summary>
    /// インタラクト時の処理(継承クラスで処理を書く)
    /// </summary>
    public abstract void Interact();


    /// <summary>
    /// インタラクト可能時のUI表示等の処理
    /// </summary>
    public virtual void OnInteractableHighlight()
    {
        ShowInteractUI(true);
    }

    /// <summary>
    /// インタラクト不可能時のUI非表示等の処理
    /// </summary>
    public virtual void OnInteractableUnHighlighted()
    {
        ShowInteractUI(false);
    }

    /// <summary>
    /// UIの表示・非表示を切り替える
    /// </summary>
    public virtual void ShowInteractUI(bool isVisible)
    {
        interactUI.SetActive(isVisible);
    }
}
