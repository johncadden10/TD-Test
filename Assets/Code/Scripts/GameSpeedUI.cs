using UnityEngine;
using TMPro;

public class GameSpeedUI : MonoBehaviour
{
    [SerializeField] private float[] speeds = { 1f, 1.5f, 2f, 3f };
    [SerializeField] private TextMeshProUGUI label; // optional
    [SerializeField] private bool enableHotkeys = true;

    private int index;

    private void Start()
    {
        index = Mathf.Max(0, System.Array.IndexOf(speeds, GameSpeed.Speed));
        Refresh();
    }

    // wire to a button's OnClick
    public void Cycle()
    {
        index = (index + 1) % speeds.Length;
        GameSpeed.SetSpeed(speeds[index]);
        Refresh();
    }

    private void Update()
    {
        if (!enableHotkeys) return;

        if (Input.GetKeyDown(KeyCode.Equals)) Step(1);
        if (Input.GetKeyDown(KeyCode.Minus))  Step(-1);
        if (Input.GetKeyDown(KeyCode.Alpha0)) { index = 0; GameSpeed.SetSpeed(speeds[0]); Refresh(); }
    }

    private void Step(int dir)
    {
        index = Mathf.Clamp(index + dir, 0, speeds.Length - 1);
        GameSpeed.SetSpeed(speeds[index]);
        Refresh();
    }

    private void Refresh()
    {
        if (label != null) label.text = $"{speeds[index]:0.#}x";
    }
}