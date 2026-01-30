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
    // INPUT
    // -------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // -------------------------
    // VISUALS
    // -------------------------
    private void Update()
    {
        HandleAnimations();
        HandleSpriteFlip();
    }

    // -------------------------
    // PHYSICS
    // -------------------------
    private void FixedUpdate()
    {
        Vector2 currentMove = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        Vector2 targetPosition = rb.position + currentMove;

        rb.MovePosition(targetPosition);
    }

    // -------------------------
    // HELPER METHODS
    // -------------------------
    private void HandleAnimations()
    {
        if (animator != null)
        {
            animator.SetBool("isRunning", moveInput.sqrMagnitude > 0);
        }
    }

    private void HandleSpriteFlip()
    {
        if (moveInput.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
}