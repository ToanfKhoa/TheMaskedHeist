using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput = Vector2.zero;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // -------------------------
    // INPUT SYSTEM (Sent by PlayerInput component)
    // -------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        // Reads value from WASD or Arrow Keys or Gamepad Stick
        moveInput = context.ReadValue<Vector2>();
    }

    // -------------------------
    // LOGIC UPDATE (Visuals)
    // -------------------------
    private void Update()
    {
        HandleAnimations();
        HandleSpriteFlip();
    }

    // -------------------------
    // PHYSICS UPDATE (Movement)
    // -------------------------
    private void FixedUpdate()
    {
        // Apply movement using Rigidbody position (prevents jitter/collision issues)
        // Normalized ensures diagonal movement isn't faster
        Vector2 currentMove = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + currentMove);
    }

    // -------------------------
    // VISUAL HELPERS
    // -------------------------
    private void HandleAnimations()
    {
        if (animator != null)
        {
            // Set running boolean if input magnitude is > 0
            animator.SetBool("isRunning", moveInput.sqrMagnitude > 0);
        }
    }

    private void HandleSpriteFlip()
    {
        // Flip sprite based on X direction
        if (moveInput.x > 0)
            transform.localScale = new Vector3(-1, 1, 1); // Face Right
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(1, 1, 1);  // Face Left
    }
}