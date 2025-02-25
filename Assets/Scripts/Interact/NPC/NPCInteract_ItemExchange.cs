﻿using System;
using UnityEngine;
using UniRx;

/// <summary>
/// NPCインタラクト_アイテムトレード(InteractBase継承)
/// アイテムトレードNPCの処理をまとめてある
/// </summary>
public class NPCInteract_ItemExchange : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("PlayerInteractをアタッチ")] private PlayerInteract playerInteract;

    //=== 変数宣言 ===
    private int currentDialogueIndex = 0;  // 現在の会話のインデックス
    private IDisposable conversationSubscription; // 購読を管理する変数

    //=== メソッド ===
    /// <summary>
    ///・ 継承したインタラクト処理内で
    /// 　このNPCが会話した時のメソッドを呼び出す
    /// </summary>
    public override void InteractProcess()
    {
        // 会話を表示
        StartConversation();
    }

    /// <summary>
    /// ・NPCの名前やセリフ等の情報を設定し、
    /// 　それを表示される
    /// ・リストによる会話自の会話進行処理
    /// </summary>
    private void StartConversation()
    {
        SetNpcName();               // NPCの名前をセット
        currentDialogueIndex = 0;   // セリフインデックスをリセット
        PlayerStateManager.instance.IsInConversation = true;
        inventory.ShowInventoryUI();

        PlayerController.StopMovement();    // 会話中はプレイヤーの移動を停止
        DisplayDialogue(InitialDialogue);   // 最初のセリフを表示

        ShowInteractUI(false); // 会話中はインタラクトUIを非表示にする

        // 前回の購読を解除（もし存在すれば）
        conversationSubscription?.Dispose();

        // 新しい購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => PlayerStateManager.instance.IsInConversation && !PlayerController.IsMoving && playerInteract.interactAction.triggered)
            .Subscribe(_ =>
            {
                if (currentDialogueIndex < ConversationList.Count)
                {
                    // 次のセリフを表示
                    DisplayDialogue(ConversationList[currentDialogueIndex]);
                    currentDialogueIndex++; // 次のセリフインデックスに進む
                }
                else
                {
                    // 全てのセリフを読み終えたら会話を終了
                    EndConversation();
                }
            })
            .AddTo(this); // メモリリーク防止
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
    /// ・会話が終了した際にUIを非表示にし、
    /// 　各フラグをオフにする会話終了メソッド
    /// </summary>
    private void EndConversation()
    {
        Debug.Log("会話を終了しました・.");
        PlayerStateManager.instance.IsInConversation = false;
        ShowDialogueWindow(false);              // 会話ウィンドウを非表示
        PlayerController.ResumeMovement();      // プレイヤーの移動を再開

        conversationSubscription?.Dispose();    // 購読を解除
    }
}    
