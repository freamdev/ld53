using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI welcomeText;
    [SerializeField] Button buyFuelButton;
    [SerializeField] BoatController player;
    [SerializeField] Transform tradeGoodsPanel;
    [SerializeField] TradeItemUI tradeGoodItemPrefab;
    [SerializeField] Transform errorPanel;
    [SerializeField] TextMeshProUGUI errorText;

    List<GameObject> tradePanels;

    public void HidePanel()
    {
        if (tradePanels != null && tradePanels.Count > 0)
        {
            tradePanels.ForEach(f => Destroy(f));
        }
        HideError();
        transform.rotation = Quaternion.Euler(Vector3.up * 90);
    }

    public void ShowPanel()
    {
        HidePanel();
        tradePanels = new List<GameObject>();
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void UpdatePanel(CityController city)
    {
        welcomeText.text = $"Welcome to {city.GetCityName()} city!";
        buyFuelButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy fuel({DatabaseProvider.Get().FuelPrice}$)";

        var tradeItems = city.GetTradeData();

        foreach (var item in tradeItems)
        {
            var newPanelItem = Instantiate(tradeGoodItemPrefab, tradeGoodsPanel);
            tradePanels.Add(newPanelItem.gameObject);
            newPanelItem.Init(item, this);
        }
    }

    public void ShowError(string text)
    {
        errorText.text = text;
        errorPanel.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void HideError()
    {
        errorText.text = "";
        errorPanel.transform.rotation = Quaternion.Euler(Vector3.right * 90);
    }

    public void BuyFuel()
    {
        player.BuyFuel();
    }

    public void DropCargo()
    {
        player.DropCargo();
    }
}
