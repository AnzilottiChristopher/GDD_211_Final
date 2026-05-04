using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class Customer : MonoBehaviour
{
    [Header("Customer Patience Levels")]
    [SerializeField] private TextMeshProUGUI patienceText;
    [SerializeField] private float maxPatience = 25f;
    private float patience;

    [Header("Order Display")]
    [SerializeField] private Image doughOrderImage;
    [SerializeField] private Image toppingOrderImage;

    [Header("Order Data")]
    [SerializeField] private int targetDough;
    [SerializeField] private List<string> targetToppings = new List<string>();

    [Header("Highlight")]
    [SerializeField] private Image highlightImage;

    [Header("Character")]
    [SerializeField] private Image characterImage;

    [Header("Clothesline")]
    [SerializeField] private Clothesline clothesline;

    private static readonly string[] PossibleToppings = { "Barnacles", "Pearls", "Starfish Sprinkles" };
    private static readonly string[] DoughNames = { "Kelp", "Chum", "Coral", "Jelly" };

    public int SlotIndex { get; private set; }

    private bool isServed = false;
    private Coroutine patienceCoroutine;

    public void Init(int slotIndex)
    {
        SlotIndex = slotIndex;
        patience = maxPatience;
        SetHighlight(false);
        clothesline = FindFirstObjectByType<Clothesline>(FindObjectsInactive.Include);

        if (characterImage != null)
        {
            Sprite s = GameManager.Instance.GetRandomCustomerSprite();
            Debug.Log("Setting character sprite to: " + s.name);
            characterImage.sprite = s;
        }

        GenerateOrder();
        patienceCoroutine = GameManager.Instance.RunCoroutine(PatienceRoutine());
    }

    public void SetHighlight(bool on)
    {
        if (highlightImage != null)
            highlightImage.enabled = on;
    }

    private IEnumerator PatienceRoutine()
    {
        while (patience > 0f && !isServed)
        {
            yield return null;
            patience -= Time.deltaTime;
            UpdatePatienceUI();
        }

        if (!isServed)
        {
            patience = 0f;
            OnPatienceExpired();
        }
    }

    private void GenerateOrder()
    {
        targetToppings.Clear();
        targetDough = Random.Range(1, 5);
        targetToppings.Add(PossibleToppings[Random.Range(0, PossibleToppings.Length)]);
        UpdateOrderUI();

        // Add a ticket to the clothesline for this order
        if (clothesline != null)
            clothesline.AddTicket(SlotIndex, targetDough, targetToppings[0]);
        else
            Debug.LogWarning("[Customer] No Clothesline reference assigned!");
    }

    private void UpdateOrderUI()
    {
        if (doughOrderImage != null)
            doughOrderImage.sprite = GameManager.Instance.GetDoughSprite(targetDough);

        if (toppingOrderImage != null)
        {
            int toppingIndex = System.Array.IndexOf(PossibleToppings, targetToppings[0]);
            if (toppingIndex >= 0)
                toppingOrderImage.sprite = GameManager.Instance.GetToppingSprite(toppingIndex);
        }
    }

    private void UpdatePatienceUI()
    {
        if (patienceText != null)
            patienceText.text = Mathf.CeilToInt(patience) + "s";
    }

    public bool CheckOrder(int servedDough, List<string> servedToppings)
    {
        if (servedDough != targetDough) return false;
        if (servedToppings == null || servedToppings.Count == 0) return false;
        if (!targetToppings.Contains(servedToppings[0])) return false;
        return true;
    }

    public void Serve(bool correct, float quality = 1f)
    {
        if (isServed) return;
        isServed = true;
        SetHighlight(false);

        if (patienceCoroutine != null)
            GameManager.Instance.StopCoroutine(patienceCoroutine);

        // Remove the ticket from the clothesline
        if (clothesline != null)
            clothesline.RemoveTicket(SlotIndex);

        if (correct)
        {
            int basePoints = 10 + Mathf.FloorToInt(patience);
            int finalPoints = Mathf.RoundToInt(basePoints * quality);
            GameManager.Instance.AddScore(finalPoints);
            Debug.Log("Correct order! Quality: " + quality.ToString("F2") + " +" + finalPoints + " points");
        }
        else
        {
            Debug.Log("Wrong order — customer unhappy.");
        }

        GameManager.Instance.ReleaseSlot(SlotIndex);
    }

    private void OnPatienceExpired()
    {
        if (isServed) return;
        isServed = true;

        if (patienceCoroutine != null)
            GameManager.Instance.StopCoroutine(patienceCoroutine);

        // Remove the ticket when customer leaves
        if (clothesline != null)
            clothesline.RemoveTicket(SlotIndex);

        Debug.Log("Customer left — patience ran out.");
        GameManager.Instance.ReleaseSlot(SlotIndex);
    }
}