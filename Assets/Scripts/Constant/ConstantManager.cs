using UnityEngine;

/// <summary>
/// ・定義をまとめたクラス
/// </summary>
public class ConstantManager : MonoBehaviour
{
    //=== 定数 ===
    public const int interactWaitingTime = 2;   // インタラクト待機時間

    //=== ボリュームキー ===
    public const string MasterVolumeKey = "MasterVolume";
    public const string BGMVolumeKey = "BGMVolume";
    public const string SEVolumeKey = "SEVolume";


    //=== 解像度 ===
    public static readonly Vector2Int Resolution1920x1080 = new Vector2Int(1920, 1080);
    public static readonly Vector2Int Resolution1600x900 = new Vector2Int(1600, 900);
    public static readonly Vector2Int Resolution1280x720 = new Vector2Int(1280, 720);

    //=== macOS用の解像度 ===
    public static readonly Vector2Int Resolution1920x1200 = new Vector2Int(1920, 1200);
    public static readonly Vector2Int Resolution1680x1050 = new Vector2Int(1680, 1050);
    public static readonly Vector2Int Resolution1280x800 = new Vector2Int(1280, 800);

}
