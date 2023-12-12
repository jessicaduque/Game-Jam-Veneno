using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private Button thisButton;
    AudioManager _audioManager => AudioManager.I;

    private void OnValidate()
    {
        if(thisButton == null)
        {
            thisButton = GetComponent<Button>();
        }
    }

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(MakeSound);
    }

    private void OnEnable()
    {
        thisButton.enabled = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void MakeSound()
    {
        _audioManager.PlaySfx("ButtonClick"); 
        thisButton.enabled = false;
        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.1f);
        thisButton.enabled = true;
    }

}
