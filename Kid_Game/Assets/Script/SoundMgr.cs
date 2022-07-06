using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundMgr : SingletonMono<SoundMgr>
{
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            AwakeAfter();
        }
    }

    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    public AudioClip[] BGMClip; // ����� �ҽ��� ����.
    public AudioClip[] SFXClip; // ����� �ҽ��� ����.

    Dictionary<string, AudioClip> SFXaudioClipsDic;
    Dictionary<string, AudioClip> BGMaudioClipsDic;
    AudioSource sfxPlayer;
    AudioSource bgmPlayer;

    [SerializeField]
    GameObject OtherPanel;

    void AwakeAfter()
    {
        sfxPlayer = GetComponent<AudioSource>();
        SetupBGM();

        SFXaudioClipsDic = new Dictionary<string, AudioClip>();
        BGMaudioClipsDic = new Dictionary<string, AudioClip>();
        foreach (AudioClip sfx in SFXClip)
        {
            SFXaudioClipsDic.Add(sfx.name, sfx);
        }

        foreach(AudioClip bgm in BGMClip)
        {
            BGMaudioClipsDic.Add(bgm.name, bgm);
        }
    }

    void SetupBGM()
    {
        GameObject child = new GameObject("BGM");
        child.transform.SetParent(transform);
        bgmPlayer = child.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
        bgmPlayer.clip = BGMClip[0];
        bgmPlayer.volume = masterVolumeBGM;
    }

    private void Start()
    {
        if (bgmPlayer != null)
            bgmPlayer.Play();
    }

    private void Update()
    {
        if (!OtherPanel)
        {
            OtherPanel = GameObject.Find("OtherPanel").gameObject;
            OtherPanel.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Slider>().value = masterVolumeBGM;
        }

        else
            SetVolumes();
    }

    public bool IsBGMPlaying()
    {
        return bgmPlayer ? bgmPlayer.isPlaying : false;
    }

    // �� �� ��� : ���� �Ű������� ����
    public void PlaySound(string a_name, float a_volume = 1f)
    {
        if (SFXaudioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return;
        }

        Debug.Log(a_volume * masterVolumeSFX);
        sfxPlayer.PlayOneShot(SFXaudioClipsDic[a_name], a_volume * masterVolumeSFX);
    }

    // �������� �� �� ��� : ���� �Ű������� ����
    public void PlayRandomSound(string[] a_nameArray, float a_volume = 1f)
    {
        string l_playClipName;

        l_playClipName = a_nameArray[Random.Range(0, a_nameArray.Length)];

        if (SFXaudioClipsDic.ContainsKey(l_playClipName) == false)
        {
            Debug.Log(l_playClipName + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(SFXaudioClipsDic[l_playClipName], a_volume * masterVolumeSFX);
    }

    // �����Ҷ��� ���ϰ��� GameObject�� �����ؼ� �����Ѵ�. ���߿� �ɼǿ��� ���� ũ�� �����ϸ� �̰� ���� �����ؼ� �ٲ�����..
    public GameObject PlayLoopSound(string a_name)
    {
        if (SFXaudioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return null;
        }

        bgmPlayer.clip = SFXaudioClipsDic[a_name];
        bgmPlayer.volume = masterVolumeSFX;
        bgmPlayer.loop = true;
        bgmPlayer.Play();

        // GameObject l_obj = new GameObject("LoopSound");
        // AudioSource source = l_obj.AddComponent<AudioSource>();
        // source.clip = audioClipsDic[a_name];
        return null;
    }

    public void ChangeBGM(string a_name)
    {
        bgmPlayer.Stop();

        if (BGMaudioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return;
        }
        bgmPlayer.clip = BGMaudioClipsDic[a_name];
        bgmPlayer.Play();
    }

    // �ַ� ���� ����� ������ ����.
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    #region �ɼǿ��� ��������
    public void SetVolumeSFX(float a_volume)
    {
        masterVolumeSFX = a_volume;
    }

    public void SetVolumeBGM(float a_volume)
    {
        masterVolumeBGM = a_volume;
        bgmPlayer.volume = masterVolumeBGM;
    }

    public void SetVolumes() // SFX, BGM 둘다 적용
    {
        masterVolumeSFX = OtherPanel.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Slider>().value;
        masterVolumeBGM = OtherPanel.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Slider>().value;

        bgmPlayer.volume = masterVolumeBGM;
    }
    #endregion
}
