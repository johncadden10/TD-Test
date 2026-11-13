using UnityEngine;

public static class StarModifiers
{
    public static void Apply(GameObject star, EnemyGroup group, Health ahead, ModifierVisuals visuals)
    {
        if (group.belted && star.GetComponent<BeltedModifier>() == null)
        {
            star.AddComponent<BeltedModifier>();
            AddOverlay(star, "BeltedOverlay",
                visuals != null ? visuals.beltedOverlay : null,
                visuals != null ? visuals.beltedColor : Color.gray,
                visuals != null ? visuals.overlayScale : 1.3f);
        }

        if (group.fused && star.GetComponent<FusionModifier>() == null)
        {
            star.AddComponent<FusionModifier>();
            AddOverlay(star, "FusedOverlay",
                visuals != null ? visuals.fusedOverlay : null,
                visuals != null ? visuals.fusedColor : new Color(1f, 0.6f, 0.1f),
                (visuals != null ? visuals.overlayScale : 1.3f) * 1.15f); // slightly larger so both rings show
        }

        if (group.constellation)
        {
            ConstellationLink link = star.GetComponent<ConstellationLink>();
            if (link == null) link = star.AddComponent<ConstellationLink>();
            link.Setup(ahead, visuals); // ahead is null for the head
        }

        star.GetComponent<Health>().RefreshModifiers();
        star.GetComponent<HiddenUnlessNearGhost>()?.RefreshRenderers();
    }

    private static void AddOverlay(GameObject star, string name, Sprite sprite, Color color, float scale)
    {
        SpriteRenderer main = star.GetComponentInChildren<SpriteRenderer>();

        GameObject go = new GameObject(name);
        go.transform.SetParent(star.transform, false);
        go.transform.localScale = Vector3.one * scale;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite != null ? sprite : PlaceholderRing();
        sr.color = color;

        if (main != null)
        {
            sr.sortingLayerID = main.sortingLayerID;
            sr.sortingOrder = main.sortingOrder + 1;
        }
    }

    private static Sprite ring;

    private static Sprite PlaceholderRing()
    {
        if (ring != null) return ring;

        const int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float c = (size - 1) / 2f;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c)) / c;
                tex.SetPixel(x, y, (d <= 1f && d >= 0.8f) ? Color.white : Color.clear);
            }

        tex.Apply();
        ring = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return ring;
    }
}