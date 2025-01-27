using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// NPCインタラクト 選択肢イベント(InteractBase継承)
/// 選択肢NPCの処理をまとめてある
/// </summary>
public class NPCInteract_ChoiceEvent : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("選択肢ボタンUI")] private GameObject choiceButtonUI;
    [SerializeField, Header("最初に選択状態にするボタンをアタッチ")] private GameObject firstApplyButton;

    [SerializeField, Header("選択肢表示時のテキスト文章")] private string choiceText;

    [SerializeField, Header("PlayerInteractをアタッチ")] private PlayerInteract playerInteract;

    //=== 変数宣言 ===
    private int currentDialougueIndex = 0;  // 現在のインデックス
    private bool isAccommodationConfirmationActive = false; //選択肢確認フラグ
    private IDisposable conversationSubcription;

    //=== メソッド ===
    /// <summary>
    /// ・継承したインタラクト処理内で
    /// 　このNPCが会話した時のメソッドを呼びだす
    /// </summary>
    public override void InteractProcess()
    {
        // 会話を表示
        StartConversation();
    }

    private void StartConversation()
    {
        SetNpcName();   // NPCの名前をセット
        currentDialougueIndex = 0;  // セリフインデックスをリセット
        PlayerStateManager.instance.IsInConversation = true;
        inventory.ShowInventoryUI();

        PlayerController.StopMovement();    // 会話中はプレイヤーの移動を停止
        DisplayDialogue(InitialDialogue);   // 最初のセリフを表示

        ShowInteractUI(false);  // 会話中はインタラクトUIを非表示にする

        // 前回の購読を解除(もし存在すれば)
        conversationSubcription?.Dispose();

        // 新しい購読を登録
        conversationSubcription = Observable.EveryUpdate()
            .Where(_ => PlayerStateManager.instance.IsInConversation &&
                        !PlayerStateManager.instance.IsChoice &&
                        !PlayerController.IsMoving &&
                        playerInteract.interactAction.triggered)
            .Subscribe(_ =>
            {
                if (currentDialougueIndex < ConversationList.Count)
                {
                    // 次のセリフを表示
                    DisplayDialogue(ConversationList[currentDialougueIndex]);
                }
                else
                {
                    // 会話が終了したら選択肢画面を表示
                    DisplayChoiceConfirmation();
                }
            }).AddTo(this);  // メモリリーク防止
    }

    /// <summary>
    /// ・選択肢の処理
    /// </summary>
    private void DisplayChoiceConfirmation()
    {
        PlayerStateManager.instance.IsChoice = true;
        isAccommodationConfirmationActive = true;
        DisplayDialogue(choiceText);
        ShowButton(true);
    }

    /// <summary>
    /// ・会話を表示するメソッド
    /// ・会話ウィンドウとテキストを表示
    /// </summary>
    /// <param name="dialogue">表示するセリフ</param>
    private void DisplayDialogue(string dialogue)
    {
        ShowDialogueWindow(true);       // 会話ウィンドウとテキストを表示
        DialogueText.text = dialogue;   // テキストをセット
    }

    /// <summary>
    /// ・宿屋宿泊承認確認ボタンのの表示・非表示を切り替える処理
    /// </summary>
    /// <param name="isVisible">メッセージウィンドウの有効・無効</param>
    private void ShowButton(bool isVisible)
    {
        choiceButtonUI.gameObject.SetActive(isVisible);
        EventSystem.current.SetSelectedGameObject(firstApplyButton); // 最初に選択状態にするボタンを割り当て
    }

    //=== 選択肢イベントまとめ ===

    /// <summary>
    /// ・選択肢を承認した場合の処理
    /// </summary>
    public void ApplyChoice()
    {
        // ここに承認した際の処理を追加する

        Debug.Log("選択肢を承認しました");
    }

    /// <summary>
    /// ・選択肢を拒否した場合の処理
    /// </summary>
    public void UnApplyChoice()
    {
        Debug.Log("選択肢を拒否しました");
    }
}
