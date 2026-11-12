using UnityEngine;

public class ResistanceModifier : MonoBehaviour, IDamageModifier
{
    [System.Serializable]
    public struct Entry
    {
        public DamageType type;
        public float multiplier; // 0 = immune, 0.5 = resist, 1.5 = weak
    }

    [SerializeField] private Entry[] entries;

    public int Order => 10;

    public void Modify(ref DamageInfo info)
    {
        foreach (Entry e in entries)
        {
            if (info.type.Has(e.type))
                info.amount *= e.multiplier;
        }
    }

    public bool IsImmuneTo(DamageType type)
    {
        foreach (Entry e in entries)
            if (type.Has(e.type) && e.multiplier <= 0f) return true;
        return false;
    }
}