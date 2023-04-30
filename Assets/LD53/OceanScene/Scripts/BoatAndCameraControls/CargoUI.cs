using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI count;

    public void Init(CargoItem item)
    {
        icon.sprite = item.Item.Icon;
        count.text = $"{item.ItemCount}/{DatabaseProvider.Get().MaxCargoSlotSize}";
    }
}
