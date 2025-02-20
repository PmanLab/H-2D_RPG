using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // SEの種類
    [HideInInspector]
    public enum E_SE
    {
        //=== UI系 ===
        Confirm,        //決定
        Cancel,         //戻る
        //=== ゲーム本編 ===
        ClearPuzzle,    //パズル完成
        SlideDoor,      //扉：スライド
        GrabObject,     //オブジェクト：つかむ
        ReleaseObject,  //オブジェクト：はなす
        GrabPaper,      //指示票を手に取る
        GoalFootStep,   //ゴール時に鳴らす足音
        LockOpenDoor,   //扉：開錠
        NewGuideWrite,  //新規支持票生成
        GetItem,        //アイテム取得
    }

    //=== シリアライズ ===
    //--- オーディオソースとミキサ ---
    [SerializeField, Header("オーディオミキサー")] AudioMixer audioMixer;
    [SerializeField, Header("オーディオソース(BGM)")] AudioSource bgmAudioSource;
    [SerializeField, Header("オーディオソース(SE)")] List<AudioSource> seAudioSource;

    //--- スライダ ---
    [SerializeField, Header("オーディオスライダの指定(Master)")] Slider masterAudioSlider;
    [SerializeField, Header("オーディオスライダの指定(BGM)")] Slider bgmSlider;
    [SerializeField, Header("オーディオスライダの指定(SE)")] Slider seSlider;

    //--- SEリスト ---
    [SerializeField, Header("SEを流すα値")] float seTiming_ = 0.5f;

    // シングルトンのインスタンス
    public static AudioManager instance;

    private void Awake()
    {
        // シングルトンインスタンスの設定
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);    // 重複時削除
        }
    }

    // 初期化処理
    private void Start()
    {
        // 初期化
        bgmAudioSource.UnPause();

        // スライダーの初期値を設定
        masterAudioSlider.value = PlayerPrefs.GetFloat(ConstantManager.MasterVolumeKey, 1.0f);
        bgmSlider.value = PlayerPrefs.GetFloat(ConstantManager.BGMVolumeKey, 1.0f);
        seSlider.value = PlayerPrefs.GetFloat(ConstantManager.SEVolumeKey, 1.0f);

        // スライダー変更時のリスナーを設定
        masterAudioSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        seSlider.onValueChanged.AddListener(OnSEVolumeChanged);

        // スライダーの値に基づいて音量を設定
        OnMasterVolumeChanged(masterAudioSlider.value);
        OnBGMVolumeChanged(bgmSlider.value);
        OnSEVolumeChanged(seSlider.value);
    }

    private void OnMasterVolumeChanged(float value)
    {
        //=== 主音量 ===
        value = Mathf.Clamp01(value);
        float decibel = 20.0f * Mathf.Log10(value);
        decibel = Mathf.Clamp(decibel, -80.0f, 0.0f);
        // オーディオミキサーの "Master" パラメータを更新
        audioMixer.SetFloat("Master", decibel);

        PlayerPrefs.SetFloat(ConstantManager.MasterVolumeKey, value);   // 情報保存
    }

    private void OnBGMVolumeChanged(float value)
    {
        //=== BGM ===
        value = Mathf.Clamp01(value);
        // 変化するのは-80～0の間
        float decibel = 20.0f * Mathf.Log10(value);
        decibel = Mathf.Clamp(decibel, -80.0f, 0.0f);
        audioMixer.SetFloat("BGM", decibel);

        PlayerPrefs.SetFloat(ConstantManager.BGMVolumeKey, value);  // 情報保存
    }

    private void OnSEVolumeChanged(float value)
    {
        value = Mathf.Clamp01(value);
        // 変化するのは-80～0の間
        float decibel = 20.0f * Mathf.Log10(value);
        decibel = Mathf.Clamp(decibel, -80.0f, 0.0f);
        audioMixer.SetFloat("SE", decibel);

        PlayerPrefs.SetFloat(ConstantManager.SEVolumeKey, value);   // 情報保存
    }

    //--- BGM再生 処理 ---
    public void PlayBGM()
    {
        bgmAudioSource.Play();
    }

    //--- BGM停止 処理 ---
    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    //--- BGM一時停止 処理 ---
    public void PauseBGM()
    {
        bgmAudioSource.Pause();
    }

    //--- BGM再生処理 ---
    public void UnPauseBGM()
    {
        bgmAudioSource.UnPause();
    }

    //--- SE再生 処理 ---
    public void SePlay(E_SE _se)
    {
        seAudioSource[(int)_se].Play();
    }

    //--- SE停止 処理 ---
    public void SeStop(E_SE _se)
    {
        seAudioSource[(int)_se].Stop();
    }

    //--- SE一時停止 処理 ---
    public void SePause(E_SE _se)
    {
        seAudioSource[(int)_se].Pause();
    }

    //--- SE一時停止解除 処理 ---
    public void SeUnPause(E_SE _se)
    {
        seAudioSource[(int)_se].UnPause();
    }

}
