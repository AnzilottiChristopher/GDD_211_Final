using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Orchestrates the full cookie-building flow:
// 1. Player selects dough type (button) → DoughMinigame activates
// 2. DoughMinigame completes → OvenMinigame activates
// 3. OvenMinigame completes (with quality) → ToppingMinigame activates
// 4. ToppingMinigame completes → cookie is ready to serve
public class CookieBuilder : MonoBehaviour
{
    [Header("Cookie State")]
    private int dough = -1;
    private List<string> toppings = new List<string>();
    private float cookQuality = 1f;     // set by oven minigame, affects score

    [Header("Minigames")]
    [SerializeField] private Doughminigame doughMinigame;
    [SerializeField] private Ovenminigame ovenMinigame;
    [SerializeField] private Toppingminigame toppingMinigame;

    [Header("Minigame Panels")]
    // Each minigame lives in its own panel — enable/disable to show/hide
    [SerializeField] private GameObject doughMinigamePanel;
    [SerializeField] private GameObject ovenMinigamePanel;
    [SerializeField] private GameObject toppingMinigamePanel;

    [Header("Serve")]
    [SerializeField] private Button serveButton;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isReady = false;

    void Start()
    {
        SetServeButtonActive(false);
        HideAllMinigamePanels();
        UpdateStatus("Select a dough to begin!");

        // Subscribe to minigame completion events
        if (doughMinigame != null)
            doughMinigame.OnComplete += OnDoughComplete;
        if (ovenMinigame != null)
            ovenMinigame.OnComplete += OnOvenComplete;
        if (toppingMinigame != null)
            toppingMinigame.OnComplete += OnToppingComplete;
    }

    void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        if (doughMinigame != null)
            doughMinigame.OnComplete -= OnDoughComplete;
        if (ovenMinigame != null)
            ovenMinigame.OnComplete -= OnOvenComplete;
        if (toppingMinigame != null)
            toppingMinigame.OnComplete -= OnToppingComplete;
    }

    // ─── Step 1: Dough Selection ──────────────────────────────────
    // Wire each dough button to SelectDough(int) with value 1-4
    public void SelectDough(int newDough)
    {
        if (isReady) return;

        dough = newDough;
        Debug.Log("Selected dough: " + dough);
        UpdateStatus("Dough " + dough + " selected — mix it up!");

        // Activate dough minigame
        ShowPanel(doughMinigamePanel);
        if (doughMinigame != null)
            doughMinigame.StartMinigame();
    }

    // ─── Step 2: Dough Minigame Complete → Oven ───────────────────
    private void OnDoughComplete()
    {
        HideAllMinigamePanels();
        UpdateStatus("Dough mixed! Into the oven...");

        ShowPanel(ovenMinigamePanel);
        if (ovenMinigame != null)
            ovenMinigame.StartMinigame();
    }

    // ─── Step 3: Oven Complete → Toppings ─────────────────────────
    private void OnOvenComplete(float quality)
    {
        cookQuality = quality;
        HideAllMinigamePanels();
        UpdateStatus("Baked! Now add toppings.");

        ShowPanel(toppingMinigamePanel);
        if (toppingMinigame != null)
            toppingMinigame.StartMinigame();
    }

    // ─── Step 4: Topping Minigame Complete → Ready to Serve ───────
    private void OnToppingComplete()
    {
        HideAllMinigamePanels();
        isReady = true;
        UpdateStatus("Cookie ready! Click a customer then hit Serve.");
        SetServeButtonActive(true);
    }

    // ─── Topping selection ────────────────────────────────────────
    // Still wire topping buttons to this — determines WHICH topping
    // goes on, separate from the minigame
    public void AddTopping(string topping)
    {
        toppings.Clear();
        toppings.Add(topping);
        Debug.Log("Added topping: " + topping);
    }

    // ─── Serving ──────────────────────────────────────────────────
    public void Serve()
    {
        if (!isReady)
        {
            UpdateStatus("Finish making the cookie first!");
            return;
        }

        Customer target = GameManager.Instance.GetSelectedCustomer();
        if (target == null)
        {
            UpdateStatus("Click a customer first!");
            return;
        }

        bool correct = target.CheckOrder(dough, toppings);
        
        // Apply quality modifier to score — handled in Customer.Serve()
        // Pass quality through so Customer can scale points
        target.Serve(correct, cookQuality);

        UpdateStatus(correct ? "Perfect order!" : "Wrong order...");
        ResetCookie();
    }

    // ─── Reset ────────────────────────────────────────────────────
    public void ResetCookie()
    {
        dough = -1;
        toppings.Clear();
        cookQuality = 1f;
        isReady = false;
        HideAllMinigamePanels();
        SetServeButtonActive(false);
        UpdateStatus("Select a dough to begin!");
    }

    // ─── Helpers ──────────────────────────────────────────────────
    public int GetDough() => dough;
    public List<string> GetToppings() => toppings;

    private void ShowPanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(true);
    }

    private void HideAllMinigamePanels()
    {
        if (doughMinigamePanel != null)    doughMinigamePanel.SetActive(false);
        if (ovenMinigamePanel != null)     ovenMinigamePanel.SetActive(false);
        if (toppingMinigamePanel != null)  toppingMinigamePanel.SetActive(false);
    }

    private void SetServeButtonActive(bool active)
    {
        if (serveButton != null) serveButton.interactable = active;
    }

    private void UpdateStatus(string msg)
    {
        Debug.Log("[CookieBuilder] " + msg);
        if (statusText != null) statusText.text = msg;
    }
}