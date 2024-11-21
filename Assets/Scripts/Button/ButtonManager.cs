using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    /// <summary>
    /// シーンロードメソッド 
    /// 
    /// ボタン押下自にシーンを移動する処理
    /// 引数：(シーン名：string)
    /// 
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName);
    }

    /// <summary>
    /// アプリケーション終了メソッド
    /// 
    /// ボタン押下自にアプリケーションを終了させる処理
    /// 
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
    /// オブジェクトActiveTrueメソッド
    /// 
    /// オブジェクトを有効(Active(true))にする処理
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    public void ShowGameObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// オブジェクトActiveFalseメソッド
    /// 
    /// オブジェクトを無効(Active(false))にする処理
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    public void HideGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ポーズ解除専用メソッド
    /// 
    /// Pause状態を解除する処理
    /// 
    /// </summary>
    public void UnPause()
    {
        GameStateManager.instance.EndPaused();
    }
}