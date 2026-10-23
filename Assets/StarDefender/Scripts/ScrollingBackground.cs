using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 3f;
    public float frameHeight = 20f;

    public Transform[] frames;

    void Update()
    {
        foreach(Transform frame in frames)
        {
            frame.position += Vector3.back * scrollSpeed * Time.deltaTime;

            if (frame.position.z <= -20f)
            {
                frame.position += Vector3.forward * frameHeight * 3;
            }
        }
    }
}
