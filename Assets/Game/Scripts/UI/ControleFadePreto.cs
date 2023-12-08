using Utils.Singleton;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleFadePreto : Singleton<ControleFadePreto>
{
    [SerializeField] private GameObject TelaPretaPanel;
    [SerializeField] private CanvasGroup cg_TelaPreta;

    private float tempoFadePreto => Utilities.tempoPretoFade;
    //private AudioManager _audioManager => AudioManager.I;

    protected override void Awake()
    {
        base.Awake();

        Time.timeScale = 1;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnValidate()
    {
        cg_TelaPreta = TelaPretaPanel.GetComponent<CanvasGroup>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeInSceneStart();
    }

    public void FadeInSceneStart()
    {
        Utilities.FadeOutPanel(TelaPretaPanel, cg_TelaPreta, tempoFadePreto);
    }

    public void FadeOutScene(string nomeScene)
    {
        TelaPretaPanel.SetActive(true);
        cg_TelaPreta.DOFade(1, tempoFadePreto).OnComplete(() => SceneManager.LoadScene(nomeScene)).SetUpdate(true);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}