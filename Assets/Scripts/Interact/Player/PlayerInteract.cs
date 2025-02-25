﻿using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーインタラクト(InteractBase継承)
/// プレイヤーが行うインタラクト操作に関する事をまとめてある
/// </summary>
public class PlayerInteract : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("インタラクト可能な距離")] private float interactRange = 2.0f;  // インタラクト可能な範囲
    [SerializeField, Header("インタラクト用のレイヤー")] private LayerMask interactableLayer;  // インタラクト可能なオブジェクトのレイヤー
    [SerializeField, Header("PlayerInputをアタッチ")] private PlayerInput playerInput;

    //=== 変数宣言 ===
    public InputAction interactAction { get; set; }     // "Interact"アクションを保持する変数
    public InputAction inventoryAction { get; set; }    // "Inventory"アクションを保持する変数

    /// <summary>
    /// インタラクションメソッド
    /// </summary>
    public override void InteractProcess(){}

    /// <summary>
    /// ・PlayerInputからインタラクトアクションを取得し、
    /// インタラクト処理の監視を開始する
    /// </summary>
    private void Start()
    {
        //--- InputActionを取得 ---
        interactAction = playerInput.actions["Interact"]; // PlayerInputから「Interact」アクションを取得
        inventoryAction = playerInput.actions["Inventory"]; // PlayerInputから「Inventory」アクションを取得

        //--- インタラクト処理 ---
        Observable.EveryUpdate()
            .Where(_ => interactAction.triggered && CanInteract() && !inventory.isShowInventoryUI)
            .Subscribe(_ => TryInteract())
            .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => inventoryAction.triggered)
            .Subscribe(_ => inventory.ShowInventoryUI())
            .AddTo(this);

        //--- UIの表示/非表示を分けて処理 ---
        Observable.EveryUpdate()
            .Where(_ => CanInteract())
            .Subscribe(_ => ShowInteractUI(true))
            .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => !CanInteract())
            .Subscribe(_ => ShowInteractUI(false))
            .AddTo(this);
    }

    /// <summary>
    /// ・プレイヤーがインタラクトできる範囲内にいるか確認する
    /// </summary>
    /// <returns>インタラクトできる場合はtrue、できない場合はfalseを返す</returns>
    private bool CanInteract()
    {
        // プレイヤーの周囲にインタラクト可能なオブジェクトがあるかを判定
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        return colliders.Length > 0 && !DialogueWindow.activeSelf;  // 会話ウィンドウが表示中でない場合にインタラクト可能
    }

    /// <summary>
    /// ・インタラクトできるオブジェクトに対して処理を行う
    /// </summary>
    private void TryInteract()
    {
        // プレイヤーの周囲にインタラクト可能なオブジェクトがあれば処理を実行
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        foreach (var collider in colliders)
        {
            InteractBase interactable = collider.GetComponent<InteractBase>();
            if (interactable != null)  // InteractBaseが存在すればインタラクト処理を実行
            {
                interactable.Interact();  // インタラクト処理を呼び出す
                break;  // 最初のインタラクト可能オブジェクトでインタラクトを実行したらループを抜ける
            }
        }
    }
}
