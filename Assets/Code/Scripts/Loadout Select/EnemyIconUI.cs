using UnityEngine;
using UnityEngine.UI;

public class EnemyIconUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private string enemyId;
    private EnemyPreviewUI previewUI;

    public void Setup(string id, EnemyPreviewUI preview)
    {
        enemyId = id;
        previewUI = preview;

        EnemyDefinition def = preview.Registry.GetById(id);
        if (def != null)
            iconImage.sprite = def.icon;
        else
            Debug.LogWarning($"No enemy definition found for id '{id}'");
    }

    public void OnIconPressed()
    {
        previewUI.ShowEnemyInfo(enemyId);
    }
}