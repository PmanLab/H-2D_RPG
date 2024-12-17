using System;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

/// <summary>
/// NPCインタラクト_宿屋(InteractBase継承)
/// 宿屋NPCの処理をまとめてある
/// </summary>
public class NPCInteract_InnKeeper : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("宿屋の価格")] private int innPrice;
    [SerializeField, Header("宿屋の宿泊メッセージ")] private string innMessage;

    [SerializeField, Header("Inventoryをアタッチ")] private Inventory inventory;
    [SerializeField, Header("PlayerInteractをアタッチ")] private PlayerInteract playerInteract;

    //=== 変数宣言 ===
    private int currentDialogueIndex = 0;           // 現在の会話のインデックス
    private IDisposable conversationSubscription;   // 購読を管理する変数

    //=== メソッド ===
    /// <summary>
    /// ・継承したインタラクト処理内で
    /// 　このNPCが会話した時のメソッドを呼び出す
    /// </summary>
    public override void Interact()
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
        SetNpcName();           //NPCの名前をセット
        currentDialogueIndex = 0; // セリフインデックスをリセット
        PlayerStateManager.instance.StartConversation();
        inventory.isConvertionActive = true;
        inventory.ShowInventoryUI();

        PlayerController.StopMovement(); // 会話中はプレイヤーの移動を停止
        DisplayDialogue(InitialDialogue); // 最初のセリフを表示

        ShowInteractUI(false); // 会話中はインタラクトUIを非表示にする

        // 前回の購読を解除（もし存在すれば）
        conversationSubscription?.Dispose();

        // 新しい購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => PlayerStateManager.instance.GetConversation() && !PlayerController.IsMoving && playerInteract.interactAction.triggered)
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
                    // 会話が終了したら宿泊確認を表示
                    DisplayInnConfirmation();
                }
            }).AddTo(this); // メモリリーク防止
    }

    /// <summary>
    /// ・宿泊確認の選択肢処理
    /// ・選択肢後、各パターン(Y：はい、N：いいえ)
    /// └Y：所持金からお金を払い体力を全回復する
    /// └N：何もしない
    /// 
    /// </summary>
    private void DisplayInnConfirmation()
    {
        //isConversationActive = true;
        DisplayDialogue("宿泊しますか？ (B: はい, A: いいえ)");

        // 購読を使ってユーザーの入力を確認
        conversationSubscription?.Dispose(); // 以前の購読を解除

        // 購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => PlayerStateManager.instance.GetConversation())
            .Subscribe(_ =>
            {
                if (playerInteract.YesAction.triggered)
                {
                    if (PlayerStatusManager.CanAfford(innPrice))
                    {
                        PlayerStatusManager.SpendMoney(innPrice); // 宿泊代を支払う
                        PlayerStatusManager.MaxHeal(); // プレイヤーを回復
                        DisplayDialogue(innMessage); // 宿泊メッセージを表示

                        // メッセージ表示指定した待機時間後、会話終了
                        Observable.Timer(TimeSpan.FromSeconds(ConstantManager.interactWaitingTime))
                            .Subscribe(_ => EndConversation())
                            .AddTo(this);
                    }
                    else
                    {
                        DisplayDialogue("お金が足りません");

                        // メッセージ表示指定した待機時間後、会話終了
                        Observable.Timer(TimeSpan.FromSeconds(ConstantManager.interactWaitingTime))
                            .Subscribe(_ => EndConversation())
                            .AddTo(this);
                    }
                }
                else if (playerInteract.NoAction.triggered)
                {
                    DisplayDialogue("宿泊をキャンセルしました");

                    Observable.Timer(TimeSpan.FromSeconds(ConstantManager.interactWaitingTime)) // 2秒待機
                        .Subscribe(_ => EndConversation())
                        .AddTo(this);
                }
            }).AddTo(this); // メモリリーク防止
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
    /// ・会話が終了した際にUIを非表示にする
    /// </summary>
    private void EndConversation()
    {
        Debug.Log("会話を終了しました・.");
        PlayerStateManager.instance.EndConversation();
        inventory.isConvertionActive = false;
        ShowDialogueWindow(false);              // 会話ウィンドウを非表示
        ShowInteractUI(true);    // インタラクトUIを再表示
        PlayerController.ResumeMovement();      // プレイヤーの移動を再開

        conversationSubscription?.Dispose();    // 購読を解除
    }



    
}
