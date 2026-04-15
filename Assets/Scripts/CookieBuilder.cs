using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CookieBuilder : MonoBehaviour
{
    [Header("Cookie State")]
    private int dough = -1;
    private List<string> toppings = new List<string>();
    private float cookQuality = 1f;

    [Header("Minigames")]
    [SerializeField] private Doughminigame doughMinigame;
    [SerializeField] private Ovenminigame ovenMinigame;
    [SerializeField] private Toppingminigame toppingMinigame;

    [Header("Minigame Panels")]
    [SerializeField] private GameObject doughMinigamePanel;
    [SerializeField] private GameObject ovenMinigamePanel;
    [SerializeField] private GameObject toppingMinigamePanel;

    [Header("Buttons")]
    [SerializeField] private Button bakeButton;
    [SerializeField] private Button serveButton;
    [SerializeField] private Button trashButton;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isReady = false;
    private bool isBaking = false;
    private static readonly string[] DoughNames = { "Kelp", "Chum", "Coral", "Jelly" };


    void Start()
    {
        SetTrashButtonActive(false);
        SetServeButtonActive(false);
        SetBakeButtonActive(true);
        HideAllMinigamePanels();
        UpdateStatus("Select a dough and topping, then hit Bake!");

        // Subscribe to minigame completion events
        if (doughMinigame != null)
            doughMinigame.OnComplete += OnDoughMinigameComplete;
        if (ovenMinigame != null)
            ovenMinigame.OnComplete += OnOvenMinigameComplete;
        if (toppingMinigame != null)
            toppingMinigame.OnComplete += OnToppingMinigameComplete;
    }

    void OnDestroy()
    {
        if (doughMinigame != null)
            doughMinigame.OnComplete -= OnDoughMinigameComplete;
        if (ovenMinigame != null)
            ovenMinigame.OnComplete -= OnOvenMinigameComplete;
        if (toppingMinigame != null)
            toppingMinigame.OnComplete -= OnToppingMinigameComplete;
    }

    // ─── Step 1: Dough Selection (just stores value) ──────────────
    public void SelectDough(int newDough)
    {
        if (isBaking || isReady) return;
        dough = newDough;
        string doughName = DoughNames[newDough - 1];
        Debug.Log("Selected dough: " + doughName);
        UpdateStatus(doughName + " dough selected. Now pick a topping!");
    }

    // ─── Step 2: Topping Selection (just stores value) ────────────
    public void AddTopping(string topping)
    {
        if (isBaking || isReady) return;
        toppings.Clear();
        toppings.Add(topping);
        Debug.Log("Added topping: " + topping);
        UpdateStatus("Topping: " + topping + ". Ready to bake!");
    }

    // ─── Step 3: Bake Button → triggers minigame chain ────────────
    public void StartBake()
    {
        if (isBaking || isReady) return;

        if (dough == -1)
        {
            UpdateStatus("Pick a dough first!");
            return;
        }
        if (toppings.Count == 0)
        {
            UpdateStatus("Pick a topping first!");
            return;
        }

        isBaking = true;
        SetBakeButtonActive(false);
        UpdateStatus("Mix the dough!");

        // Start with dough minigame
        ShowPanel(doughMinigamePanel);
        if (doughMinigame != null)
            doughMinigame.StartMinigame();
    }

    // ─── Dough Minigame Complete → Oven ───────────────────────────
    private void OnDoughMinigameComplete()
    {
        HideAllMinigamePanels();
        UpdateStatus("Dough mixed! Into the oven...");

        ShowPanel(ovenMinigamePanel);
        if (ovenMinigame != null)
            ovenMinigame.StartMinigame();
    }

    // ─── Oven Minigame Complete → Toppings ────────────────────────
    private void OnOvenMinigameComplete(float quality)
    {
        cookQuality = quality;
        HideAllMinigamePanels();
        UpdateStatus("Baked! Now add the toppings.");

        ShowPanel(toppingMinigamePanel);
        if (toppingMinigame != null)
            toppingMinigame.StartMinigame();
    }

    // ─── Topping Minigame Complete → Ready to Serve ───────────────
    private void OnToppingMinigameComplete()
    {
        HideAllMinigamePanels();
        isBaking = false;
        isReady = true;
        UpdateStatus("Cookie ready! Click a customer then hit Serve.");
        SetServeButtonActive(true);
        SetTrashButtonActive(true);
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

        Debug.Log("Serving — dough: " + dough + " topping: " + (toppings.Count > 0 ? toppings[0] : "NONE"));

        bool correct = target.CheckOrder(dough, toppings);
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
        isBaking = false;
        HideAllMinigamePanels();
        SetBakeButtonActive(true);
        SetServeButtonActive(false);
        SetTrashButtonActive(false);
        UpdateStatus("Select a dough and topping, then hit Bake!");
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
        if (doughMinigamePanel != null)   doughMinigamePanel.SetActive(false);
        if (ovenMinigamePanel != null)    ovenMinigamePanel.SetActive(false);
        if (toppingMinigamePanel != null) toppingMinigamePanel.SetActive(false);
    }

    private void SetBakeButtonActive(bool active)
    {
        if (bakeButton != null) bakeButton.interactable = active;
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
    
    private void SetTrashButtonActive(bool active)
    {
        if(trashButton != null) trashButton.interactable = active;
    }
}