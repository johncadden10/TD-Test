using UnityEngine;

public class LoadoutGhostInfoUI : MonoBehaviour
{
    public static LoadoutGhostInfoUI main;

    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GhostDetailUI ghostDetailUI;
    [SerializeField] private GhostGridUI ghostGridUI;

    private void Awake()
    {
        main = this;
        panel.SetActive(false);
    }

    public void OpenForTower(TowerDefinition tower)
    {
        panel.SetActive(true);
        ghostDetailUI.Setup(tower, null); // null as no grid to return to
        ghostGridUI.gameObject.SetActive(false);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}