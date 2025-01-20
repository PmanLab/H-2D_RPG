using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonManager : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("GameStateManagerをアタッチ")] private GameStateManager gameStateManager;
    [SerializeField, Header("NPCInteract_ItemShop(対応させるNPC)をアタッチ")] NPCInteract_ItemShop npcInteract_ItemShop;
    [SerializeField, Header("NPCInteract_InnKeeper(対応させるNPC)をアタッチ")] NPCInteract_InnKeeper npcInteract_InnKeeper;
    [SerializeField, Header("リスト選択番号を入力")] private int currentIndexNumber = 0;


    //=== メソッド ===
    /// <summary>
    /// ・指定したボタンを初期選択状態にする
    /// </summary>
    /// <param name="firstPickButton">初期選択状態にするボタンを割り当てる</param>
    public void FirstPickButton(GameObject firstPickButton)
    {
        EventSystem.current.SetSelectedGameObject(firstPickButton);
    }

    /// <summary>
    /// ・シーンを移動する処理
    /// </summary>
    /// <param name="sceneName">移動先シーン名</param>

    public void LoadScene(string sceneName)
    {
         SceneManager.LoadScene(sceneName);
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
        gameStateManager.IsInPause = false;
    }

    /// <summary>
    /// ・指定したインデックスのアイテムを購入する
    /// </summary>
    public void BuyShopButton()
    {
         npcInteract_ItemShop.IndexButtonClose();
         npcInteract_ItemShop.TryPurchaseItem(currentIndexNumber);
    }
    
    /// <summary>
    /// ・購入ボタンを非表示にする
    /// </summary>
    public void CancelShopButton()
    {
        npcInteract_ItemShop.ShopCloseButton();
    }

    /// <summary>
    /// ・宿に泊まる事を承認し、宿泊の処理を行う
    /// </summary>
    public void InnApplyButton()
    {
        npcInteract_InnKeeper.ApplyCloseButton();
        npcInteract_InnKeeper.ApplyInn();
    }


    /// <summary>
    /// ・宿に泊まることを拒否し、承認ボタンを非表示にする
    /// </summary>
    public void UnnApplyButton()
    {
        npcInteract_InnKeeper.ApplyCloseButton();
        npcInteract_InnKeeper.UnnApplyInn();
    }
}
