using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guard : MonoBehaviour
{
    [Header("INICIAL STATS")]
    [SerializeField] private float hpMax = 6;
    private float hp;

    [Header("COMPONENTS")]
    [SerializeField] private BoxCollider2D thisCollider;
    private GameObject HealthBar;
    private Image HealthBarFill;

    [Header("KEY")]
    [SerializeField] private bool hasKey;
    [SerializeField] private KeyDetails keyDetails;

    [Header("PATROLLING")]
    [SerializeField] private Transform[] PatrollingPoints;
    private PatrollingPoint[] PatrollingPointsScripts;
    private float[] patrollingTimes = {0.5f, 0.6f, 0.7f};
    private float[] waitingTimes = {1, 2, 3};
    private Transform nextPatrollingPoint;
    private int indiceNextPatrollingPoint;
    private Transform lastPatrollingPoint;
    private bool _facingRight;
    private Sequence movementSequence;
    private bool changeSide;
    private float newPosX = 0;

    [Header("SPRITE")]
    [SerializeField] private SpriteRenderer thisSpriteRenderer;
    private bool isVisible;
    private bool resetPos;
    private float topoSprite;

    private GameObject Player;
    private bool playerIsClose;
    private PlayerController _playerController;
    private GameController _gameController => GameController.I;
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
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        hp = hpMax;
        topoSprite = thisSpriteRenderer.bounds.size.y / 2;
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
        if (!isVisible && thisSpriteRenderer.isVisible)
        {
            HealthBar = Instantiate(_gameController.HealthBarPrefab, _gameController.HealthBarCanvas.transform, worldPositionStays: false);
            HealthBar.GetComponent<HealthBar>().SetEnemy(gameObject, topoSprite);
            HealthBarFill = HealthBar.GetComponentsInChildren<Image>()[0];
            isVisible = true;
        }
        else if (isVisible && !thisSpriteRenderer.isVisible)
        {
            Destroy(HealthBar);
            isVisible = false;
        }
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
    public void TakeDamage(float dano)
    {
        if (thisSpriteRenderer.isVisible)
        {
            hp -= dano;
            HealthBarFill.fillAmount -= dano / hpMax;

            if (hp <= 0)
            {
                DropKey();
                thisCollider.enabled = false;
                //Trigger animação morte
            }
        }
    }

    public void Death()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Ataque

    private void CheckPlayerClose()
    {
        Vector2 playerPos = Player.transform.position;
        float distanceX = Vector2.Distance(transform.position, playerPos);
        if (distanceX < 2f && !changeSide)
        {
            StartCoroutine(Attack());
        }
        else if (distanceX < 8f)
        {
            StopCoroutine(Attack());
            playerIsClose = true;
            if (indiceNextPatrollingPoint != -1)
            {
                movementSequence.Kill();
                PatrollingPointsScripts[indiceNextPatrollingPoint].SetGuard(null);
                indiceNextPatrollingPoint = -1;
            }

            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerPos.x + newPosX, transform.position.y), 3.8f * Time.deltaTime);
            if (changeSide && transform.position.x == playerPos.x + newPosX)
            {
                changeSide = false;
            }
            resetPos = false;
        }
        else
        {
            if (playerIsClose)
            {
                DecideNextPatrollingPoint();
                MoveNextPatrollingPoint();
                playerIsClose = false;
            }

            if (distanceX >= 50f && !resetPos)
            {
                movementSequence.Kill();
                transform.position = new Vector2(PatrollingPoints[0].position.x, PatrollingPoints[0].position.y + 0.2f);
                MovementDone();
                resetPos = true;
            }

        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.6f);
        
    }

    #endregion

    #region Key

    private void DropKey()
    {
        if(keyDetails != null)
        {
            if (!_playerController.HasKey(keyDetails.keyID))
            {
                float angularChangeInDegrees;
                if(Player.transform.position.x > transform.position.x)
                {
                    angularChangeInDegrees = -45;
                }
                else
                {
                    angularChangeInDegrees = 45;
                }
                GameObject key = Instantiate(_gameController.KeyPrefab, transform.position, Quaternion.identity);
                var body = key.GetComponent<Rigidbody2D>();
                var impulse = (angularChangeInDegrees * Mathf.Deg2Rad) * body.inertia;
                body.AddTorque(impulse, ForceMode2D.Impulse);
            }
        }
    }

    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            float thisX = transform.position.x;
            if (collision.gameObject.transform.position.x > thisX && Player.transform.position.x > thisX)
            {
                changeSide = true;
                newPosX = 2;
            }
            else if (collision.gameObject.transform.position.x < thisX && Player.transform.position.x < thisX)
            {
                changeSide = true;
                newPosX = -2f;
            }
            else
            {
                changeSide = false;
            }
        }
    }

    #endregion 

}
