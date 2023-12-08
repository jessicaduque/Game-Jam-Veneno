using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private BoxCollider2D thisCollider;

    [Header("Patrolling")]
    [SerializeField] private Transform[] PatrollingPoints;
    private PatrollingPoint[] PatrollingPointsScripts;
    [SerializeField] private float[] patrollingTimes;
    private Transform nextPatrollingPoint;
    private int indiceNextPatrollingPoint;
    private Transform lastPatrollingPoint;
    

    private Sequence movementSequence;

    private void OnValidate()
    {
        thisCollider = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        indiceNextPatrollingPoint = Random.Range(0, PatrollingPoints.Length);
        nextPatrollingPoint = PatrollingPoints[indiceNextPatrollingPoint];
        lastPatrollingPoint = nextPatrollingPoint;
        PatrollingPointsScripts = new PatrollingPoint[PatrollingPoints.Length];
        for (int i = 0; i < PatrollingPoints.Length; i++)
        {
            PatrollingPointsScripts[i] = PatrollingPoints[i].GetComponent<PatrollingPoint>();
        }
    }

    #region Movement
    private void DecideNextPatrollingPoint()
    {
        while(nextPatrollingPoint == lastPatrollingPoint)
        {
            indiceNextPatrollingPoint = Random.Range(0, PatrollingPoints.Length);
            nextPatrollingPoint = PatrollingPoints[indiceNextPatrollingPoint];
        }
    }

    private void MoveNextPatrollingPoint()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.1f);
        float patrollingTime = patrollingTimes[Random.Range(0, patrollingTimes.Length)];
        movementSequence = DOTween.Sequence();
        movementSequence.Append(transform.DOMoveX(nextPatrollingPoint.position.x, patrollingTime).OnComplete(MovementDone));
        movementSequence.Join(transform.DOMoveZ(0, patrollingTime - 1).OnComplete(CheckPointHasGuard));
    }


    private void MovementDone()
    {
        
    }

    private void CheckPointHasGuard()
    {
        if (PatrollingPoints[indiceNextPatrollingPoint].GetComponent<PatrollingPoint>().hasGuard)
        {

        }
    }
    #endregion

    #region Dano
    public void LevarDano(float dano)
    {
        hp -= dano;

        if(hp <= 0)
        {
            thisCollider.enabled = false;
            //Trigger animação morte
        }
    }

    public void Morrer()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
