using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private int keyID;
    [SerializeField] private SpriteRenderer keySpriteRenderer;
    private void Awake()
    {
        keySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(int key, Sprite sprite)
    {
        keyID = key;
        keySpriteRenderer.sprite = sprite;
    }

    public int GetKeyID()
    {
        return keyID;
    }
}
