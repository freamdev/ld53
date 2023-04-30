using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CityController : MonoBehaviour
{
    [Header("City values")]
    [SerializeField] string cityName;

    [Header("Trading things")]
    [SerializeField] GameObject orderIconPrefab;
    [SerializeField] Transform orderIconsHolder;
    [SerializeField] List<CityTradeData> tradeData;
    Dictionary<CityTradeData, GameObject> orderIcons;

    private void Awake()
    {
        var particleSystem = GetComponentInChildren<ParticleSystem>();
        var collider = GetComponent<SphereCollider>();

        var t = particleSystem.shape;
        t.radius = collider.radius;

        orderIcons = new Dictionary<CityTradeData, GameObject>();

        foreach (var item in tradeData)
        {
            var newIcon = Instantiate(orderIconPrefab, orderIconsHolder);
            orderIcons.Add(item, newIcon);
        }
    }

    private void Update()
    {
        foreach (var item in tradeData)
        {
            item.TimeLeft -= Time.deltaTime;
            if (item.TimeLeft <= 0)
            {
                item.Reset(tradeData.Select(s => s.Item).ToList());
            }
        }
        UpdateOrderIcons();
    }

    public void UpdateOrderIcons()
    {
        foreach (var item in orderIcons)
        {
            var image = item.Value.transform.Find("Timer").GetComponent<Image>();
            image.color = Color.Lerp(Color.red, Color.green, ((item.Key.Need + 1f) / 2f));
            image.fillAmount = item.Key.GetTimer();
            item.Value.transform.Find("GoodsImage").GetComponent<Image>().sprite = item.Key.Item.Icon;
        }
    }

    public string GetCityName()
    {
        return cityName;
    }

    public List<CityTradeData> GetTradeData()
    {
        return tradeData;
    }
}

[System.Serializable]
public class CityTradeData
{
    public TradeGood Item;
    [Range(-1, 1)]
    public float Need;
    public float TimeLeft;
    public float LastLength;

    public void Reset(List<TradeGood> currentItems)
    {
        Item = Resources.LoadAll<TradeGood>("TradeGoods").Where(w => !currentItems.Contains(w)).OrderBy(o => System.Guid.NewGuid()).FirstOrDefault();
        LastLength = Random.Range(30, 50);
        Need = Random.Range(-1f, 1f);
        TimeLeft = LastLength;
    }

    public float GetTimer()
    {
        return TimeLeft / LastLength;
    }

    public int GetSellCost()
    {
        return (int)(Item.BaseValue * (1 + Need));
    }

    public int GetBuyCost()
    {
        return (int)(Item.BaseValue * (1 + Need));
    }
}