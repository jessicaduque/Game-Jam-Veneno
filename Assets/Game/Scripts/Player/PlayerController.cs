using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Singleton;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private float hp;
    [SerializeField] private Vector2 inicialPos;

    private CustomInputs input = null;

    [Header("MOVEMENT")]
    private Vector2 moveVector = Vector2.zero;
    [SerializeField] private float moveSpeed = 10f;


    private Rigidbody2D rb = null;

    private void Awake()
    {
        SetPosition(inicialPos);
        rb = GetComponent<Rigidbody2D>();
        input = new CustomInputs();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.canceled -= OnMovementCancelled;
    }

    private void FixedUpdate()
    {
        rb.velocity = moveVector * moveSpeed;
    }

    #region Movement
    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }

    #endregion

    #region SET

    private void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    #endregion
}
