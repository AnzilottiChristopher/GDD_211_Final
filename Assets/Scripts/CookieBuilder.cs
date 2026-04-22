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
    [SerializeField] private Button trashButton;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Build Panel")]
    [SerializeField] private GameObject ingredientPanel;
    [SerializeField] private Image doughDisplay;
    [SerializeField] private TextMeshProUGUI toppingLabel;
    [SerializeField] private GameObject cookieItem;

    private bool isReady = false;
    private bool isBaking = false;
    private static readonly string[] DoughNames = { "Kelp", "Chum", "Coral", "Jelly" };
    public float GetCookQuality() => cookQuality;

    void Start()
    {
        SetTrashButtonActive(false);
        SetBakeButtonActive(true);
        HideAllMinigamePanels();
        ingredientPanel.SetActive(true);
        doughDisplay.gameObject.SetActive(false);
        toppingLabel.gameObject.SetActive(false);
        cookieItem.SetActive(false);
        UpdateStatus("Select a dough and topping, then hit Bake!");

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

    // ─── Step 1: Dough Selection ───────────────────────────────────
    public void SelectDough(int newDough, Button doughButton)
    {
        if (isBaking || isReady) return;
        dough = newDough;
        string doughName = DoughNames[newDough - 1];
        Debug.Log("Selected dough: " + doughName);
        UpdateStatus(doughName + " dough selected. Now pick a topping!");

        // show dough image in build panel
        doughDisplay.sprite = doughButton.targetGraphic.GetComponent<Image>().sprite;
        doughDisplay.gameObject.SetActive(true);
    }

    // ─── Step 2: Topping Selection ────────────────────────────────
    public void AddTopping(string topping)
    {
        if (isBaking || isReady) return;
        toppings.Clear();
        toppings.Add(topping);
        Debug.Log("Added topping: " + topping);
        UpdateStatus("Topping: " + topping + ". Ready to bake!");
        toppingLabel.text = topping;
        toppingLabel.gameObject.SetActive(true);
    }

    // ─── Step 3: Bake ─────────────────────────────────────────────
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

    // ─── Topping Minigame Complete → Cookie Ready ─────────────────
    private void OnToppingMinigameComplete()
    {
        HideAllMinigamePanels();
        isBaking = false;
        isReady = true;

        // TODO Make it so it's the image and don't change the children
        doughDisplay.transform.SetParent(cookieItem.transform, false);
        toppingLabel.transform.SetParent(cookieItem.transform, false);

        //doughDisplay.gameObject.SetActive(false);
        //toppingLabel.gameObject.SetActive(false);
        cookieItem.SetActive(true);
        SetTrashButtonActive(true);
        UpdateStatus("Cookie ready! Drag it to your inventory.");
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

        //TODO remove when have actual asset
        doughDisplay.transform.SetParent(ingredientPanel.transform, false);
        toppingLabel.transform.SetParent(ingredientPanel.transform, false);

        SetBakeButtonActive(true);
        SetTrashButtonActive(false);
        cookieItem.SetActive(false);
        doughDisplay.sprite = null;
        doughDisplay.gameObject.SetActive(false);
        toppingLabel.text = "";
        toppingLabel.gameObject.SetActive(false);
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

    private void SetTrashButtonActive(bool active)
    {
        if (trashButton != null) trashButton.interactable = active;
    }

    private void UpdateStatus(string msg)
    {
        Debug.Log("[CookieBuilder] " + msg);
        if (statusText != null) statusText.text = msg;
    }
}