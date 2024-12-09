using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// NPCインタラクト_ショップ(InteractBase継承)
/// ショップ(売買)NPCの処理をまとめてある
/// </summary>
public class NPCInteract_ItemShop : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("アイテム価格")] private int itemPrice;
    [SerializeField, Header("アイテムの購入確認メッセージ")] private string purchaseMessage;

    [SerializeField, Header("インタラクトUIを制御するPlayerInteract")] private PlayerInteract playerInteract;  // PlayerInteractを参照

    [SerializeField, Header("購入可能アイテム")] private List<BaseItem> shopItems;
    [SerializeField, Header("プレイヤーのインベントリ")] private Inventory playerInventory;

    [SerializeField, Header("購入可能なアイテムリストを表示するUI")] private GameObject ItemListUI;
    [SerializeField, Header("購入可能なアイテムリストを表示するText")] private Text ItemListText;


    //=== 変数宣言 ===
    private int currentDialogueIndex = 0;  // 現在の会話のインデックス
    private bool isConversationActive = false;
    private bool isPurchaseConfirmationActive = false;
    private IDisposable conversationSubscription; // 購読を管理する変数


    //=== メソッド ===
    /// <summary>
    ///・ 継承したインタラクト処理内で
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
        // 会話がすでに進行中であればインデックスをリセットしない
        if (!isConversationActive)
        {
            SetNpcName();               // NPCの名前をセット
            currentDialogueIndex = 0;  // セリフインデックスをリセット
            isConversationActive = true;

            PlayerController.StopMovement();    // 会話中はプレイヤーの移動を停止
            DisplayDialogue(InitialDialogue);   // 最初のセリフを表示

            ShowInteractUI(false); // 会話中はインタラクトUIを非表示にする

            // 前回の購読を解除（もし存在すれば）
            conversationSubscription?.Dispose();

            // 新しい購読を登録
            conversationSubscription = Observable.EveryUpdate()
                .Where(_ => isConversationActive && !PlayerController.IsMoving && Input.GetKeyDown(KeyCode.Space))
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
                        // 会話が終了したらアイテム購入確認を表示
                        if (!isPurchaseConfirmationActive)
                        {
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

        // アイテムリストを表示委
        string dialogue = "購入するアイテムを選んでください!!\n\n";
        for (int i = 0; i < shopItems.Count; i++)
        {
            dialogue += $"{i + 1}. {shopItems[i].itemName} - {shopItems[i].price}G \n ({shopItems[i].description}) \n\n";  // 価格も表示
        }
        DisplayDialogue("番号を選んでください。");
        DisplayItemList(dialogue);

        // 購読を使ってユーザーの入力を確認
        conversationSubscription?.Dispose();  // 以前の購読を解除

        // 購読を登録
        conversationSubscription = Observable.EveryUpdate()
            .Where(_ => isPurchaseConfirmationActive)
            .Subscribe(_ =>
            {
                // 数字キーの入力を動的に処理
                for (int i = 0; i < shopItems.Count; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        TryPurchaseItem(shopItems[i]);
                        break;  // アイテムを選んだらループを終了
                    }
                }
            }).AddTo(this);  // メモリリーク防止
    }

    private void TryPurchaseItem(BaseItem item)
    {
        if (PlayerStatusManager.CanAfford(item.price))
        {
            if (playerInventory.AddItem(item))
            {
                PlayerStatusManager.SpendMoney(item.price);
                Debug.Log("アイテムを購入しました！");
                DisplayDialogue("アイテムを購入しました！");
            }
            else
            {
                DisplayDialogue("インベントリがいっぱいです！");
            }
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


    /// <summary>
    /// ・会話を表示するメソッド
    /// ・会話ウィンドウとテキストを表示
    /// </summary>
    /// <param name="dialogue">表示するセリフ</param>
    private void DisplayDialogue(string dialogue)
    {
        ShowDialogueWindow(true);       
        DialogueText.text = dialogue;   
    }

    /// <summary>
    /// ・アイテムリストを表示するメソッド
    /// ・会話ウィンドウとテキストを表示
    /// </summary>
    /// <param name="dialougue">表示するアイテムテキスト</param>
    private void DisplayItemList(string dialougue)
    {
        ShowItemListDialogueWindow(true);
        ItemListText.text = dialougue;

    }

    /// <summary>
    /// ・購入可能なアイテムリストウィンドウの表示・非表示を切り替える処理
    /// </summary>
    /// <param name="isVisible">メッセージウィンドウの有効・無効</param>
    public virtual void ShowItemListDialogueWindow(bool isVisible)
    {
        ItemListUI.SetActive(isVisible);                         // ウィンドウの表示/非表示を設定
        ItemListText.gameObject.SetActive(isVisible);            // テキストの表示/非表示を設定
    }

    /// <summary>
    /// ・会話が終了した際にUIを非表示にする
    /// </summary>
    private void EndConversation()
    {
        Debug.Log("会話を終了しました・・・");
        isConversationActive = false;
        ShowDialogueWindow(false);              // 会話ウィンドウを非表示
        ShowItemListDialogueWindow(false);      // アイテムリストウィンドウを非表示
        ShowInteractUI(true);                   // インタラクトUIを再表示
        PlayerController.ResumeMovement();      // プレイヤーの移動を再開

        conversationSubscription?.Dispose();    // 購読を解除

        isPurchaseConfirmationActive = false;
    }
}

