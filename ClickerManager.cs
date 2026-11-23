using UnityEngine;
using TMPro;

public class ClickerManager : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public AudioSource audioSource;
    public ClickerActions actions;

    public int counter = 0;
    public int clickValue = 1;

    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        counterText.text = counter.ToString();
        actions.Refresh();   // actualiza botones bloqueados y textos
    }

    public void CountClick() { actions.CountClick(); }
    public void BuyUpgrade() { actions.BuyUpgrade(); }
    public void BuyUpgrade2() { actions.BuyUpgrade2(); }
    public void BuyAutoClick() { actions.BuyAutoClick(); }
    public void BuyLucky() { actions.BuyLucky(); }
    public void BuyClickPower() { actions.BuyClickPower(); }
}
