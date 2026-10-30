using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyPreviewUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform enemyIconContainer;
    [SerializeField] private GameObject enemyIconPrefab;
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyDescriptionText;
    [SerializeField] private EnemyRegistry enemyRegistry;

    public EnemyRegistry Registry => enemyRegistry;

    public void Setup(LevelDefinition level)
    {
        foreach (Transform child in enemyIconContainer)
            Destroy(child.gameObject);

        enemyInfoPanel.SetActive(false);

        foreach (string enemyId in level.enemyTypes)
        {
            GameObject iconObj = Instantiate(enemyIconPrefab, enemyIconContainer);
            EnemyIconUI icon = iconObj.GetComponent<EnemyIconUI>();
            icon.Setup(enemyId, this);
        }
    }

    public void ShowEnemyInfo(string enemyId)
    {
        EnemyDefinition def = enemyRegistry.GetById(enemyId);
        if (def == null) return;

        enemyInfoPanel.SetActive(true);
        enemyNameText.text = def.displayName;
        enemyDescriptionText.text = def.description;
    }

    public void HideEnemyInfo()
    {
        enemyInfoPanel.SetActive(false);
    }
}