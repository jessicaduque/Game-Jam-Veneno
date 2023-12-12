using Utils.Singleton;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
        cg_TelaPreta = TelaPretaPanel.GetComponent<CanvasGroup>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeInSceneStart();
    }

    public void FadeInSceneStart()
    {
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(1f);
        Utilities.FadeOutPanel(TelaPretaPanel, cg_TelaPreta, tempoFadePreto);
    }

    public void FadeOutScene(string nomeScene)
    {
        TelaPretaPanel.SetActive(true);
        cg_TelaPreta.DOFade(1, tempoFadePreto).OnComplete(() => SceneManager.LoadScene(nomeScene)).SetUpdate(true);
    }

    public void FadeInOutPanel(GameObject Panel, bool state)
    {
        TelaPretaPanel.SetActive(true);
        cg_TelaPreta.DOFade(1, tempoFadePreto).OnComplete(() =>
        {
            Panel.SetActive(state);
            cg_TelaPreta.DOFade(0, tempoFadePreto).OnComplete(() => TelaPretaPanel.SetActive(false));
        });
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}