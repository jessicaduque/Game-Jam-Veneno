using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    [SerializeField] private float hp = 6;
    [SerializeField] private BoxCollider2D thisCollider;

    [Header("Patrolling")]
    [SerializeField] private Transform[] PatrollingPoints;
    private PatrollingPoint[] PatrollingPointsScripts;
    private float[] patrollingTimes = {0.5f, 0.6f, 0.7f};
    private float[] waitingTimes = {1, 2, 3};
    private Transform nextPatrollingPoint;
    private int indiceNextPatrollingPoint;
    private Transform lastPatrollingPoint;
    private bool _facingRight;
    private Sequence movementSequence;
    
    [SerializeField] private SpriteRenderer thisSpriteRenderer;
    
    private GameObject Player;
    private bool playerIsClose;
    private PlayerController _playerController => PlayerController.I;
    private void OnValidate()
    {
        if(thisCollider == null)
        {
            thisCollider = GetComponent<BoxCollider2D>();
        }
        if(thisSpriteRenderer == null)
        {
            thisSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Awake()
    {
        Player = _playerController.gameObject;
        indiceNextPatrollingPoint = Random.Range(0, PatrollingPoints.Length);
        nextPatrollingPoint = PatrollingPoints[indiceNextPatrollingPoint];
        lastPatrollingPoint = nextPatrollingPoint;
        PatrollingPointsScripts = new PatrollingPoint[PatrollingPoints.Length];
        for (int i = 0; i < PatrollingPoints.Length; i++)
        {
            PatrollingPointsScripts[i] = PatrollingPoints[i].GetComponent<PatrollingPoint>();
        }
    }

    private void Start()
    {
        DecideNextPatrollingPoint();
        MoveNextPatrollingPoint();
    }

    private void Update()
    {
        CheckPlayerClose();
    }

    #region Movement
    private void DecideNextPatrollingPoint()
    {
        lastPatrollingPoint = nextPatrollingPoint;
        while (nextPatrollingPoint == lastPatrollingPoint)
        {
            indiceNextPatrollingPoint = Random.Range(0, PatrollingPoints.Length);
            if (PatrollingPointsScripts[indiceNextPatrollingPoint].GetGuard() != null)
            {
                nextPatrollingPoint = lastPatrollingPoint;
            }
            else
            {
                nextPatrollingPoint = PatrollingPoints[indiceNextPatrollingPoint];
            }
        }
        _facingRight = nextPatrollingPoint.position.x > transform.position.x;
        thisSpriteRenderer.flipX = _facingRight;
        PatrollingPointsScripts[indiceNextPatrollingPoint].SetGuard(gameObject);
    }

    private void MoveNextPatrollingPoint()
    {
        float patrollingTime = patrollingTimes[Random.Range(0, patrollingTimes.Length)];
        float patrolTime = patrollingTime * Mathf.Abs(nextPatrollingPoint.position.x - transform.position.x);
        movementSequence = DOTween.Sequence();
        movementSequence.Append(transform.DOMoveX(nextPatrollingPoint.position.x, patrolTime));
        movementSequence.AppendInterval(waitingTimes[Random.Range(0, waitingTimes.Length)]).OnComplete(MovementDone);
    }


    private void MovementDone()
    {
        PatrollingPointsScripts[indiceNextPatrollingPoint].SetGuard(null);
        DecideNextPatrollingPoint();
        MoveNextPatrollingPoint();
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

    #region Ataque

    private void CheckPlayerClose()
    {
        float distance = Vector2.Distance(transform.position, Player.transform.position);
        if(distance < 2.1f)
        {
            Debug.Log("Atacando");
        }
        else if(distance < 10f)
        {
            playerIsClose = true;
            if (indiceNextPatrollingPoint != -1)
            {
                movementSequence.Kill();
                PatrollingPointsScripts[indiceNextPatrollingPoint].SetGuard(null);
                indiceNextPatrollingPoint = -1;
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(Player.transform.position.x, transform.position.y), 2 * Time.deltaTime);
        }
        else
        {
            if (playerIsClose)
            {
                DecideNextPatrollingPoint();
                MoveNextPatrollingPoint();
                playerIsClose = false;
            }
        }
    }

    #endregion
}
