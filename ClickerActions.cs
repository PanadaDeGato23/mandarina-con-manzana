using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClickerActions : MonoBehaviour
{
    public ClickerManager manager;

    public Button upgradeButton;
    public Button upgradeButton2;
    public Button autoClickButton;
    public Button LuckyUpgrade;
    public Button clickPowerButton;

    public TextMeshProUGUI luckyText;
    public TextMeshProUGUI clickPowerText;
    public TextMeshProUGUI statsText;       // texto de "por click / por segundo"
    public Transform clickButtonTransform;  // el botón principal, para la animación

    public int upgradeCost = 50;
    public int upgradeCost2 = 150;
    public int autoClickCost = 100;
    public int luckyCost = 50;
    public int clickPowerCost = 10;
    public float luckyChance = 0.15f; // 15%

    public bool luckyBought = false;
    private bool upgradeBought = false;
    private bool upgrade2Bought = false;
    private bool autoBought = false;

    private int baseClick = 1;
    private Coroutine punchRoutine;

    void Start()
    {
        luckyText.gameObject.SetActive(false);
    }

    // Actualiza botones bloqueados y el texto de stats
    public void Refresh()
    {
        int counter = manager.counter;

        upgradeButton.interactable = !upgradeBought && counter >= upgradeCost;
        upgradeButton2.interactable = !upgrade2Bought && counter >= upgradeCost2;
        autoClickButton.interactable = !autoBought && counter >= autoClickCost;
        LuckyUpgrade.interactable = !luckyBought && counter >= luckyCost;
        clickPowerButton.interactable = counter >= clickPowerCost;

        clickPowerText.text = "+1 CLICK (" + clickPowerCost + ")";

        int perClick = baseClick * manager.clickValue;
        int perSecond = autoBought ? perClick : 0;
        statsText.text = "Por click: " + perClick + "\nPor segundo: " + perSecond;
    }

    public void CountClick()
    {
        int value = baseClick * manager.clickValue;

        if (luckyBought && Random.value < luckyChance)
        {
            value *= 2;
            ShowLuckyText("¡x2!");
        }

        manager.counter += value;
        manager.UpdateText();

        manager.audioSource.Play();   // sonido para click manual y auto click

        // animación del botón
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchScale());
    }

    // El botón crece un poco y vuelve a su tamaño en 0.15 segundos
    IEnumerator PunchScale()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.15f;
            float scale = 1f + 0.2f * Mathf.Sin(t * Mathf.PI);
            clickButtonTransform.localScale = Vector3.one * scale;
            yield return null;
        }
        clickButtonTransform.localScale = Vector3.one;
    }

    void ShowLuckyText(string message)
    {
        luckyText.text = message;
        luckyText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideLuckyText));
        Invoke(nameof(HideLuckyText), 0.5f);
    }

    void HideLuckyText()
    {
        luckyText.gameObject.SetActive(false);
    }

    public void BuyClickPower()
    {
        if (manager.counter >= clickPowerCost)
        {
            manager.counter -= clickPowerCost;
            baseClick++;
            clickPowerCost *= 2;   // 10, 20, 40, 80...
            manager.UpdateText();
        }
    }

    public void BuyUpgrade()
    {
        if (!upgradeBought && manager.counter >= upgradeCost)
        {
            manager.counter -= upgradeCost;
            manager.clickValue = 2;
            upgradeBought = true;
            manager.UpdateText();
        }
    }

    public void BuyUpgrade2()
    {
        if (!upgrade2Bought && manager.counter >= upgradeCost2)
        {
            manager.counter -= upgradeCost2;
            manager.clickValue = 4;
            upgrade2Bought = true;
            manager.UpdateText();
        }
    }

    public void BuyAutoClick()
    {
        if (!autoBought && manager.counter >= autoClickCost)
        {
            manager.counter -= autoClickCost;
            autoBought = true;
            manager.UpdateText();
            InvokeRepeating(nameof(CountClick), 1f, 1f);
        }
    }

    public void BuyLucky()
    {
        if (!luckyBought && manager.counter >= luckyCost)
        {
            manager.counter -= luckyCost;
            luckyBought = true;
            manager.UpdateText();
            ShowLuckyText("¡x2 ACTIVADO!");
        }
    }
}