using UnityEngine;
using TMPro;

public class GameUIFix : MonoBehaviour
{
    public static GameUIFix Instance;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;

    private void Awake()
    {
        Instance = this;
        if (resultText != null) resultText.gameObject.SetActive(false);
    }

    public void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(Mathf.Max(0, time));
    }

    public void ShowWinner(string name, int score)
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = $" {name} Wins!\nScore: {score}";
        }
    }
}