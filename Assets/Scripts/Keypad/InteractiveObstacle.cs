using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class InteractiveObstacle : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Sprite openSprite; 

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private bool _isOpen = false; 

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    public void Open()
    {
        if (_isOpen) return;

        _isOpen = true;
        _spriteRenderer.sprite = openSprite;
        _collider.enabled = false;        
    }
}