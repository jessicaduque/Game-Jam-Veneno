using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("INICIAL STATS")]
    [SerializeField] private float hp;
    [SerializeField] private Vector2 inicialPos;

    [Header("COMPONENTS")]
    [SerializeField] SpriteRenderer thisSpriteRenderer;
    [SerializeField] private BoxCollider2D thisCollider;
    [SerializeField] private BoxCollider2D feetCollider;
    [SerializeField] private Animator thisAnimator;
    public Rigidbody2D thisRb = null;
    private CustomInputs input = null;

    [Header("DAMAGE")]
    [SerializeField] private float damageDistance = 10f;
    private float posYSaida, posYFinal;

    [Header("MOVEMENT")]
    private float moveX = 0;
    private float moveXDash = 1;
    [SerializeField] private float moveSpeed = 8f;
    private bool _facingRight = true;
    private int dashPress = 0;

    [Header("JUMPS")]
    private int amountJumps = 2;
    private bool jumped = false;
    private bool canJump = true;
    private bool canCheckCancelled = true;
    [SerializeField] private float jumpingPower = 16f;
    bool isOnPlatform;

    [Header("ANIMATION")]
    private bool attackHappening;
    private bool dashHappening;

    [Header("DOORS")]
    private List<int> keyIDs = new List<int>();
    private Door thisDoor;
    private int lastDoorID;

    private AudioManager _audioManager => AudioManager.I;

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody2D>();
        transform.position = inicialPos;
        thisSpriteRenderer = GetComponent<SpriteRenderer>();

        input = new CustomInputs();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SetPosition;
    }

    private void OnEnable()
    {
        input.Enable();
        SetupInput(true);
    }

    private void OnDisable()
    {
        input.Disable();
        SetupInput(false);
    }

    private void FixedUpdate()
    {
        if (!dashHappening && !attackHappening)
        {
            thisRb.velocity = new Vector2(moveX * moveXDash * moveSpeed, thisRb.velocity.y);
        }
        else if(dashHappening)
        {
            thisRb.velocity = new Vector2(moveXDash * moveSpeed, 0);
        }
        else if (attackHappening)
        {
            thisRb.velocity = new Vector2(moveXDash * moveSpeed, thisRb.velocity.y);
        }
    }


    #region Movement

    private void SetupInput(bool state)
    {
        if (state)
        {
            input.Player.Movement.performed += OnMovementPerformed;
            input.Player.Movement.canceled += OnMovementCancelled;
            input.Player.Down.performed += OnDownPerformed;
            input.Player.Jump.performed += JumpPerformed;
            input.Player.Jump.canceled += JumpCancelled;
            input.Player.Dash.performed += OnDashPerformed; // Fundamental ser adicionada após movement devido ao Dash
            //input.Player.AtaqueMorder.performed += OnAttackPerformed;
        }
        else
        {
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCancelled;
            input.Player.Down.performed -= OnDownPerformed;
            input.Player.Jump.performed -= JumpPerformed;
            input.Player.Jump.canceled -= JumpCancelled;
            input.Player.Dash.performed -= OnDashPerformed;
            //input.Player.AtaqueMorder.performed -= OnAttackPerformed;
        }
        
    }

    #region Lateral Movement

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveX = value.ReadValue<Vector2>().x;

        if (moveX > 0 && !_facingRight)
        {
            thisSpriteRenderer.flipX = false;
            _facingRight = true;
        }
        else if (moveX < 0 && _facingRight)
        {
            thisSpriteRenderer.flipX = true;
            _facingRight = false;
        }
        thisAnimator.SetBool("Running", true);
        
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        thisAnimator.SetBool("Running", false);
        moveX = 0;
        moveXDash = 1;
    }


    #endregion


    #region Jump

    public void CheckCanJump()
    {
        if(amountJumps == 2)
        {
            canJump = true;
            canCheckCancelled = false;
        }
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        if (amountJumps > 0 && !jumped && canJump && !dashHappening && !attackHappening)
        {
            posYSaida = transform.position.y;
            thisRb.velocity = new Vector2(thisRb.velocity.x, 0);
            thisRb.AddForce(Vector2.up * jumpingPower * 1, ForceMode2D.Impulse);
            amountJumps--;
            jumped = true;
            canJump = false;
        }
    }

    private void JumpCancelled(InputAction.CallbackContext context)
    {
        if (canCheckCancelled)
        {
            jumped = false;
            canJump = true;
        }
        
    }

    #endregion

    #region Down

    private void OnDownPerformed(InputAction.CallbackContext context)
    {
        if (isOnPlatform && !dashHappening && !attackHappening)
        {
            StartCoroutine(EnableFeet());
            feetCollider.enabled = false;
        }
        else
        {
            StopCoroutine(EnableFeet());
            feetCollider.enabled = true;
        }
    }

    private IEnumerator EnableFeet()
    {
        float time = 0;
        while (isOnPlatform)
        {
            time += Time.deltaTime;
            if(time > 0.09f)
            {
                isOnPlatform = false;
            }
            yield return null;
        }
        feetCollider.enabled = true;
    }

    #endregion

    #region Dash

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashPress++;
        if (dashPress > 2)
            dashPress = 0;
        if(dashPress == 1 && !attackHappening && !dashHappening)
        {
            StopCoroutine(DoDashTime());
            StartCoroutine(DoDashTime());
        }
    }

    private IEnumerator DoDashTime()
    {
        bool facingRight = _facingRight;
        float time = 0.0f;
        while(time < 0.3f)
        {
            if(_facingRight != facingRight)
            {
                time = 1;
            }
            else if(dashPress == 2)
            {
                dashHappening = true;
                thisRb.velocity = Vector2.zero;
                thisAnimator.SetTrigger("Dash");
                moveXDash = (facingRight ? 4f : -4f);
                time = 1;
            }
            time += Time.deltaTime;
            yield return null;
        }
        dashPress = 0;
    }
    #endregion

    #region Attack
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!dashHappening)
        {
            thisAnimator.SetTrigger("Bite");
            thisRb.AddForce(Vector2.up * jumpingPower / 2 * 1, ForceMode2D.Impulse);
            moveXDash = (_facingRight ? 1f : -1f);
            attackHappening = true;
        }
    }

    #endregion

    #endregion

    #region Damage

    public void TakeDamage(float dano)
    {
        if (!dashHappening && !attackHappening)
        {
            Debug.Log(dano);
            thisAnimator.SetTrigger("Hit");
            hp -= dano;

            if (hp <= 0)
            {
                thisCollider.enabled = false;
                // Trigger morte
            }
        }
    }

    public void Death()
    {
        Time.timeScale = 0;
    }

    #endregion

    #region Interaction

    private void EnterDoor(InputAction.CallbackContext context)
    {
        if (thisDoor.GetIsLocked())
        {
            foreach(int key in keyIDs)
            {
                if(key == thisDoor.GetDoorID())
                {
                    lastDoorID = thisDoor.GetDoorID();
                    thisDoor.GoScene();
                }
            }
            _audioManager.PlaySfx("DoorLocked");
            Debug.Log("Sem chave para essa porta.");
        }
        else
        {
            thisDoor.GoScene();
        }
    }

    #endregion

    #region PUBLIC INFO

    public bool HasKey(int id)
    {
        foreach(int k in keyIDs)
        {
            if(id == k)
            {
                return true;
            }
        }
        return false;
    }


    #endregion

    #region SET

    public void SetActionDone()
    {
        moveXDash = 1;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            moveX = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            moveX = 1;
        }
        if (attackHappening)
        {
            StartCoroutine(DelayAttack());
        }
       
        dashHappening = false;
        thisAnimator.ResetTrigger("Bite");
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(0.4f);

        attackHappening = false;
    }

    private void SetPosition(Scene scene, LoadSceneMode mode)
    {
        foreach(Door d in FindObjectsOfType<Door>())
        {
            if(d.GetDoorID() == lastDoorID)
            {
                transform.position = d.gameObject.transform.position;
            }
        }
    }

    public void SetInitialPosition(Vector2 pos)
    {
        inicialPos = pos;
    }

    public void SetJump()
    {
        canCheckCancelled = true;
        canJump = true;
        amountJumps = 2;
    }

    public void SetIsOnPlatform(bool state)
    {
        isOnPlatform = state;
        if (state)
            SetJump();
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            posYFinal = transform.position.y;
            if(posYSaida - posYFinal > damageDistance)
            {
                TakeDamage((int)((posYSaida - posYFinal) / 10));
            }
        }

        if (collision.CompareTag("Respawn"))
        {
            thisDoor = collision.gameObject.GetComponent<Door>();
            input.Player.Interagir.performed += EnterDoor;
        }

        if (collision.CompareTag("Key"))
        {
            Debug.Log(collision.GetComponent<Key>().GetKeyID());
            keyIDs.Add(collision.GetComponent<Key>().GetKeyID());
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor")){
            posYSaida = transform.position.y;
        }

        if (collision.CompareTag("Respawn"))
        {
            input.Player.Interagir.performed -= EnterDoor;
            thisDoor = null;
        }

        if (collision.CompareTag("Respawn"))
        {
            SceneManager.sceneLoaded -= SetPosition;
        }
    }


    #endregion
}
