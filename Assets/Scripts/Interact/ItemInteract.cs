using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemInteract : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("開けられるかどうか")] private bool isLocked;
    [SerializeField, Header("開いた後のメッセージ")] private string openedMessage;

    public override void Interact()
    {
        if(isLocked)
        {
            Debug.Log("ロックされています");
            // 鍵が必要な場合の処理や開いた時の処理を書く↓
        }
        else
        {
            Debug.Log($"アイテムを開きます: {InteractableName}");
            Debug.Log(openedMessage);  // アイテム開放後のメッセージを表示
            ShowInteractUI(false);  // UIを非表示にする
        }
    }

}
