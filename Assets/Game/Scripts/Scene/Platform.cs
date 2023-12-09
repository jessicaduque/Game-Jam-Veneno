using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Floor
{
    private PlayerController _playerController => PlayerController.I;
    //[SerializeField] private BoxCollider2D thisCollider2D;

    private void OnValidate()
    {
        //thisCollider2D = GetComponent<BoxCollider2D>(); 
    }

}
