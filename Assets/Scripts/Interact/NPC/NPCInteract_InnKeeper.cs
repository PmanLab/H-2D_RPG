using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// NPCインタラクト_宿屋(InteractBase継承)
/// 宿屋NPCの処理をまとめてある
/// </summary>
public class NPCInteract_InnKeeper : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("最初のセリフ")] private string initialDialogue;
    [SerializeField, Header("通常会話リスト")] private List<string> conversationList;
    [SerializeField, Header("会話テキストを表示するUIのText")] private Text dialogueText;  // Textコンポーネントを参照   [SerializeField, Header("会話ウィンドウのImage")] private Image dialogueWindow;  // 会話ウィンドウのImageコンポーネント
    [SerializeField, Header("会話ウィンドウのImage")] private GameObject dialogueWindow;  // 会話ウィンドウのImageコンポーネント

    [SerializeField, Header("宿屋の価格")] private int innPrice;
    [SerializeField, Header("宿屋の宿泊メッセージ")] private string innMessage;

    [SerializeField, Header("インタラクトUIを制御するPlayerInteract")] private PlayerInteract playerInteract;  // PlayerInteractを参照
    [SerializeField, Header("PlayerControllerをアタッチ")] private PlayerController playerController;
    [SerializeField, Header("PlayerStatusManagerをアタッチ")] private PlayerStatusManager playerStatusManager;

    //=== 変数宣言 ===
    private int currentDialogueIndex = 0;  // 現在の会話のインデックス
    private bool isConversationActive = false;
    private IDisposable conversationSubscription; // 購読を管理する変数

    //=== プロパティ ===
    public string InititalDialogue => initialDialogue;

    /// <summary>
    /// インタラクトメソッド
    /// </summary>
    public override void Interact()
    {
        // 会話を表示
        StartConversation();
    }

    /// <summary>
    /// 会話の開始処理メソッド
    /// </summary>
    private void StartConversation()
    {
        SetNpcName();           //NPCの名前をセット
        currentDialogueIndex = 0; // セリフインデックスをリセット
        isConversationActive = true;

        playerController.StopMovement(); // 会話中はプレイヤーの移動を停止
        DisplayDialogue(initialDialogue); // 最初のセリフを表示

        playerInteract.ShowInteractUI(false); // 会話中はインタラクトUIを非表示にする

        // 前回の購読を解除（もし存在すれば）
        conversationSubscription?.Dispose();

        // 新しい購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => isConversationActive && !playerController.IsMoving && Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ =>
            {
                if (currentDialogueIndex < conversationList.Count)
                {
                    // 次のセリフを表示
                    DisplayDialogue(conversationList[currentDialogueIndex]);
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
    /// 宿泊確認を表示するメソッド
    /// </summary>
    private void DisplayInnConfirmation()
    {
        isConversationActive = true;
        DisplayDialogue("宿泊しますか？ (Y: はい, N: いいえ)");

        // 購読を使ってユーザーの入力を確認
        conversationSubscription?.Dispose(); // 以前の購読を解除

        // 購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => isConversationActive)
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.Y)) // 「Y」が押された場合
                {
                    if (playerStatusManager.CanAfford(innPrice))
                    {
                        playerStatusManager.SpendMoney(innPrice); // 宿泊代を支払う
                        playerStatusManager.MaxHeal(); // プレイヤーを回復
                        DisplayDialogue(innMessage); // 宿泊メッセージを表示

                        // 宿泊メッセージ後に2秒待機して会話終了
                        Observable.Timer(TimeSpan.FromSeconds(2))
                            .Subscribe(_ => EndConversation())
                            .AddTo(this);
                    }
                    else
                    {
                        DisplayDialogue("お金が足りません");

                        // お金が足りない場合も2秒後に会話終了
                        Observable.Timer(TimeSpan.FromSeconds(2))
                            .Subscribe(_ => EndConversation())
                            .AddTo(this);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.N))
                {
                    DisplayDialogue("宿泊をキャンセルしました");

                    Observable.Timer(TimeSpan.FromSeconds(2)) // 2秒待機
                        .Subscribe(_ => EndConversation())
                        .AddTo(this);
                }
            }).AddTo(this); // メモリリーク防止
    }

    /// <summary>
    /// 会話を表示するメソッド
    /// </summary>
    private void DisplayDialogue(string dialogue)
    {
        ShowDialogueWindow(true);       // 会話ウィンドウとテキストを表示
        dialogueText.text = dialogue;   // テキストをセット
    }

    /// <summary>
    /// 会話が終了したらUIを非表示にするメソッド
    /// </summary>
    private void EndConversation()
    {
        Debug.Log("会話を終了しました・.");
        isConversationActive = false;
        ShowDialogueWindow(false);              // 会話ウィンドウを非表示
        playerInteract.ShowInteractUI(true);    // インタラクトUIを再表示
        playerController.ResumeMovement();      // プレイヤーの移動を再開

        conversationSubscription?.Dispose();    // 購読を解除
    }

    /// <summary>
    /// 会話ウィンドウの表示・非表示を切り替えるメソッド
    /// </summary>
    private void ShowDialogueWindow(bool isVisible)
    {
        dialogueWindow.SetActive(isVisible);  // ウィンドウの表示/非表示を設定
        dialogueText.gameObject.SetActive(isVisible);  // テキストの表示/非表示を設定
    }

}
