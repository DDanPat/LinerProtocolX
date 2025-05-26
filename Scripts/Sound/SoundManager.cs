using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMType
{
    None,
    Main,
    Battle,
    // 필요시 추가
}
public enum SFXType
{
    ButtonClick,
    Gacha,
    EnemyDie,
    TURRETSHOOT_SHOTGUN,
    TURRETSHOOT_GUNTURRET,
    TURRETSHOOT_RAILGUN,
    TURRETSHOOT_FIREGUN,
    TURRETSHOOT_GRANADE,
    TURRETSHOOT_MISSILE,
    TURRETSHOOT_TESLACOIL,
    TURRETSHOOT_LAYSERTURRET,
    TURRETSHOOT_ACCELERATETURRET,
    TURRETSHOOT_PLASMATOWER,
    TURRETSHOOT_ENERGYSTOMTURRET,
    TURRETSHOOT_PLASMATURRET,
    EXPLOSION_1111111,
    // 필요시 추가
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Params")]
    private const string BGMVolumeParam = "BGMVolume";
    private const string SFXVolumeParam = "SFXVolume";

    [SerializeField]private float lastBgmVolume = 0.4f;
    [SerializeField] private float lastSfxVolume = 0.4f;

    private bool isBgmMuted = false;
    public bool IsBGMMuted => isBgmMuted;

    private bool isSfxMuted = false;
    public bool IsSFXMuted => isSfxMuted;

    [Header("Audio Clips")]
    public List<BGMData> bgmClips;
    public List<SFXData> sfxClips;

    private Dictionary<BGMType, List<AudioClip>> bgmDict = new();
    private Dictionary<SFXType, AudioClip> sfxDict = new();

    private List<AudioClip> bgmList = new List<AudioClip>();


    [System.Serializable]
    public class BGMData
    {
        public BGMType type;
        public AudioClip clip;
    }

    [System.Serializable]
    public class SFXData
    {
        public SFXType type;
        public AudioClip clip;
    }

    private void Start()
    {
        Initialize();
        PlayBGM(BGMType.Main); // BGM 잠시 꺼둠
    }

    private void Initialize()
    {
        foreach (var data in bgmClips)
        {
            if (!bgmDict.ContainsKey(data.type))
                bgmDict[data.type] = new List<AudioClip>();

            bgmDict[data.type].Add(data.clip);
        }
        foreach (var data in sfxClips) sfxDict[data.type] = data.clip;
    }

    public void PlayBGM(BGMType type, bool loop = true)
    {
        if (bgmDict.TryGetValue(type, out List<AudioClip> bgmList))
        {
            int randomBGM = Random.Range(0, bgmList.Count);
            bgmSource.clip = bgmList[randomBGM];
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM() => bgmSource.Stop();

    public void PlaySFX(SFXType type)
    {
        if (sfxDict.TryGetValue(type, out AudioClip clip))
            sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        lastBgmVolume = volume;
        if (!isBgmMuted)
            audioMixer.SetFloat(BGMVolumeParam, Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        lastSfxVolume = volume;
        if (!isSfxMuted)
            audioMixer.SetFloat(SFXVolumeParam, Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void ToggleBGMMute()
    {
        isBgmMuted = !isBgmMuted;

        if (isBgmMuted)
            audioMixer.SetFloat(BGMVolumeParam, -80f);
        else
        {
            SetBGMVolume(lastBgmVolume);
            if (!bgmSource.isPlaying && bgmSource.clip != null)
                bgmSource.Play();
        }
    }

    public void ToggleSFXMute()
    {
        isSfxMuted = !isSfxMuted;

        if (isSfxMuted)
            audioMixer.SetFloat(SFXVolumeParam, -80f);
        else
            SetSFXVolume(lastSfxVolume);
    }

    public void PlayBGMForScene(string sceneName)
    {
        BGMType bgmType = BGMType.None; // 기본값 (None을 미리 enum에 추가해두는 게 안전합니다.)

        switch (sceneName)
        {
            case "MainScenes":
                bgmType = BGMType.Main;
                break;
            case "BattleScenes":
            case "TestBattleScenes":
                bgmType = BGMType.Battle;
                break;
            case "TutorialScenes":
                bgmType = BGMType.Battle;
                break;
            // 필요한 만큼 추가
            default:
                Debug.LogWarning($"씬 {sceneName}에 대한 BGM 설정이 없습니다.");
                break;
        }

        PlayBGM(bgmType);
    }
}

