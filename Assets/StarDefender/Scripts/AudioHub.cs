using UnityEngine;

/// <summary>
/// Simple audio cues generated at runtime (no external files required).
/// </summary>
public class AudioHub : MonoBehaviour
{
    public static AudioHub Instance { get; private set; }

    AudioSource source;
    AudioClip shootClip;
    AudioClip loseLifeClip;
    AudioClip killClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;

        shootClip = MakeBeep(880f, 0.06f);
        loseLifeClip = MakeBeep(220f, 0.25f);
        killClip = MakeBeep(520f, 0.12f);
    }

    public static void PlayShoot()
    {
        if (Instance != null) Instance.source.PlayOneShot(Instance.shootClip, 0.5f);
    }

    public static void PlayLoseLife()
    {
        if (Instance != null) Instance.source.PlayOneShot(Instance.loseLifeClip, 0.7f);
    }

    public static void PlayKill()
    {
        if (Instance != null) Instance.source.PlayOneShot(Instance.killClip, 0.6f);
    }

    static AudioClip MakeBeep(float freq, float duration)
    {
        int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        var clip = AudioClip.Create("beep", samples, 1, sampleRate, false);
        var data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * 0.4f;
        }
        clip.SetData(data, 0);
        return clip;
    }
}
