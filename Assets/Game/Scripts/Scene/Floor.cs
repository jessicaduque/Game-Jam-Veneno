using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    protected bool activateStayCheck;
    private float posY;
    private PlayerController _playerController => PlayerController.I;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            posY = _playerController.gameObject.transform.position.y;
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
            if (Mathf.Abs(posY - _playerController.gameObject.transform.position.y) < 0.001f)
            {
                _playerController.SetJump();
                activateStayCheck = false;
            }
            else
            {
                posY = _playerController.gameObject.transform.position.y;
            }
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerController.CheckCanJump();
        }
    }
}
