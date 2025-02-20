using UnityEngine;
using UniRx;

public class CameraMove : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("追従するプレイヤーオブジェクト")] private Transform playerTransform;
    [SerializeField, Header("カメラの追従速度")] private float followSpeed = 5.0f;
    [SerializeField, Header("カメラのオフセット")] private Vector3 offset = new Vector3(0, 5, -10);

    //==- 変数宣言 ===
    private Vector3ReactiveProperty cameraTargetPosition = new Vector3ReactiveProperty(); // カメラの目標位置

    /// <summary>
    /// ・カメラ座標を正しい位置(Player + OffSet)に初期化する
    /// </summary>
    private void Awake()
    {
        cameraTargetPosition.Value = playerTransform.position + offset;
    }

    /// <summary>
    /// ・カメラの追従処理を設定
    /// ・プレイヤーの位置をリアクティブに監視し、カメラをスムーズに追従する
    /// </summary>
    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform が未設定です");
            return;
        }

        //


        // プレイヤーの位置を監視してカメラの目標位置を更新
        Observable.EveryUpdate()
            .Select(_ => playerTransform.position + offset) // プレイヤー位置 + オフセット
            .DistinctUntilChanged() // 前回と同じ位置なら処理しない
            .Subscribe(target =>
            {
                cameraTargetPosition.Value = target; // カメラの目標位置を更新
            })
            .AddTo(this);

        // カメラを追従する動作をサブスクライブ
        cameraTargetPosition
            .Subscribe(target =>
            {
                // スムーズにカメラを追従
                transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
            })
            .AddTo(this);
    }

}

