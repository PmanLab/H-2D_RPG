using UnityEngine;

public class GameManager : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("FPS固定の設定値")] private int targetFPS = 60;

    //=== インスタンス ===
    public static GameManager instance;     // シングルトン用

    //=== メソッド ===
    /// <summary>
    /// ・シングルトン生成処理(GameManager)
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
    /// ・FPS値を固定する処理
    /// </summary>
    private void Start()
    {
        Application.targetFrameRate = targetFPS;
    }

}
