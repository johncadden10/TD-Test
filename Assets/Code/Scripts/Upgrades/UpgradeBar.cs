using UnityEngine;

public class UpgradeBar : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject upgradeButtonPrefab;

    private void Start()
    {
        foreach (LevelLoadout.Entry entry in LevelLoadout.entries)
        {
            GameObject obj = Instantiate(upgradeButtonPrefab, container);
            obj.GetComponent<UpgradeButton>().Setup(entry);
        }
    }
}