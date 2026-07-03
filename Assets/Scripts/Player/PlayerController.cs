using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] GameObject mask;
    [SerializeField] CUIColorPicker colorPicker;
    private float moveSpeed;

    private Vector2 moveInput = Vector2.zero;
    private Rigidbody2D rb;
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
        if (colorPicker == null)
        {
            colorPicker = FindObjectOfType<CUIColorPicker>();
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

    public CUIColorPicker.MaskMode GetMaskMode()
    {
        return colorPicker != null ? colorPicker.CurrentMode : CUIColorPicker.MaskMode.Normal;
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