using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Modifier Visuals")]
public class ModifierVisuals : ScriptableObject
{
    [Header("Belted")]
    public Sprite beltedOverlay;
    public Color beltedColor = new Color(0.4f, 0.3f, 0.1f, 1f);

    [Header("Fused")]
    public Sprite fusedOverlay;
    public Color fusedColor = new Color(0f, 0.3f, 1f, 1f);

    public float overlayScale = 1.3f;

    [Header("Constellation Line")]
    public Material lineMaterial; // empty = default sprite material
    public Color lineColor = new Color(1f, 1f, 1f, 0.7f);
    public float lineWidth = 0.05f;
}