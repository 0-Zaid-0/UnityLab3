using UnityEngine;

/// <summary>
/// Shared health for invaders. Dies after two standard bullet hits (or one powered shot).
/// </summary>
public class InvaderHealth : MonoBehaviour
{
    [SerializeField] int maxHits = 2;
    [SerializeField] int scoreValue = 100;

    int hitsRemaining;

    void Awake()
    {
        hitsRemaining = maxHits;
    }

    public void TakeDamage(int amount)
    {
        hitsRemaining -= amount;
        if (hitsRemaining > 0) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        AudioHub.PlayKill();
        Destroy(gameObject);
    }
}
