using System.Collections.Generic;
using UnityEngine;

public class ConstellationChain
{
    public List<ConstellationLink> members = new List<ConstellationLink>(); // head first
}

public class ConstellationLink : MonoBehaviour, IDamageModifier
{
    private Health ahead;
    private LineRenderer line;
    private ConstellationChain chain;

    public int Order => -10;
    public bool IsBlocked => ahead != null; // destroyed stars read as null

    public void Setup(Health aheadStar, ModifierVisuals visuals)
    {
        ahead = aheadStar;

        ConstellationLink aheadLink = aheadStar != null ? aheadStar.GetComponent<ConstellationLink>() : null;
        if (aheadLink != null && aheadLink.chain != null) chain = aheadLink.chain;
        else if (chain == null) chain = new ConstellationChain();

        if (!chain.members.Contains(this)) chain.members.Add(this);

        if (aheadStar != null && line == null) CreateLine(visuals);
    }

    private void CreateLine(ModifierVisuals visuals)
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.numCapVertices = 4;

        line.material = (visuals != null && visuals.lineMaterial != null)
            ? visuals.lineMaterial
            : new Material(Shader.Find("Sprites/Default"));

        Color col = visuals != null ? visuals.lineColor : Color.cyan;
        float width = visuals != null ? visuals.lineWidth : 0.06f;
        line.startColor = col; line.endColor = col;
        line.startWidth = width; line.endWidth = width;

        SpriteRenderer main = GetComponentInChildren<SpriteRenderer>();
        if (main != null)
        {
            line.sortingLayerID = main.sortingLayerID;
            line.sortingOrder = main.sortingOrder - 1;
        }
    }

    public void Modify(ref DamageInfo info)
    {
        if (IsBlocked) info.amount = 0f;
    }

    // status controllers of every star behind this one
    public IEnumerable<StatusController> Followers()
    {
        if (chain == null) yield break;

        chain.members.RemoveAll(m => m == null);
        int i = chain.members.IndexOf(this);
        for (int j = i + 1; j < chain.members.Count; j++)
        {
            StatusController s = chain.members[j].GetComponent<StatusController>();
            if (s != null) yield return s;
        }
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