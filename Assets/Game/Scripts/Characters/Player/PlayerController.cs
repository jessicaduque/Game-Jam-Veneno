using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;
using System.Collections;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private float hp;
    [SerializeField] private Vector2 inicialPos;

    private CustomInputs input = null;

    [Header("MOVEMENT")]
    private float moveX = 0;
    private float moveXDash = 1;
    [SerializeField] private float moveSpeed = 8f;
    private bool _facingRight = true;
    private int amountJumps = 2;
    private bool jumped = false;
    private bool canJump = true;
    private bool canCheckCancelled = true;
    [SerializeField] private float jumpingPower = 16f;
    private int dashPress = 0;

    [SerializeField] SpriteRenderer thisSpriteRenderer;
    [SerializeField] private BoxCollider2D thisCollider;
    [SerializeField] private BoxCollider2D feetCollider;
    public Rigidbody2D thisRb = null;

    private new void Awake()
    {
        SetPosition(inicialPos);
        thisRb = GetComponent<Rigidbody2D>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        input = new CustomInputs();
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
        thisRb.velocity = new Vector2(moveX * moveSpeed * moveXDash, thisRb.velocity.y);
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
            thisRb.velocity = new Vector2(thisRb.velocity.x, 0);
            thisRb.AddForce(Vector2.up * jumpingPower * (amountJumps == 1 ? 1.2f : 1), ForceMode2D.Impulse);
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
        feetCollider.enabled = false;
        StartCoroutine(EnableFeet());
    }

    private IEnumerator EnableFeet()
    {
        yield return new WaitForSeconds(0.2f);
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

    #region Dano

    public void LevarDano(float dano)
    {
        hp -= dano;

        if(hp <= 0)
        {
            thisCollider.enabled = false;
            // Trigger morte
        }
    }

    #endregion



    #region SET

    private void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetJump()
    {
        canCheckCancelled = true;
        canJump = true;
        amountJumps = 2;
    }

    #endregion
}
