using UnityEngine;

public class Billboard : MonoBehaviour
{
    Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation * originalRotation;
    }
}
