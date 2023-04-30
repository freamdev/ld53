using System.Collections;
using TMPro;
using UnityEngine;

public class MessageShower : MonoBehaviour
{
    public TextMeshProUGUI storyText;

    private void Awake()
    {
        HideMessage();
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();
        HideMessage();
        transform.rotation = Quaternion.Euler(Vector3.zero);
        StartCoroutine(PrintMessage(message));
    }

    IEnumerator PrintMessage(string m)
    {
        var i = 0;
        while (i < m.Length)
        {
            storyText.text = m.Substring(0, i);
            i++;
            yield return new WaitForSeconds(.08f);
        }

        yield return new WaitForSeconds(1.3f);
        HideMessage();
    }

    public void HideMessage()
    {
        transform.rotation = Quaternion.Euler(Vector3.up * 90);
        storyText.text = "";
    }
}
