using System;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

/// <summary>
/// チェイスインタラクト(InteractBase継承)
/// 主にチェストに関するインタラクト処理をまとめてある
/// </summary>
public class ChestInteract : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("宝箱の鍵状況(ロック)")] private bool isLocked = false;
    [SerializeField, Header("開いた後のメッセージ入力")] private string openedMessage;
    [SerializeField, Header("中身がなかった時のメッセージを入力")] private string noContentsMessage;
    [SerializeField, Header("鍵がかかっていた時のメッセージを入力")] private string lockMessage;
    
    [SerializeField, Header("入手アイテムを表示するウィンドウをアタッチ")] private GameObject getItemWindow;
    [SerializeField, Header("入手アイテムを表示するImageをアタッチ")] private Image itemIconImage;
    [SerializeField, Header("入手アイテム名を表示するテキストをアタッチ")] private Text itemText;

    [SerializeField, Header("PlayerInteractをアタッチ")] private PlayerInteract playerInteract;
    [SerializeField, Header("宝箱入手アイテムデータをアタッチ")] DataItem dataItem;

    [SerializeField, Header("宝箱開封後のSpriteRendererをアタッチ")] SpriteRenderer openChestSpriteRender;
    [SerializeField, Header("宝箱開封前のSpriteをアタッチ")] Sprite ContentsChestSprite;
    [SerializeField, Header("宝箱開封後のSpriteをアタッチ")] Sprite noContentsChestSprite;
    [SerializeField, Header("宝箱 開封フラグ(デバッグ)")] private bool isOpen = false;                     // 宝箱の状況を確認するフラグ(中身あり || 空)
    
    //=== 変数宣言 ===
    //private int currentDialougueIndex = 0;          // 現在の会話インデックス
    private IDisposable conversationSubscription;   // 購読を管理する変数

    //=== メソッド ===

    /// <summary>
    /// ・チェストの開閉状態によってSpriteを差し替える
    /// </summary>
    private void Start()
    {
            this.ObserveEveryValueChanged(_ => isOpen)
        .Subscribe(_ =>
        {
            if (_)
            {// 宝箱が開いていた場合
                ChangeChestSprite(noContentsChestSprite);   // 宝箱 画像を差し替え(開)
            }
            else
            {// 宝箱が開いていない場合
                ChangeChestSprite(ContentsChestSprite);     // 宝箱 画像を差し替え(閉)
            }

        })
        .AddTo(this);
    }

    /// <summary>
    ///・ 継承したインタラクト処理内で
    /// 　このNPCが会話した時のメソッドを呼び出す
    /// </summary>
    public override void InteractProcess()
    {
        StartEvent();
    }

    /// <summary>
    /// ・宝箱イベント
    /// </summary>
    private void StartEvent()
    {
        SetNpcName();   // ウィンドウテキストの名前をセット
        //currentDialougueIndex = 0;  // セリフのインデックスをリセット
        PlayerStateManager.instance.IsInConversation = true;
        inventory.ShowInventoryUI();

        PlayerController.StopMovement();    // 会話中はプレイヤーの移動を停止

        if (isOpen)
        {// 宝箱が開いている場合
            ShowInteractUI(false);  // 会話中はインタラクトUIを非表示にする
            DisplayDialougue(noContentsMessage);  // 最初のセリフを表示

            // 前回の購読を解除(もし存在すれば)
            conversationSubscription?.Dispose();

            // 2秒後にアイテム入手ウィンドウを閉じて会話終了
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => EndCinversation())
                .AddTo(this);

        }
        else
        {// 宝箱が開いていない場合

            ShowInteractUI(false);  // 会話中はインタラクトUIを非表示にする
            DisplayDialougue(InitialDialogue);  // 最初のセリフを表示

            // 前回の購読を解除(もし存在すれば)
            conversationSubscription?.Dispose();

            // 新しい購読を登録
            conversationSubscription = Observable.EveryUpdate()
                .Where(_ => PlayerStateManager.instance.IsInConversation &&
                !PlayerStateManager.instance.IsChoice &&
                !PlayerController.IsMoving &&
                playerInteract.interactAction.triggered)
                .First()            // 一度のみ実行
                .Subscribe(_ =>
                {
                    TryOpen();  // 宝箱を開ける処理

                })
                .AddTo(this);


        }
    }

    /// <summary>
    /// ・宝箱のSpriteを変更する
    /// </summary>
    private void ChangeChestSprite(Sprite chestSprite)
    {
        openChestSpriteRender.sprite = chestSprite;
    }

    /// <summary>
    /// ・宝箱を開く処理
    /// └ ケース１：鍵が閉まっている場合
    /// └ ケース２：鍵が開いていた場合
    /// </summary>
    private void TryOpen()
    {
        if (isLocked)
        {// ロックがかかっていた場合
            Debug.Log("ロックされています");

            DialogueText.text = lockMessage;    // ウィンドウテキストを書き換え

            // 2秒後にアイテム入手ウィンドウを閉じて会話終了
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => EndCinversation())
                .AddTo(this);

        }
        else
        {// ロックがかかっていない場合
            Debug.Log($"アイテムを開きます: {InteractableName}");
            Debug.Log(dataItem.itemName + openedMessage);  // アイテム開放後のメッセージを表示
            ShowInteractUI(false);  // UIを非表示にする
            DisplayItemWindow(true);
            isOpen = true;

            inventory.AddItem(dataItem);            // アイテムをインベントリに追加(スタック)する
            itemIconImage.sprite = dataItem.icon;   // 入手アイテムのアイコンを表示 Imageにセット
            itemText.text = dataItem.itemName + openedMessage;      // 入手アイテムのアイコンを表示 Textにセット

            // 2秒後にアイテム入手ウィンドウを閉じて会話終了
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ => EndCinversation())
                .AddTo(this);
        }
    }

    /// <summary>
    /// ・会話ウィンドウとテキストを表示
    /// </summary>
    /// <param name="dialogue">表示するセリフ</param>
    private void DisplayDialougue(string dialogue)
    {
        ShowDialogueWindow(true);       // 会話ウィンドウとテキスト表示
        DialogueText.text = dialogue;   // テキストをセット
    }

    /// <summary>
    /// ・入手アイテムウィンドウ 表示・非表示処理
    /// </summary>
    /// <param name="display"></param>
    private void DisplayItemWindow(bool display)
    {
        getItemWindow.SetActive(display);          
    }

    
    private void EndCinversation()
    {
        Debug.Log("会話を終了しました・・・");
        PlayerStateManager.instance.IsInConversation = false;
        PlayerStateManager.instance.IsChoice = false;
        DisplayItemWindow(false);                   // アイテム入手ウィンドウを非表示
        ShowDialogueWindow(false);                  // 会話ウィンドウを非表示
        PlayerController.ResumeMovement();          // プレイヤーの移動を再開

        conversationSubscription?.Dispose();        // 購読を解除
    }
}
