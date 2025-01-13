using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インタラクトベース(抽象クラス)
/// インタラクトにおける共通処理をまとめてある
/// </summary>
public abstract class InteractBase : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("PlayerStatusManagerをアタッチ")] private PlayerStatusManager playerStatusManager;
    [SerializeField, Header("PlayerControllerをアタッチ")] private PlayerController playerController;

    [SerializeField, Header("オブジェクト名")] private string interactableName;
    [SerializeField, Header("表示するインタラクト可能UI")] private GameObject interactUI;
    [SerializeField, Header("オブジェクト名を表示するText")] private Text interactableNameText;
    [SerializeField, Header("名前表示用のUIテキスト")] private Text nameDisplayText;

    [SerializeField, Header("最初のセリフ")] private string initialDialogue;
    [SerializeField, Header("通常会話リスト")] private List<string> conversationList;
    [SerializeField, Header("会話テキストを表示するUIのText")] private Text dialogueText;
    [SerializeField, Header("会話ウィンドウのImage")] private GameObject dialogueWindow;

    //=== プロパティ ===
    public string InteractableName { get; set; }
    public string InitialDialogue => initialDialogue;
    public List<string> ConversationList => conversationList;
    public Text DialogueText => dialogueText;
    public PlayerStatusManager PlayerStatusManager => playerStatusManager;
    public PlayerController PlayerController => playerController;
    public GameObject DialogueWindow => dialogueWindow;


    /// <summary>
    /// ・インスペクターで指定した名前を
    /// 　メッセージウィンドウで表示
    /// </summary>
    public virtual void SetNpcName() => nameDisplayText.text = interactableName;

    //=== メソッド ===
    /// <summary>
    /// ・オブジェクトを無効・有効にする処理
    /// </summary>
    /// <param name="isVisible">UIの有効・無効</param>
    public virtual void ShowInteractUI(bool isVisible) => interactUI.SetActive(isVisible);

    /// <summary>
    /// ・非会話状態時にインタラクト処理を行う
    /// </summary>
    public void Interact()
    {
        Debug.Log("いんたらくと処理が承認されました");
        if (PlayerStateManager.instance.GetConversation() && 
            PlayerStateManager.instance.GetChoice()) { return; }
        InteractProcess();
    }

    /// <summary>
    /// ・仮想関数
    /// └ 継承クラスで処理を書く
    /// </summary>
    public abstract void InteractProcess();

    /// <summary>
    /// ・会話ウィンドウの表示・非表示を切り替える処理
    /// </summary>
    /// <param name="isVisible">メッセージウィンドウの有効・無効</param>
    public virtual void ShowDialogueWindow(bool isVisible)
    {
        dialogueWindow.SetActive(isVisible);                // ウィンドウの表示/非表示を設定
        dialogueText.gameObject.SetActive(isVisible);       // テキストの表示/非表示を設定
        PlayerStatusManager.ShowCurrentMoney(isVisible);    // 所持金の表示/非表示を設定
    }
}