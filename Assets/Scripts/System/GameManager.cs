using UnityEngine;

public class GameManager : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("FPS固定の設定値")] private int targetFPS = 60;

    //=== インスタンス ===
    public static GameManager instance;

    /// <summary>
    /// 第一初期化メソッド
    /// </summary>
    private void Awake()
    {
        //--- シングルトン ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 第二初期化メソッド
    /// </summary>
    private void Start()
    {
        // FPSを固定する
        Application.targetFrameRate = targetFPS;
    }

}
