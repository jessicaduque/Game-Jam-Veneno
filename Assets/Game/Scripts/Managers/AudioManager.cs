using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils.Singleton;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Sounds")]
    [SerializeField] private List<Sound> musics = new List<Sound>();
    [SerializeField] private List<Sound> sfx = new List<Sound>();

    private AudioSource _musicSource1;
    private AudioSource _musicSource2;
    private AudioSource _sfxSource;

    public int EnabledSFX { get; private set; }
    public int EnabledMusicLevel { get; private set; }

    private const string keyMixerEffects = "Sfx";
    private const string keyMixerMusic = "Music";

    private bool _musicSource1IsPlaying;

    protected override void Awake()
    {
        base.Awake();

        InitSetup();
    }

    private void Start()
    {
        EnabledSFX = PlayerPrefs.HasKey(keyMixerEffects) ? PlayerPrefs.GetInt(keyMixerEffects) : 1;
        EnabledMusicLevel = PlayerPrefs.HasKey(keyMixerMusic) ? PlayerPrefs.GetInt(keyMixerMusic) : 1;

        SetupMixer(keyMixerEffects, EnabledSFX);
        SetupMixer(keyMixerMusic, EnabledMusicLevel);

        //Initial menu music
        //FadeInMusic("Menu");
    }

    private void SetupMixer(string mixer, int enabled)
    {
        if (enabled == 1)
        {
            masterMixer.SetFloat(mixer, 0f);
        }
        else
        {
            masterMixer.SetFloat(mixer, -80f);
        }
    }

    private void InitSetup()
    {
        _musicSource1 = new GameObject("Music_1").AddComponent<AudioSource>();
        _musicSource1.gameObject.transform.SetParent(transform);
        _musicSource1.outputAudioMixerGroup = musicGroup;
        _musicSource1.playOnAwake = false;

        _musicSource2 = new GameObject("Music_2").AddComponent<AudioSource>();
        _musicSource2.gameObject.transform.SetParent(transform);
        _musicSource2.outputAudioMixerGroup = musicGroup;
        _musicSource2.playOnAwake = false;

        _sfxSource = new GameObject("SFX").AddComponent<AudioSource>();
        _sfxSource.gameObject.transform.SetParent(transform);
        _sfxSource.outputAudioMixerGroup = sfxGroup;
        _sfxSource.playOnAwake = false;

    }

    public void PlaySfx(string soundName)
    {
        Sound sound = GetSFX(soundName);

        if (sound != null)
        {
            SetupSfxAudioSource(sound.volume, sound.pitch);
            _sfxSource.PlayOneShot(sound.clip);
        }
    }

    public void StopSfx()
    {
        _sfxSource.Stop();
    }

    public void PlaySfx(AudioClip audioClip, float volume = 1f, float pitch = 1f)
    {
        SetupSfxAudioSource(volume, pitch);
        _sfxSource.PlayOneShot(audioClip);
    }

    private void SetupSfxAudioSource(float volume, float pitch)
    {
        _sfxSource.loop = false;
        _sfxSource.volume = volume;
        _sfxSource.pitch = pitch;
    }

    public void PlayMusic(string soundName)
    {
        Sound sound = GetMusic(soundName);

        if (sound != null)
        {
            AudioSource currentMusicSource = GetCurretAudioSource();
            SetupCurrentMusicSource(currentMusicSource, sound);
            currentMusicSource.Play();
        }
    }

    public void PlayMusicCheckSame(string soundName)
    {
        Sound sound = GetMusic(soundName);

        if (sound != null)
        {
            if (GetCurretAudioSource().clip != sound.clip)
            {
                AudioSource currentMusicSource = GetCurretAudioSource();
                SetupCurrentMusicSource(currentMusicSource, sound);
                currentMusicSource.Play();
            }
        }
    }
    public void StopMusic()
    {
        if (_musicSource1IsPlaying)
        {
            _musicSource1.Stop();
            _musicSource1IsPlaying = false;
        }
        else
        {
            _musicSource2.Stop();
            _musicSource1IsPlaying = true;
        }
    }

    public void FadeInMusic(string soundName, float targetVolume = 1f, float duration = 0.45f)
    {
        Sound sound = GetMusic(soundName);
        if (sound != null)
        {
            AudioSource currentMusicSource = GetCurretAudioSource();

            SetupCurrentMusicSource(currentMusicSource, sound);

            StartCoroutine(FadeIn(currentMusicSource,
                targetVolume,
                sound.volume,
                duration));
        }
    }
    public void FadeOutMusic(string soundName, float targetVolume = 0f, float duration = 0.45f, bool stopTheEnd = true)
    {
        Sound sound = GetMusic(soundName);
        if (sound != null)
        {
            AudioSource currentMusicSource = _musicSource1IsPlaying ? _musicSource1 : _musicSource2;

            StartCoroutine(FadeOut(currentMusicSource, targetVolume, sound.volume, duration, stopTheEnd));
        }
    }
    public void PlayCrossFade(string soundName, float targetVol = 1, float duration = 0.5f)
    {
        Sound sound = GetMusic(soundName);

        if (sound != null)
        {
            if (GetCurretAudioSource().clip != sound.clip)
            {
                AudioSource currentMusicSource;
                AudioSource newSource;

                if (_musicSource1IsPlaying)
                {
                    currentMusicSource = _musicSource1;
                    newSource = _musicSource2;
                    _musicSource1IsPlaying = false;

                }
                else
                {
                    currentMusicSource = _musicSource2;
                    newSource = _musicSource1;
                    _musicSource1IsPlaying = true;
                }

                SetupCurrentMusicSource(newSource, sound);

                StartCoroutine(FadeOut(currentMusicSource, 0f, currentMusicSource.volume, duration));
                StartCoroutine(FadeIn(newSource, targetVol, sound.volume, duration));
            }
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float targetVolume, float maxVolume, float duration = 0.45f)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0f;
            audioSource.Play();
        }

        float timer = 0;
        float currentVol = audioSource.volume;
        float targetVol = Math.Clamp(targetVolume, 0f, maxVolume);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            var volume = Mathf.Lerp(currentVol, targetVol, timer / duration);
            audioSource.volume = volume;
            yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float targetVolume, float maxVolume, float duration = 0.45f, bool stopTheEnd = true)
    {
        float timer = 0;
        float currentVol = audioSource.volume;
        float targetVol = Math.Clamp(targetVolume, 0f, maxVolume);

        while (audioSource.volume > targetVol)
        {
            timer += Time.deltaTime;
            var volume = Mathf.Lerp(currentVol, targetVol, timer / duration);
            audioSource.volume = volume;
            yield return null;
        }

        if (stopTheEnd)
        {
            audioSource.Stop();
        }
    }

    private void SetupCurrentMusicSource(AudioSource currentMusicSource, Sound sound)
    {
        currentMusicSource.clip = sound.clip;
        currentMusicSource.volume = sound.volume;
        currentMusicSource.loop = sound.loop;
        currentMusicSource.pitch = sound.pitch;
    }

    private AudioSource ChangeCurretAudioSource()
    {
        AudioSource currentMusicSource;

        if (_musicSource1IsPlaying)
        {
            _musicSource1.Stop();
            currentMusicSource = _musicSource2;
            _musicSource1IsPlaying = false;
        }
        else
        {
            _musicSource2.Stop();
            currentMusicSource = _musicSource1;
            _musicSource1IsPlaying = true;
        }

        return currentMusicSource;
    }

    private AudioSource GetCurretAudioSource()
    {
        AudioSource currentMusicSource;

        if (_musicSource1IsPlaying)
        {
            currentMusicSource = _musicSource1;
        }
        else
        {
            currentMusicSource = _musicSource2;
        }

        return currentMusicSource;
    }
    private Sound GetMusic(string soundName)
    {
        Sound sound = musics.Find(music => music.soundName == soundName);

        if (sound == null)
        {
            AudioDebug($"Music name <color=red> {soundName} </color> not found");
            return null;
        }

        return sound;
    }
    private Sound GetSFX(string soundName)
    {
        Sound sound = sfx.Find(music => music.soundName == soundName);

        if (sound == null)
        {
            AudioDebug($"SFX name <color=red> {soundName} </color> not found");
            return null;
        }

        return sound;
    }
    private void AudioDebug(string message)
    {
        Debug.Log($"[Audio Manager]: {message}");
    }

    public void ChangeStateMixerSFX(bool value)
    {
        if (value)
        {
            EnabledSFX = 1;
            masterMixer.SetFloat(keyMixerEffects, 0f);
        }
        else
        {
            EnabledSFX = 0;
            masterMixer.SetFloat(keyMixerEffects, -80f);
        }

        PlayerPrefs.SetInt(keyMixerEffects, EnabledSFX);
        PlayerPrefs.Save();
    }

    public void ChangeStateMixerMusic(bool value)
    {
        if (value)
        {
            EnabledMusicLevel = 1;
            masterMixer.SetFloat(keyMixerMusic, -0f);
        }
        else
        {
            EnabledMusicLevel = 0;
            masterMixer.SetFloat(keyMixerMusic, -80f);
        }

        PlayerPrefs.SetInt(keyMixerMusic, EnabledMusicLevel);
        PlayerPrefs.Save();
    }

}