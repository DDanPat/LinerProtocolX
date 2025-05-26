using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Button")]
    [SerializeField] private Button bgmMuteButton;
    [SerializeField] private Button sfxMuteButton;

    [Header("Icon")]
    [SerializeField] private Image bgmIcon;
    [SerializeField] private Image sfxIcon;

    [Header("Sound Sprite")]
    [SerializeField] private Sprite iconSoundOn;
    [SerializeField] private Sprite iconSoundOff;

    private const float MinVolume = 0.0001f;

    private void Start()
    {
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        bgmSlider.onValueChanged.AddListener((v) =>
        {
            SoundManager.Instance.SetBGMVolume(v);
            PlayerPrefs.SetFloat("BGMVolume", v);
            UpdateIcon(bgmIcon, v, SoundManager.Instance.IsBGMMuted);
        });

        sfxSlider.onValueChanged.AddListener((v) =>
        {
            SoundManager.Instance.SetSFXVolume(v);
            PlayerPrefs.SetFloat("SFXVolume", v);
            UpdateIcon(sfxIcon, v, SoundManager.Instance.IsSFXMuted);
        });

        bgmMuteButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ToggleBGMMute();
            UpdateIcon(bgmIcon, bgmSlider.value, SoundManager.Instance.IsBGMMuted);
        });

        sfxMuteButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ToggleSFXMute();
            UpdateIcon(sfxIcon, sfxSlider.value, SoundManager.Instance.IsSFXMuted);
        });

        SoundManager.Instance.SetBGMVolume(bgm);
        SoundManager.Instance.SetSFXVolume(sfx);

        UpdateIcon(bgmIcon, bgm, SoundManager.Instance.IsBGMMuted);
        UpdateIcon(sfxIcon, sfx, SoundManager.Instance.IsSFXMuted);
    }

    private void UpdateIcon(Image icon, float volume, bool isMuted)
    {
        bool isEffectivelyMuted = isMuted || volume <= MinVolume;
        icon.sprite = isEffectivelyMuted ? iconSoundOff : iconSoundOn;
    }
}
