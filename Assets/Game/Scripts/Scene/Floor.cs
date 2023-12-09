using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private bool activateStayCheck;

    private PlayerController _playerController => PlayerController.I;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_playerController.thisRb.velocity.y == 0)
            {
                _playerController.SetJump();
            }
            else
            {
                activateStayCheck = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (activateStayCheck)
        {
            if (_playerController.thisRb.velocity.y == 0)
            {
                _playerController.SetJump();
                activateStayCheck = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerController.CheckCanJump();
        }
    }
}
