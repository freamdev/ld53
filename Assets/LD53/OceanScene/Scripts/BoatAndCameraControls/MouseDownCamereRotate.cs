using Cinemachine;
using UnityEngine;

public class MouseDownCamereRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }

        return UnityEngine.Input.GetAxis(axisName);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
