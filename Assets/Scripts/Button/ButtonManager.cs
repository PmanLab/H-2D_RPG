using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    /// <summary>
    /// シーンロードメソッド 
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName);
    }

    /// <summary>
    /// アプリケーション終了メソッド
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
    /// </summary>
    /// <param name="gameObject"></param>
    public void ShowGameObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// オブジェクトActiveFalseメソッド
    /// </summary>
    /// <param name="gameObject"></param>
    public void HideGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ポーズ解除専用メソッド
    /// </summary>
    public void UnPause()
    {
        GameStateManager.instance.EndPaused();
    }
}