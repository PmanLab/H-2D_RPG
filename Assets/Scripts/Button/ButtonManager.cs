using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("NPCInteract_ItemShop(対応させる店員)をアタッチ")] NPCInteract_ItemShop npcInteract_ItemShop;
    [SerializeField, Header("リスト選択番号を入力")] private int currentIndexNumber = 0;

    //=== メソッド ===
    /// <summary>
    /// ・シーンを移動する処理
    /// </summary>
    /// <param name="sceneName">移動先シーン名</param>

    public void LoadScene(string sceneName)
    {
         LoadScene(sceneName);
    }


    /// <summary>
    /// ・アプリケーションを終了させる処理
    /// </summary>
    public void EndButton()
    {
#if UNITY_EDITOR
       EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    /// <summary>
    /// ・オブジェクトを有効(Active(true))にする処理
    /// </summary>
    /// <param name="gameObject">有効にしたいオブジェクトを指定</param>
    public void ShowGameObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// ・オブジェクトを無効(Active(false))にする処理
    /// </summary>
    /// <param name="gameObject">無効にしたいオブジェクトを指定</param>
    public void HideGameObject(GameObject gameObject)
    {
         gameObject.SetActive(false);
    }

    /// <summary>
    /// ・Pause状態を解除する処理
    /// </summary>
    public void UnPause()
    {
         GameStateManager.instance.EndPaused();
    }

    /// <summary>
    /// ・指定したインデックスのアイテムを購入する
    /// </summary>
    public void BuyShopButton()
    {
         npcInteract_ItemShop.TryPurchaseItem(currentIndexNumber);
         npcInteract_ItemShop.IndexCloseButton();
    }
    
    /// <summary>
    /// ・指定した
    /// </summary>
    public void CancelShopButton()
    {
        npcInteract_ItemShop.ShopCloseButton();
    }
}
