using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConstellationMapUI : MonoBehaviour
{
    public static ConstellationMapUI main;

    [Header("References")]
    [SerializeField] private LevelRegistry levelRegistry;
    [SerializeField] private GameObject nodeParent;
    [SerializeField] private GameObject lineParent;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI selectedLevelNameText;
    [SerializeField] private TextMeshProUGUI selectedLevelRewardText;

    private List<ConstellationNode> spawnedNodes = new List<ConstellationNode>();
    private ConstellationNode selectedNode;
    private WorldDefinition currentWorld;

    private void Awake()
    {
        main = this;
        confirmButton.interactable = false;
    }

    public void LoadWorld(int worldIndex)
    {
        ClearMap();
        selectedNode = null;
        confirmButton.interactable = false;
        selectedLevelNameText.text = "";
        selectedLevelRewardText.text = "";

        currentWorld = levelRegistry.worlds[worldIndex];
        SaveData save = SaveManager.main.CurrentSave;

        int globalStartIndex = 0;
        for (int i = 0; i < worldIndex; i++)
            globalStartIndex += levelRegistry.worlds[i].levels.Count;

        // spawn nodes
        for (int i = 0; i < currentWorld.levels.Count; i++)
        {
            LevelDefinition level = currentWorld.levels[i];
            int globalIndex = globalStartIndex + i;

            if (globalIndex > save.highestLevelUnlocked)
                continue; // dont show locked levels

            bool completed = globalIndex < save.highestLevelUnlocked;

            GameObject nodeObj = Instantiate(nodePrefab, nodeParent.transform);
            RectTransform rt = nodeObj.GetComponent<RectTransform>();
            rt.anchoredPosition = level.mapPosition;

            ConstellationNode node = nodeObj.GetComponent<ConstellationNode>();
            node.Setup(level, completed);
            spawnedNodes.Add(node);
        }

        // spawn lines between connected nodes
        foreach (ConstellationNode node in spawnedNodes)
        {
            foreach (int connectionIndex in node.LevelData.connections)
            {
                // find connected node if it exists
                ConstellationNode connectedNode = spawnedNodes.Find(
                    n => currentWorld.levels.IndexOf(n.LevelData) == connectionIndex
                );

                if (connectedNode == null) continue;

                GameObject lineObj = Instantiate(linePrefab, lineParent.transform);
                ConstellationLine line = lineObj.GetComponent<ConstellationLine>();

                RectTransform fromRt = node.GetComponent<RectTransform>();
                RectTransform toRt = connectedNode.GetComponent<RectTransform>();
                line.Setup(fromRt.anchoredPosition, toRt.anchoredPosition);
            }
        }
    }

    public void OnNodeSelected(ConstellationNode node)
    {
        if (selectedNode != null)
            selectedNode.SetSelected(false);

        if (selectedNode == node)
        {
            // deselect if tapping the same node
            selectedNode = null;
            confirmButton.interactable = false;
            selectedLevelNameText.text = "";
            selectedLevelRewardText.text = "";
            return;
        }

        selectedNode = node;
        selectedNode.SetSelected(true);
        confirmButton.interactable = true;
        selectedLevelNameText.text = node.LevelData.displayName;
        selectedLevelRewardText.text = $"Essence Reward: {node.LevelData.essenceReward}";
    }

    public void OnConfirmPressed()
    {
        if (selectedNode == null) return;
        HubManager.main.OnLevelSelected(selectedNode.LevelData);
    }

    private void ClearMap()
    {
        foreach (Transform child in nodeParent.transform)
            Destroy(child.gameObject);
        foreach (Transform child in lineParent.transform)
            Destroy(child.gameObject);
        spawnedNodes.Clear();
    }
}