using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [SerializeField] private int doorID;
    [SerializeField] private string sceneName;
    [SerializeField] private bool isLocked;
    [SerializeField] private GameObject PressE;

    private ControleFadePreto _controleFadePreto => ControleFadePreto.I;

    private void OnValidate()
    {
        if(PressE == null)
        {
            PressE = transform.GetChild(0).gameObject;
        }
    }

    private void ActivateVisual(bool state)
    {
        PressE.SetActive(state);
    }

    public void GoScene()
    {
        _controleFadePreto.FadeOutScene(sceneName);
    }

    #region Trigger

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActivateVisual(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActivateVisual(false);
        }
    }

    #endregion

    #region GET

    public bool GetIsLocked()
    {
        return isLocked;
    }

    public int GetDoorID()
    {
        return doorID;
    }


    #endregion
}
