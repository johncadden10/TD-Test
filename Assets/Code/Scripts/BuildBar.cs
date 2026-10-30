using UnityEngine;

public class BuildBar : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject buildButtonPrefab;

    private void Start()
    {
        foreach (LevelLoadout.Entry entry in LevelLoadout.entries)
        {
            GameObject obj = Instantiate(buildButtonPrefab, container);
            obj.GetComponent<BuildButton>().Setup(entry);
        }
    }
}