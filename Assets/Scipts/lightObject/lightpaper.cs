using UnityEngine;

public class lightpaper : MonoBehaviour
{
    [SerializeField] private Color glowColor = new Color(1f, 1f, 0.5f, 0.3f);
    [SerializeField] private float glowScale = 1.5f;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer glowRenderer;
    private SpriteRenderer glowRenderer2;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        GameObject glowObj = new GameObject("Glow");
        glowObj.transform.SetParent(transform);
        glowObj.transform.localPosition = Vector3.zero;
        glowObj.transform.localScale = Vector3.one * glowScale;

        glowRenderer = glowObj.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = spriteRenderer.sprite;
        glowRenderer.color = glowColor;
        glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        glowRenderer.material = spriteRenderer.material;

        GameObject glowObj2 = new GameObject("GlowSoft");
        glowObj2.transform.SetParent(transform);
        glowObj2.transform.localPosition = Vector3.zero;
        glowObj2.transform.localScale = Vector3.one * (glowScale * 1.2f);

        glowRenderer2 = glowObj2.AddComponent<SpriteRenderer>();
        glowRenderer2.sprite = spriteRenderer.sprite;
        glowRenderer2.color = new Color(glowColor.r, glowColor.g, glowColor.b, glowColor.a * 0.5f);
        glowRenderer2.sortingOrder = spriteRenderer.sortingOrder - 2;
        glowRenderer2.material = spriteRenderer.material;
    }

    void Update()
    {
        if (glowRenderer != null)
        {
            glowRenderer.sprite = spriteRenderer.sprite;
            glowRenderer.material = spriteRenderer.material;
        }
        if (glowRenderer2 != null)
        {
            glowRenderer2.sprite = spriteRenderer.sprite;
            glowRenderer2.material = spriteRenderer.material;
        }
    }
}
