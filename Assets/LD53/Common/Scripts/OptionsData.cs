using UnityEngine;

public class OptionsData : MonoBehaviour
{
    //Enough of serialize fields...
    public float AudioStrength = .5f;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public static class OptionsProvider
{
    static OptionsData options;

    public static OptionsData Get()
    {
        if (options == null)
        {
            options = Resources.Load<OptionsData>("Options");
        }

        return options;
    }
}
