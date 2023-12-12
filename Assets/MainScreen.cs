using System.Collections;
using System.Collections.Generic;
using Utils.Singleton;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MainScreen : Singleton<MainScreen>
{
    [Header("MAIN")]
    [SerializeField] private Button b_start, b_credits, b_settings;

    [Header("CONFIGURAÇÕES")]
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private Button b_set_return, b_musica, b_som;
    [SerializeField] private TextMeshProUGUI t_musica, t_som;
    int musicOn, sfxOn;

    [Header("CRÉDITOS")]
    [SerializeField] private GameObject CreditsPanel;
    [SerializeField] private Button b_cred_return;

    private ControleFadePreto _controleFadePreto => ControleFadePreto.I;
    private AudioManager _audioManager => AudioManager.I;

    private new void Awake()
    {
        b_start.onClick.AddListener(StartGame);

        // Settings
        b_settings.onClick.AddListener(() => ControlSettings(true));
        b_set_return.onClick.AddListener(() => ControlSettings(false));
        b_musica.onClick.AddListener(MusicChange);
        b_som.onClick.AddListener(SfxChange);

        // Credits
        b_credits.onClick.AddListener(() => ControlCredits(true));
        b_cred_return.onClick.AddListener(() => ControlCredits(false));

        PlayerPrefsSetup();

        _audioManager.PlayMusic("Menu");
    }
    private void PlayerPrefsSetup()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            musicOn = PlayerPrefs.GetInt("Music");
        }
        else
        {
            musicOn = 1;
            PlayerPrefs.SetInt("Music", 1);
        }

        if (PlayerPrefs.HasKey("Sfx"))
        {
            sfxOn = PlayerPrefs.GetInt("Sfx");
        }
        else
        {
            sfxOn = 1;
            PlayerPrefs.SetInt("Sfx", 1);
        }

        t_musica.text = (musicOn == 1 ? "Ligado" : "Desligado");
        t_som.text = (sfxOn == 1 ? "Ligado" : "Desligado");
    }

    private void MusicChange()
    {
        if (musicOn == 1)
        {
            _audioManager.ChangeStateMixerMusic(false);
            musicOn = 0;
        }
        else
        {
            _audioManager.ChangeStateMixerMusic(true);
            musicOn = 1;
        }

        t_musica.text = (musicOn == 1 ? "Ligado" : "Desligado");
    }

    private void SfxChange()
    {
        if (sfxOn == 1)
        {
            _audioManager.ChangeStateMixerSFX(false);
            sfxOn = 0;
        }
        else
        {
            _audioManager.ChangeStateMixerSFX(true);
            sfxOn = 1;
        }

        t_som.text = (sfxOn == 1 ? "Ligado" : "Desligado");
    }


    private void ControlSettings(bool state)
    {
        _controleFadePreto.FadeInOutPanel(SettingsPanel, state);
    }
    private void ControlCredits(bool state)
    {
        _controleFadePreto.FadeInOutPanel(CreditsPanel, state);
    }

    private void StartGame()
    {
        _controleFadePreto.FadeOutScene("DialogoInicial");
        _audioManager.PlayMusic("PoisonWhite");
    }
}
