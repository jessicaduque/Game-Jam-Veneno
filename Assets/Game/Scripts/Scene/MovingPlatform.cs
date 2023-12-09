using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    [Header("Movement")]
    [SerializeField] private float movementTime;
    [SerializeField] private float waitTime = 0.5f;
    [SerializeField] private Transform[] movementPoints;

    private void Awake()
    {
        transform.position = movementPoints[1].position;
        Sequence seq = DOTween.Sequence();
        foreach (Transform t in movementPoints)
        {
            seq.Append(transform.DOMove(t.position, movementTime).SetEase(Ease.InOutQuad));
            seq.AppendInterval(waitTime);

        }
        seq.SetLoops(-1);
    }
}
