using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItemName;
    [SerializeField] Image Icon;

    [SerializeField] Button BuyButton;
    [SerializeField] Button SellButton;

    TextMeshProUGUI BuyText;
    TextMeshProUGUI SellText;

    CityTradeData data;

    TradePanel tradePanel;

    private void Awake()
    {
        BuyText = BuyButton.GetComponentInChildren<TextMeshProUGUI>();
        SellText = SellButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(CityTradeData tradegood, TradePanel parent)
    {
        data = tradegood;
        tradePanel = parent;

        ItemName.text = tradegood.Item.Name;
        Icon.sprite = tradegood.Item.Icon;

        BuyText.text = tradegood.GetBuyCost().ToString("");
        SellText.text = tradegood.GetSellCost().ToString("");

        BuyButton.onClick.AddListener(() => BuyItem());
        SellButton.onClick.AddListener(() => SellItem());

        //if (tradegood.Need > 0)
        //{
        //    BuyButton.interactable = false;
        //}
    }

    private void BuyItem()
    {
        var player = FindObjectOfType<BoatController>();
        var playerCargo = player.GetCargo();
        var playerMoney = player.GetMoney();

        var errorMessage = "";
        var hadAnyError = false;

        if (!playerCargo.ContainsKey(data.Item) && playerCargo.Count >= DatabaseProvider.Get().MaxCargoCount)
        {
            //This means the player DONT have this item, but also his cargo is full, cant buy
            errorMessage = "Not enough cargo space!";
            hadAnyError = true;
        }

        if (playerCargo.ContainsKey(data.Item) && playerCargo[data.Item].ItemCount >= DatabaseProvider.Get().MaxCargoSlotSize)
        {
            //This means the player has this item but already at max capacity from it, cant buy
            errorMessage = "Cargo at max capacity!";
            hadAnyError = true;
        }

        if (playerMoney < data.GetBuyCost())
        {
            //This means the player dont have enough money, cant buy
            errorMessage = "Not enough money!";
            hadAnyError = true;
        }

        if (hadAnyError)
        {
            tradePanel.ShowError(errorMessage);
            StartCoroutine(HideError());
            return;
        }


        //If we got here we can trade finally, take money, give goods
        player.ChangeMoney(-data.GetBuyCost());
        player.BuyTradeGood(data.Item, 1, data.GetBuyCost());
    }

    IEnumerator HideError()
    {
        yield return new WaitForSeconds(1f);
        tradePanel.HideError();
    }

    private void SellItem()
    {
        var player = FindObjectOfType<BoatController>();
        var playerCargo = player.GetCargo();

        if (!playerCargo.ContainsKey(data.Item))
        {
            //This means the player dont have this cargo, no sell
            return;
        }

        player.SellTradeGood(data.Item, data.GetSellCost());
    }
}
