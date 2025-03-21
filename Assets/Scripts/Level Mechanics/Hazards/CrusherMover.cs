using Unity.VisualScripting;
using UnityEngine;

public class CrusherMover : MonoBehaviour
{
    [SerializeField] BoxCollider2D col, crusherCol;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] CrusherCollider crusherStatus;

    [Min(0)] public float Offset;

    private void OnValidate()
    {
        col ??= GetComponent<BoxCollider2D>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        crusherCol ??= crusherStatus.gameObject.GetComponent<BoxCollider2D>();

        RefreshBounds();
    }

    private void Update()
    {
        RefreshBounds();
    }

    public void RefreshBounds()
    {
        var width = spriteRenderer.size.x;
        var height = Offset;
        spriteRenderer.size = new(width, height);
        col.offset = new(0, -height/2);
        col.size = new(width, height);
        crusherCol.transform.localPosition = new(0, -height);
    }
}