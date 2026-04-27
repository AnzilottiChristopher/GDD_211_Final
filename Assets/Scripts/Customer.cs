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

    [Header("Highlight")]
    [SerializeField] private Image highlightImage;

    private static readonly string[] PossibleToppings = { "Barnacles", "Pearls", "Starfish Sprinkles" };
    private static readonly string[] DoughNames = { "Kelp", "Chum", "Coral", "Jelly" };

    public int SlotIndex { get; private set; }

    private bool isServed = false;
    private Coroutine patienceCoroutine;

    // ─── Init ─────────────────────────────────────────────────────
    public void Init(int slotIndex)
    {
        SlotIndex = slotIndex;
        patience = maxPatience;
        SetHighlight(false);
        GenerateOrder();
        patienceCoroutine = GameManager.Instance.RunCoroutine(PatienceRoutine());
    }

    // ─── Highlight ────────────────────────────────────────────────
    public void SetHighlight(bool on)
    {
        Debug.Log($"SetHighlight({on}) — image is {(highlightImage == null ? "NULL" : "assigned")}");

        if (highlightImage != null)
            highlightImage.enabled = on;
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
        {
            string doughName = DoughNames[targetDough - 1];
            orderText.text = "Dough: " + doughName + "\nTopping: " + targetToppings[0];
        }
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
    public void Serve(bool correct, float quality = 1f)
    {
        if (isServed) return;
        isServed = true;
        SetHighlight(false);

        if (patienceCoroutine != null)
            GameManager.Instance.StopCoroutine(patienceCoroutine);

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