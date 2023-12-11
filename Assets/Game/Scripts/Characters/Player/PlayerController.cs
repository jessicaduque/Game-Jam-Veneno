using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{
    [Header("INICIAL STATS")]
    [SerializeField] private float hp;
    [SerializeField] private Vector2 inicialPos;

    [Header("COMPONENTS")]
    [SerializeField] SpriteRenderer thisSpriteRenderer;
    [SerializeField] private BoxCollider2D thisCollider;
    [SerializeField] private BoxCollider2D feetCollider;
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

    [Header("ATTACK")]
    private bool isAttacking;
    private bool isAiming;

    [Header("DOORS")]
    private List<int> keyIDs = new List<int>();
    private Door thisDoor;
    private int lastDoorID;

    protected override void Awake()
    {
        base.Awake();

        transform.position = inicialPos;
        thisRb = GetComponent<Rigidbody2D>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        input = new CustomInputs();

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
        thisRb.velocity = new Vector2(moveX * moveXDash * moveSpeed, thisRb.velocity.y);
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
        }
        else
        {
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCancelled;
            input.Player.Down.performed -= OnDownPerformed;
            input.Player.Jump.performed -= JumpPerformed;
            input.Player.Jump.canceled -= JumpCancelled;
            input.Player.Dash.performed -= OnDashPerformed;
        }
        
    }

    #region Lateral Movement

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveX = value.ReadValue<Vector2>().x;

        if(moveX > 0 && !_facingRight)
        {
            thisSpriteRenderer.flipX = false;
            _facingRight = true;
        }
        else if(moveX < 0 && _facingRight)
        {
            thisSpriteRenderer.flipX = true;
            _facingRight = false;
        }
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveX = 0;
        moveXDash = 1;
    }


    #endregion


    #region Jump

    public void CheckCanJump()
    {
        if(amountJumps == 2)
        {
            canJump = false;
            canCheckCancelled = false;
        }
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        if (amountJumps > 0 && !jumped && canJump)
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
        if (isOnPlatform)
        {
            feetCollider.enabled = false;
            StartCoroutine(EnableFeet());
        }
    }

    private IEnumerator EnableFeet()
    {
        while (isOnPlatform)
        {
            yield return null;
        }
        feetCollider.enabled = true;
    }

    #endregion

    #region Dash

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashPress++;
        if(dashPress == 1)
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
                moveXDash = 1.6f;
                time = 1;
            }
            time += Time.deltaTime;
            yield return null;
        }
        dashPress = 0;
    }
    #endregion

    #endregion

    #region Damage

    public void TakeDamage(float dano)
    {
        hp -= dano;

        if(hp <= 0)
        {
            thisCollider.enabled = false;
            // Trigger morte
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
                    if(thisDoor.GetDoorSceneName() == "DialogoFinal")
                    {
                        SceneManager.sceneLoaded -= SetPosition;
                    }
                    else
                    {
                        lastDoorID = thisDoor.GetDoorID();
                    }
                    thisDoor.GoScene();
                }
            }
            // Tocar som de porta trancada
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
            keyIDs.Add(collision.GetComponent<Key>().GetKeyID());
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
    }


    #endregion
}
