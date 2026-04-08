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
    [SerializeField] private TextMeshProUGUI orderText;

    [Header("Order Data")]
    [SerializeField] private int targetDough;
    [SerializeField] private List<string> targetToppings = new List<string>();

    private static readonly string[] PossibleToppings = { "Krill", "Seaweed", "Starfish Sprinkles" };

    public int SlotIndex { get; private set; }

    private bool isServed = false;
    private Button clickButton;
    private Coroutine patienceCoroutine;

    // ─── Init ─────────────────────────────────────────────────────
    public void Init(int slotIndex)
    {
        SlotIndex = slotIndex;
        patience = maxPatience;

        clickButton = GetComponentInChildren<Button>();
        Debug.Log("Button Found: " + (clickButton != null));
        if (clickButton != null)
            clickButton.onClick.AddListener(() => GameManager.Instance.SelectCustomer(this));

        GenerateOrder();
        patienceCoroutine = GameManager.Instance.RunCoroutine(PatienceRoutine());
    }

    // ─── Patience Coroutine ───────────────────────────────────────
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

    // ─── Order Generation ─────────────────────────────────────────
    private void GenerateOrder()
    {
        targetToppings.Clear();
        targetDough = Random.Range(1, 5);
        targetToppings.Add(PossibleToppings[Random.Range(0, PossibleToppings.Length)]);
        UpdateOrderUI();
    }

    private void UpdateOrderUI()
    {
        if (orderText != null)
            orderText.text = "Dough: " + targetDough + "\nTopping: " + targetToppings[0];
    }

    private void UpdatePatienceUI()
    {
        if (patienceText != null)
            patienceText.text = Mathf.CeilToInt(patience) + "s";
    }

    // ─── Order Checking ───────────────────────────────────────────
    public bool CheckOrder(int servedDough, List<string> servedToppings)
    {
        if (servedDough != targetDough) return false;
        if (servedToppings == null || servedToppings.Count == 0) return false;
        if (!targetToppings.Contains(servedToppings[0])) return false;
        return true;
    }

    // ─── Serve ────────────────────────────────────────────────────
    // quality comes from the oven minigame (0.0 - 1.0)
    public void Serve(bool correct, float quality = 1f)
    {
        if (isServed) return;
        isServed = true;

        if (patienceCoroutine != null)
            GameManager.Instance.StopCoroutine(patienceCoroutine);

        if (correct)
        {
            // Base points + patience bonus, scaled by cook quality
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

    // ─── Patience Expired ─────────────────────────────────────────
    private void OnPatienceExpired()
    {
        if (isServed) return;
        isServed = true;

        if (patienceCoroutine != null)
            GameManager.Instance.StopCoroutine(patienceCoroutine);

        Debug.Log("Customer left — patience ran out.");
        GameManager.Instance.ReleaseSlot(SlotIndex);
    }
}