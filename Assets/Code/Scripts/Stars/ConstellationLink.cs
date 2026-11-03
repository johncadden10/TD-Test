using UnityEngine;

public class ConstellationLink : MonoBehaviour, IDamageModifier
{
    private Health ahead;
    private LineRenderer line;

    public int Order => -10;
    public bool IsBlocked => ahead != null; // destroyed stars read as null

    public void Setup(Health aheadStar, ModifierVisuals visuals)
    {
        ahead = aheadStar;

        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.numCapVertices = 4;

            Material mat = (visuals != null && visuals.lineMaterial != null)
                ? visuals.lineMaterial
                : new Material(Shader.Find("Sprites/Default"));
            line.material = mat;

            Color col = visuals != null ? visuals.lineColor : Color.cyan;
            float width = visuals != null ? visuals.lineWidth : 0.06f;
            line.startColor = col;
            line.endColor = col;
            line.startWidth = width;
            line.endWidth = width;

            SpriteRenderer main = GetComponentInChildren<SpriteRenderer>();
            if (main != null)
            {
                line.sortingLayerID = main.sortingLayerID;
                line.sortingOrder = main.sortingOrder - 1;
            }
        }
    }

    public void Modify(ref DamageInfo info)
    {
        if (IsBlocked) info.amount = 0f;
    }

    private void LateUpdate()
    {
        if (line == null) return;

        line.enabled = IsBlocked;
        if (IsBlocked)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, ahead.transform.position);
        }
    }
}