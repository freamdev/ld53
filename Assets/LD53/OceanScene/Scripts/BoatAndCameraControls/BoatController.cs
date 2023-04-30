using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoatController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform motorTransform;
    [SerializeField] Slider speedSlider;
    [SerializeField] Slider fuelSlider;
    [SerializeField] Light globalLight;
    [SerializeField] Renderer water;
    [SerializeField] Color waterBaseColor;
    [SerializeField] Color waterStormColor;
    [ColorUsage(true, true)]
    [SerializeField] Color rippleBaseColor;
    [ColorUsage(true, true)]
    [SerializeField] Color rippleStormColor;

    [Header("UI Things")]
    [SerializeField] TextMeshProUGUI enterCityText;
    [SerializeField] TextMeshProUGUI moneyCounter;
    [SerializeField] TradePanel tradePanel;
    [SerializeField] Transform cargoHolder;
    [SerializeField] CargoUI cargoPanelPrefab;
    [SerializeField] Image failedPanel;

    List<CargoUI> cargoPanels;

    CityController nearestCity;

    float money;
    float currentFuel;
    float maxFuel = 100;

    bool isFailing;
    bool isSucceeding;
    bool isFalling;

    float eventTimeLeft;

    Dictionary<TradeGood, CargoItem> cargo;
    Rigidbody boatBody;
    float acceleration;
    MessageShower messageShower;

    private void Awake()
    {
        AudioListener.volume = OptionsProvider.Get().AudioStrength;
    }

    // Start is called before the first frame update
    void Start()
    {
        cargo = new Dictionary<TradeGood, CargoItem>();
        cargoPanels = new List<CargoUI>();
        tradePanel.HidePanel();
        boatBody = GetComponent<Rigidbody>();
        currentFuel = maxFuel;
        ChangeMoney(DatabaseProvider.Get().StartMoney);
        messageShower = FindObjectOfType<MessageShower>();
        messageShower.ShowMessage(DatabaseProvider.Get().GetStoryPoint(StoryKey.StoryStart));

        eventTimeLeft = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("City"))
        {
            enterCityText.text = "Press E to enter the city";
            nearestCity = other.GetComponent<CityController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("City"))
        {
            enterCityText.text = "";
            nearestCity = null;
        }
    }



    void GenerateRandomEvent()
    {
        eventTimeLeft = Random.Range(5, 15);

        var eventKey = Random.Range(0, 8);
        RenderSettings.fogDensity = 0.00f;
        globalLight.intensity = 2;
        globalLight.colorTemperature = 5000;
        water.material.SetColor("_BaseColor", waterBaseColor);
        water.material.SetColor("_RippleColor", rippleBaseColor);

        if (eventKey == 0)
        {
            //Fog event
            RenderSettings.fogDensity = 0.01f;
        }
        else if (eventKey == 1)
        {
            //Heavy event
            RenderSettings.fogDensity = 0.04f;
        }
        else if (eventKey == 2)
        {
            //snowstorm
            globalLight.intensity = .7f;
            globalLight.colorTemperature = 12143;
            water.material.SetColor("_BaseColor", waterStormColor);
            water.material.SetColor("_RippleColor", rippleStormColor);
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 150 || transform.position.x < -150 || transform.position.z > 150 || transform.position.z < -150)
        {
            if (!isFailing)
            {
                messageShower.ShowMessage(DatabaseProvider.Get().GetStoryPoint(StoryKey.FallOffTheWorld));
                isFailing = true;
                boatBody.useGravity = true;
                StartCoroutine(Failed());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene((int)Scenes.MainMenu);
        }

        currentFuel -= acceleration / 4 * Time.deltaTime;
        UpdateFuelBar();

        if (Input.GetKeyDown(KeyCode.E) && nearestCity != null)
        {
            tradePanel.ShowPanel();
            tradePanel.UpdatePanel(nearestCity);
            boatBody.velocity = Vector3.zero;
            acceleration = 0;
        }

        LooseCheck();
        WinCheck();
    }

    void LooseCheck()
    {
        if (currentFuel <= 0 && (nearestCity == null || money < DatabaseProvider.Get().FuelPrice))
        {
            if (!isFailing)
            {
                messageShower.ShowMessage(DatabaseProvider.Get().GetStoryPoint(StoryKey.StoryEndFail));
                isFailing = true;
                StartCoroutine(Failed());
            }
        }
    }

    IEnumerator Failed()
    {
        var t = .0f;
        var failTime = 3f;
        while (t < failTime)
        {
            failedPanel.color = new Color(0, 0, 0, t / failTime);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        SceneManager.LoadScene((int)Scenes.MainMenu);
    }

    void WinCheck()
    {
        if (money >= DatabaseProvider.Get().WinMoney)
        {
            if (!isSucceeding)
            {
                isSucceeding = true;
                StartCoroutine(Succeeded());
            }
        }
    }

    IEnumerator Succeeded()
    {
        messageShower.ShowMessage(DatabaseProvider.Get().GetStoryPoint(StoryKey.StoryEndSuccess));
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene((int)Scenes.MainMenu);
    }

    void FixedUpdate()
    {
        RotatationMovement();
        ForwardMovement();
    }

    void RotatationMovement()
    {
        var rotationDir = 0;

        if (Input.GetKey(KeyCode.A))
        {
            rotationDir = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationDir = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            acceleration += speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            acceleration -= speed * Time.deltaTime;
        }

        acceleration = Mathf.Clamp(acceleration, 0, 5);
        speedSlider.value = acceleration / 5;

        boatBody.AddForceAtPosition(rotationDir * transform.right * rotationSpeed, motorTransform.position);
    }

    void ForwardMovement()
    {
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

        boatBody.AddForceAtPosition(forward * acceleration, motorTransform.position, ForceMode.Force);
    }

    public Dictionary<TradeGood, CargoItem> GetCargo()
    {
        return cargo;
    }

    public float GetMoney()
    {
        return money;
    }

    public void SellTradeGood(TradeGood itemToSell, float price)
    {
        //At this point we dealt with having cargo
        var cargoCount = cargo[itemToSell].ItemCount;
        ChangeMoney(cargoCount * price);

        cargo.Remove(itemToSell);
        UpdateCargoUI();
    }

    public void BuyTradeGood(TradeGood itemToBuy, int amount, float cost)
    {
        //At this point we dealt with the money and free space
        if (cargo.ContainsKey(itemToBuy))
        {
            cargo[itemToBuy].ItemCount += amount;
        }
        else
        {
            cargo.Add(itemToBuy, new CargoItem
            {
                Item = itemToBuy,
                OriginalPrice = cost,
                ItemCount = amount
            });
        }

        UpdateCargoUI();
    }

    public void BuyFuel()
    {
        if (money > DatabaseProvider.Get().FuelPrice)
        {
            ChangeMoney(-DatabaseProvider.Get().FuelPrice);
            GetFuel(maxFuel * .1f);
        }
    }

    public void GetFuel(float value)
    {
        currentFuel = Mathf.Min(currentFuel + value, maxFuel);
    }

    public void ChangeMoney(float value)
    {
        money += value;
        UpdateMoneyText();
    }

    void UpdateFuelBar()
    {
        fuelSlider.value = currentFuel / maxFuel;
    }

    void UpdateMoneyText()
    {
        moneyCounter.text = $"Money: {money}$";
    }

    void UpdateCargoUI()
    {
        if (cargoPanels != null)
        {
            cargoPanels.ForEach(f => Destroy(f.gameObject));
        }

        cargoPanels = new List<CargoUI>();

        foreach (var cargoItem in cargo)
        {
            var newCargoPanel = Instantiate(cargoPanelPrefab, cargoHolder);
            newCargoPanel.Init(cargoItem.Value);
            cargoPanels.Add(newCargoPanel);
        }
    }

    public void DropCargo()
    {
        var sumCargoValue = cargo.Sum(s => s.Value.ItemCount * s.Value.OriginalPrice);
        ChangeMoney(sumCargoValue * .5f);
        cargo.Clear();
        UpdateCargoUI();
    }
}

public class CargoItem
{
    public TradeGood Item;
    public float OriginalPrice;
    public int ItemCount;
}