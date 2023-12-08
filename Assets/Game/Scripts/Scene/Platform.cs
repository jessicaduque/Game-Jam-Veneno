using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private PlayerController _playerController => PlayerController.I;
    //[SerializeField] private BoxCollider2D thisCollider2D;

    private void OnValidate()
    {
        //thisCollider2D = GetComponent<BoxCollider2D>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerController.SetJump();
        }
    }

}
