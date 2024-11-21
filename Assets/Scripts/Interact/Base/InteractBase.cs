using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インタラクトベース(抽象クラス)
/// インタラクトにおける共通処理をまとめてある
/// </summary>
public abstract class InteractBase : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("オブジェクト名")] private string interactableName;
    [SerializeField, Header("表示するインタラクト可能UI")] private GameObject interactUI;  // インタラクト可能UIを表示するためのGameObject
    [SerializeField, Header("オブジェクト名を表示するText")] private Text interactableNameText;
    [SerializeField, Header("名前表示用のUIテキスト")] private Text nameDisplayText; // オブジェクト名を表示するTextコンポーネント

    //=== プロパティ ===
    public string InteractableName => interactableName;
    
    /// <summary>
    ///
    /// インタラクト時の処理メソッド
    /// (継承クラスで処理を書く)
    /// 
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// 
    /// UIの表示・非表示を切り替えるメソッド
    ///  
    /// オブジェクトを無効・有効にする処理
    /// 引数：(有効・無効(true || false))
    /// 
    /// </summary>
    public virtual void ShowInteractUI(bool isVisible)
    {
        interactUI.SetActive(isVisible);
    }

    /// <summary>
    /// 
    /// テキストにNPC名をセットするメソッド
    /// 
    /// インスペクターで指定した名前を
    /// メッセージウィンドウで表示
    /// 
    /// </summary>
    public virtual void SetNpcName()
    {
        nameDisplayText.text = interactableName;
    }
}
