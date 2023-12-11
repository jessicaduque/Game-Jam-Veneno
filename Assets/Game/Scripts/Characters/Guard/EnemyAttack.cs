using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float dano;
    private PlayerController _playerController => PlayerController.I;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _playerController.TakeDamage(dano);
    }
}
