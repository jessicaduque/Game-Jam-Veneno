using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    [Header("Movement")]
    [SerializeField] private float movementTime;
    [SerializeField] private Transform[] movementPoints;

}
