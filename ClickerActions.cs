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
    public TextMeshProUGUI statsText;
    public Transform clickButtonTransform;

    // Sprite del botón principal
    public Image clickButtonImage;                    // el Image del Button principal
    public Sprite[] sprites;                          // el primero es el inicial
    public int[] spriteGoals = { 0, 50, 150, 400 };   // clicks totales para cada sprite

    // Códigos
    public TMP_InputField codeInput;
    public TextMeshProUGUI messageText;

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
    private bool mandarinaUsed = false;

    private int baseClick = 1;
    private int totalEarned = 0;
    private Coroutine punchRoutine;

    void Start()
    {
        luckyText.gameObject.SetActive(false);
        messageText.text = "";
    }

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

        UpdateSprite();
    }

    // Todo lo que suma clicks pasa por aquí
    void AddClicks(int amount)
    {
        manager.counter += amount;
        totalEarned += amount;
        manager.UpdateText();
    }

    void UpdateSprite()
    {
        for (int i = 0; i < sprites.Length && i < spriteGoals.Length; i++)
        {
            if (totalEarned >= spriteGoals[i] && sprites[i] != null)
            {
                clickButtonImage.sprite = sprites[i];
            }
        }
    }

    public void CountClick()
    {
        int value = baseClick * manager.clickValue;

        if (luckyBought && Random.value < luckyChance)
        {
            value *= 2;
            ShowLuckyText("¡x2!");
        }

        AddClicks(value);
        manager.audioSource.Play();

        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchScale());
    }

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

    public void RedeemCode()
    {
        string code = codeInput.text.Trim().ToLower();
        codeInput.text = "";

        if (code == "mandarina")
        {
            if (mandarinaUsed)
            {
                ShowMessage("Ya canjeaste este código");
            }
            else
            {
                mandarinaUsed = true;
                AddClicks(1000);
                ShowMessage("¡Código canjeado! +1000 clicks");
            }
        }
        else
        {
            ShowMessage("Código inválido");
        }
    }

    void ShowMessage(string message)
    {
        messageText.text = message;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), 2f);
    }

    void ClearMessage()
    {
        messageText.text = "";
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