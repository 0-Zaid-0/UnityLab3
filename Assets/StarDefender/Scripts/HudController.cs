using UnityEngine;

public class HudController : MonoBehaviour
{
    GUIStyle scoreStyle;
    GUIStyle livesStyle;
    GUIStyle gameOverStyle;
    bool ready;

    void EnsureStyles()
    {
        if (ready) return;
        ready = true;
        scoreStyle = new GUIStyle(GUI.skin.label) { fontSize = 32, fontStyle = FontStyle.Bold };
        scoreStyle.normal.textColor = Color.white;
        livesStyle = new GUIStyle(GUI.skin.label) { fontSize = 32, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperRight };
        livesStyle.normal.textColor = new Color(0.4f, 0.95f, 1f);
        gameOverStyle = new GUIStyle(GUI.skin.label) { fontSize = 44, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        gameOverStyle.normal.textColor = new Color(1f, 0.3f, 0.3f);
    }

    void OnGUI()
    {
        EnsureStyles();
        if (GameManager.Instance == null) return;

        GUI.Box(new Rect(8, 8, 260, 56), "");
        GUI.Label(new Rect(20, 16, 240, 40), "SCORE  " + GameManager.Instance.Score, scoreStyle);

        GUI.Box(new Rect(Screen.width - 268, 8, 260, 56), "");
        GUI.Label(new Rect(Screen.width - 260, 16, 240, 40), "LIVES  " + GameManager.Instance.Lives, livesStyle);

        if (GameManager.Instance.IsGameOver)
        {
            GUI.Box(new Rect(Screen.width * 0.25f, Screen.height * 0.4f, Screen.width * 0.5f, 100), "");
            GUI.Label(new Rect(0, Screen.height * 0.4f, Screen.width, 100), "GAME OVER\nPress R to Restart", gameOverStyle);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameOver) return;
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null && kb.rKey.wasPressedThisFrame)
            GameManager.Instance.RestartLevel();
    }
}
