using System;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// NPCインタラクト_宿屋(InteractBase継承)
/// 宿屋NPCの処理をまとめてある
/// </summary>
public class NPCInteract_InnKeeper : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("宿屋の価格")] private int innPrice;
    [SerializeField, Header("宿屋の宿泊メッセージ")] private string innMessage;

    [SerializeField, Header("PlayerInteractをアタッチ")] private PlayerInteract playerInteract;
    [SerializeField, Header("宿泊承認ボタンUI")] private GameObject applyButtonUI;
    [SerializeField, Header("最初に選択状態にするボタンをアタッチ")] private GameObject firstApplyButton;

    //=== 変数宣言 ===
    private int currentDialogueIndex = 0;           // 現在の会話のインデックス
    private bool isAccommodationConfirmationActive = false; // 宿泊確認フラグ
    private IDisposable conversationSubscription;   // 購読を管理する変数

    //=== メソッド ===
    /// <summary>
    /// ・継承したインタラクト処理内で
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
        SetNpcName();           //NPCの名前をセット
        currentDialogueIndex = 0; // セリフインデックスをリセット
        PlayerStateManager.instance.IsInConversation = true;
        inventory.ShowInventoryUI();

        PlayerController.StopMovement(); // 会話中はプレイヤーの移動を停止
        DisplayDialogue(InitialDialogue); // 最初のセリフを表示

        ShowInteractUI(false); // 会話中はインタラクトUIを非表示にする

        // 前回の購読を解除（もし存在すれば）
        conversationSubscription?.Dispose();

        // 新しい購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => PlayerStateManager.instance.IsInConversation &&
                        !PlayerStateManager.instance.IsChoice &&
                        !PlayerController.IsMoving && 
                        playerInteract.interactAction.triggered)
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
    /// </summary>
    private void DisplayInnConfirmation()
    {
        PlayerStateManager.instance.IsChoice = true;
        isAccommodationConfirmationActive = true;
        DisplayDialogue("宿泊しますか？");
        ShowButton(true);   // 宿泊をするかの確認ボタンを表示
    }


    /// <summary>
    /// ・宿泊を承認した際の処理
    /// </summary>
    public void ApplyInn()
    {
        if (PlayerStateManager.instance.IsInConversation && isAccommodationConfirmationActive)
        {// 会話中で非宿泊状態の場合処理を通す
            if (PlayerStatusManager.CanAfford(innPrice))
            {// お金が足りた場合に処理を通す
                PlayerStatusManager.SpendMoney(innPrice); // 宿泊代を支払う
                PlayerStatusManager.MaxHeal(); // プレイヤーを回復
                DisplayDialogue(innMessage); // 宿泊メッセージを表示

                isAccommodationConfirmationActive = false;

                // メッセージ表示指定した待機時間後、会話終了
                Observable.Timer(TimeSpan.FromSeconds(ConstantManager.interactWaitingTime))
                    .Subscribe(_ => EndConversation())
                    .AddTo(this);
            }
            else
            {// お金が足りない場合に処理を通す
                DisplayDialogue("お金が足りません");

                isAccommodationConfirmationActive = false;

                // メッセージ表示指定した待機時間後、会話終了
                Observable.Timer(TimeSpan.FromSeconds(ConstantManager.interactWaitingTime))
                    .Subscribe(_ => EndConversation())
                    .AddTo(this);
            }
        }
    }

    /// <summary>
    /// ・宿泊を拒否した場合の処理
    /// </summary>
    public void UnnApplyInn()
    {
        DisplayDialogue("宿泊をキャンセルしました");
        isAccommodationConfirmationActive = false;
        Observable.Timer(TimeSpan.FromSeconds(ConstantManager.interactWaitingTime)) // 2秒待機
            .Subscribe(_ => EndConversation())
            .AddTo(this);
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
        applyButtonUI.gameObject.SetActive(isVisible);
        EventSystem.current.SetSelectedGameObject(firstApplyButton); // 最初に選択状態にするボタンを割り当て
    }

    /// <summary>
    /// ・承認確認ボタンを非表示にする
    /// </summary>
    public void ApplyCloseButton()
    {
        applyButtonUI.gameObject.SetActive(false);      // 承認確認ボタンを非表示に設定
    }

    /// <summary>
    /// ・会話が終了した際にUIを非表示にする
    /// </summary>
    private void EndConversation()
    {
        Debug.Log("会話を終了しました・.");
        PlayerStateManager.instance.IsInConversation = false;
        PlayerStateManager.instance.IsChoice = false;
        ShowDialogueWindow(false);                  // 会話ウィンドウを非表示
        ShowButton(false);                          // 宿泊承認確認ボタンを非表示
        ShowInteractUI(true);                       // インタラクトUIを再表示
        PlayerController.ResumeMovement();          // プレイヤーの移動を再開

        conversationSubscription?.Dispose();        // 購読を解除

        isAccommodationConfirmationActive = false;  // 宿泊確認フラグ切り替え
    }



    
}
