using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

public class GameController : Singleton<GameController>
{
    [Header("HEALTH")]
    [SerializeField] public GameObject HealthBarPrefab;
    [SerializeField] public Canvas HealthBarCanvas;

    [Header("KEYS")]
    [SerializeField] public GameObject KeyPrefab;

    private new void Awake()
    {
        
    }
}
