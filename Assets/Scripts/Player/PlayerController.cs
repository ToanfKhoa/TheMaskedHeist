using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] GameObject mask;
    private float moveSpeed;

    private Vector2 moveInput = Vector2.zero;
    private Rigidbody2D rb;
    private bool movementLocked = false;
    [SerializeField] Animator animator;

    [SerializeField] private Transform holdPivot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = walkSpeed;
        //animator = GetComponent<Animator>();
    }

    // -------------------------
    // INPUT
    // -------------------------

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDiamondStolen.AddListener(SpeedUp);
        }
    }
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
        if (movementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentMove = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        Vector2 targetPosition = rb.position + currentMove;

        rb.MovePosition(targetPosition);
    }

    // Locks/unlocks player movement (e.g. while caught in a trap net).
    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;
        if (locked)
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
                animator.SetBool("isRunning", false);
        }
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

    public Color GetMaskColor()
    {
        if (mask != null)
        {
            SpriteRenderer sr = mask.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                return sr.color;
            }
        }
        return Color.white;
    }

    public void HoldDiamond(GameObject diamond)
    {
        if (holdPivot != null && diamond != null)
        {
            diamond.transform.SetParent(holdPivot);
            diamond.transform.localPosition = Vector3.zero;
        }
    }

    private void SpeedUp()
    {
        moveSpeed = runSpeed;
    }
}