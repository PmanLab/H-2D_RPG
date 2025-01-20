using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムインタラクト(InteractBase継承)
/// 主にアイテムに関するインタラクト処理をまとめてある
/// </summary>
public class ItemInteract : InteractBase
{
    //=== シリアライズ ===
    [SerializeField, Header("開けられるかどうか")] private bool isLocked;
    [SerializeField, Header("開いた後のメッセージ")] private string openedMessage;
    
    //=== メソッド ===
    /// <summary>
    /// ・宝箱を開ける時の処理
    /// </summary>
    public override void InteractProcess()
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
