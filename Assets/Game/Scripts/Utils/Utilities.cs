using UnityEngine;
using DG.Tweening;

public static class Utilities
{
    public const float tempoPanelFade = 0.4f;
    public const float tempoPretoFade = 0.4f;
    public static Camera cam => Camera.main;

    public static void FadeInPanel(GameObject thisObject, CanvasGroup cg_object, float time = tempoPanelFade)
    {
        thisObject.SetActive(true);
        cg_object.DOFade(1, time).SetUpdate(true);
    }

    public static void FadeOutPanel(GameObject thisObject, CanvasGroup cg_object, float time = tempoPanelFade)
    {
        cg_object.DOFade(0, time).OnComplete(() => thisObject.SetActive(false)).SetUpdate(true);
    }

    public static void FadeCrossPanel(GameObject thisObject1, CanvasGroup cg_object1, GameObject thisObject2, CanvasGroup cg_object2, float time = tempoPanelFade)
    {
        cg_object1.DOFade(0, time).OnComplete(() => {
            thisObject1.SetActive(false);
            thisObject2.SetActive(true);
            cg_object2.DOFade(1, time);
        });
    }
}
