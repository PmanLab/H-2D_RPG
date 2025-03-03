using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
    //=== シリアライズ ===
    [SerializeField, Header("フルスクリーン/ウィンドウモード用ドロップダウン")]
    private Dropdown fullscreenDropdown;

    [SerializeField, Header("解像度用ドロップダウン")]
    private Dropdown resolutionDropdown;

    //=== 初期化メソッド ===
    void Start()
    {
        // 現在のフルスクリーンモードをドロップダウンに反映
        fullscreenDropdown.value = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) ? 0 : 1;

        // プラットフォームによる解像度設定
        SetResolutionOptions();

        // ドロップダウンの選択肢が変更されたときに呼び出すメソッドを登録
        fullscreenDropdown.onValueChanged.AddListener(delegate { ChangeFullscreenMode(fullscreenDropdown.value); });
        resolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(resolutionDropdown.value); });
    }

    // プラットフォームに応じた解像度設定メソッド
    private void SetResolutionOptions()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            // Windows用の解像度設定
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(new List<string> { "1920x1080", "1600x900", "1280x720" });
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            // macOS用の解像度設定
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(new List<string> { "1920x1200", "1680x1050", "1280x800" });
        }

        // 現在の解像度をドロップダウンに反映
        UpdateResolutionDropdown();
    }

    // 現在の解像度をドロップダウンに反映するメソッド
    private void UpdateResolutionDropdown()
    {
        // 16:9の解像度
        if (Screen.width == ConstantManager.Resolution1920x1080.x && Screen.height == ConstantManager.Resolution1920x1080.y)
        {
            resolutionDropdown.value = 0; // 1920x1080
        }
        else if (Screen.width == ConstantManager.Resolution1600x900.x && Screen.height == ConstantManager.Resolution1600x900.y)
        {
            resolutionDropdown.value = 1; // 1600x900
        }
        else if (Screen.width == ConstantManager.Resolution1280x720.x && Screen.height == ConstantManager.Resolution1280x720.y)
        {
            resolutionDropdown.value = 2; // 1280x720
        }
        // 16:10の解像度
        else if (Screen.width == ConstantManager.Resolution1920x1200.x && Screen.height == ConstantManager.Resolution1920x1200.y)
        {
            resolutionDropdown.value = 0; // 1920x1200
        }
        else if (Screen.width == ConstantManager.Resolution1680x1050.x && Screen.height == ConstantManager.Resolution1680x1050.y)
        {
            resolutionDropdown.value = 1; // 1680x1050
        }
        else if (Screen.width == ConstantManager.Resolution1280x800.x && Screen.height == ConstantManager.Resolution1280x800.y)
        {
            resolutionDropdown.value = 2; // 1280x800
        }
    }

    // フルスクリーン/ウィンドウモード変更メソッド
    private void ChangeFullscreenMode(int _index)
    {
        switch (_index)
        {
            case 0: // フルスクリーンモード
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case 1: // ウィンドウモード
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    // 解像度変更メソッド
    private void ChangeResolution(int _index)
    {
        switch (_index)
        {
            case 0: // 1920x1080 or 1920x1200
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    Screen.SetResolution(ConstantManager.Resolution1920x1080.x, ConstantManager.Resolution1920x1080.y, Screen.fullScreen);
                }
                else
                {
                    Screen.SetResolution(ConstantManager.Resolution1920x1200.x, ConstantManager.Resolution1920x1200.y, Screen.fullScreen);
                }
                break;

            case 1: // 1600x900 or 1680x1050
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    Screen.SetResolution(ConstantManager.Resolution1600x900.x, ConstantManager.Resolution1600x900.y, Screen.fullScreen);
                }
                else
                {
                    Screen.SetResolution(ConstantManager.Resolution1680x1050.x, ConstantManager.Resolution1680x1050.y, Screen.fullScreen);
                }
                break;

            case 2: // 1280x720 or 1280x800
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    Screen.SetResolution(ConstantManager.Resolution1280x720.x, ConstantManager.Resolution1280x720.y, Screen.fullScreen);
                }
                else
                {
                    Screen.SetResolution(ConstantManager.Resolution1280x800.x, ConstantManager.Resolution1280x800.y, Screen.fullScreen);
                }
                break;
        }
    }
}
