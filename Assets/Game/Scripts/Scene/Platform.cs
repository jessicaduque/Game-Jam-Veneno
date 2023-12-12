using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Floor
{
    [SerializeField] private GameObject Player;


    protected override void Awake()
    {
        base.Awake();

        Player = _playerController.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.IsChildOf(Player.transform))
        {
            _playerController.SetIsOnPlatform(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.transform.IsChildOf(Player.transform))
        {
            _playerController.SetIsOnPlatform(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.IsChildOf(Player.transform))
        {
            _playerController.SetIsOnPlatform(false);
        }
    }
}
