using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// NPCインタラクト_ショップ(InteractBase継承)
/// ショップ(売買)NPCの処理をまとめてある
/// </summary>
public class NPCInteract_ItemShop : InteractBase
{
    //=== シリアライズ ===

    [SerializeField, Header("最初のセリフ")] private string initialDialogue;
    [SerializeField, Header("通常会話リスト")] private List<string> conversationList;
    [SerializeField, Header("会話テキストを表示するUIのText")] private Text dialogueText;  // Textコンポーネントを参照   [SerializeField, Header("会話ウィンドウのImage")] private Image dialogueWindow;  // 会話ウィンドウのImageコンポーネント
    [SerializeField, Header("会話ウィンドウのImage")] private GameObject dialogueWindow;  // 会話ウィンドウのImageコンポーネント

    [SerializeField, Header("アイテム価格")] private int itemPrice;
    [SerializeField, Header("アイテムの購入確認メッセージ")] private string purchaseMessage;

    [SerializeField, Header("インタラクトUIを制御するPlayerInteract")] private PlayerInteract playerInteract;  // PlayerInteractを参照
    [SerializeField, Header("PlayerControllerをアタッチ")] private PlayerController playerController;
    [SerializeField, Header("PlayerStatusManagerをアタッチ")] private PlayerStatusManager playerStatusManager;

    //=== 変数宣言 ===
    private int currentDialogueIndex = 0;  // 現在の会話のインデックス
    private bool isConversationActive = false;
    private bool isPurchaseConfirmationActive = false;
    private IDisposable conversationSubscription; // 購読を管理する変数


    //=== プロパティ ===
    public string InititalDialogue => initialDialogue;


    /// <summary>
    /// 
    /// インタラクトメソッド
    /// 
    /// 継承したインタラクト処理内で
    /// このNPCが会話した時のメソッドを呼び出す
    /// 
    /// </summary>
    public override void Interact()
    {
        // 会話を表示
        StartConversation();
    }

    /// <summary>
    /// 
    /// 会話の開始処理メソッド
    /// 
    /// NPCの名前やセリフ等の情報を設定し、
    /// それを表示される
    /// 
    /// リストによる会話自の会話進行処理
    /// 
    /// </summary>
    private void StartConversation()
    {
        // 会話がすでに進行中であればインデックスをリセットしない
        if (!isConversationActive)
        {
            SetNpcName();               // NPCの名前をセット
            currentDialogueIndex = 0;  // セリフインデックスをリセット
            isConversationActive = true;

            playerController.StopMovement();    // 会話中はプレイヤーの移動を停止
            DisplayDialogue(initialDialogue);   // 最初のセリフを表示

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
                        // 会話が終了したらアイテム購入確認を表示
                        if (!isPurchaseConfirmationActive)
                        {
                            isPurchaseConfirmationActive = true;
                            // 購入確認の表示メソッドを呼び出す
                            DisplayPurchaseConfirmation();
                        }
                        else
                        {
                            // 購入確認後、会話を終了
                            EndConversation();
                        }
                    }
                })
                .AddTo(this); // メモリリーク防止
        }
    }

    /// <summary>
    /// 
    /// アイテム購入確認処理メソッド
    /// 
    /// 選択を選択
    /// └ はい：購入(個人マネーが減る)
    /// └ いいえ：購入キャンセル
    /// 
    /// </summary>
    private void DisplayPurchaseConfirmation()
    {
        isPurchaseConfirmationActive = true;
        DisplayDialogue("アイテムを購入しますか？ (Y: はい, N: いいえ)");

        // 購読を使ってユーザーの入力を確認
        conversationSubscription?.Dispose();  // 以前の購読を解除

        // 購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => isPurchaseConfirmationActive)
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.Y))  // 「Y」が押された場合
                {
                    HandleItemPurchase();
                    EndConversation();
                }
                else if (Input.GetKeyDown(KeyCode.N))  // 「N」が押された場合
                {
                    DisplayDialogue("購入がキャンセルされました");
                    EndConversation();
                }
            }).AddTo(this);  // メモリリーク防止
    }


    /// <summary>
    /// 
    /// 会話を表示するメソッド
    /// 
    ///  会話ウィンドウとテキストを表示
    ///  テキストをセット
    /// 
    /// </summary>
    private void DisplayDialogue(string dialogue)
    {
        ShowDialogueWindow(true);       
        dialogueText.text = dialogue;   
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

        isPurchaseConfirmationActive = false;
    }

    /// <summary>
    /// 
    /// 会話ウィンドウの表示・非表示を切り替えるメソッド
    /// 
    /// 引数：(有効・無効(true || false)l)
    /// 
    /// </summary>
    private void ShowDialogueWindow(bool isVisible)
    {
        dialogueWindow.SetActive(isVisible);  // ウィンドウの表示/非表示を設定
        dialogueText.gameObject.SetActive(isVisible);  // テキストの表示/非表示を設定
    }

    /// <summary>
    /// 
    /// 購入処理をPlayerStatusManagerを使用して行うメソッド
    /// 
    /// 購入処理の一連をまとめてある
    /// ※今は買うか買わないかぐらい
    /// 
    /// </summary>
    private void HandleItemPurchase()
    {
        if (playerStatusManager.CurrentMoney >= itemPrice)
        {
            playerStatusManager.SpendMoney(itemPrice);
            Debug.Log("アイテムを購入しました！");
            DisplayDialogue("アイテムを購入しました！");

            // アイテム付与処理を追加する場所

            // アイテム購入後、2秒後に会話終了
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => EndConversation())
                .AddTo(this);
        }
        else
        {
            DisplayDialogue("お金が足りません！");
            Debug.Log("お金が足りません！");

            // お金が足りない場合も2秒後に会話終了
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => EndConversation())
                .AddTo(this);
        }
    }

}
