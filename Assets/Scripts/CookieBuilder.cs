using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manages the cookie workstation: ingredient selection, minigame flow, and UI.
/// Cookie data lives on the Cookie component of the instantiated prefab.
/// </summary>
public class CookieBuilder : MonoBehaviour
{
    public static CookieBuilder Instance { get; private set; }

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
    [SerializeField] private Image toppingPreview;
    [SerializeField] private Sprite[] doughSprites;
    [SerializeField] private Sprite[] finishedCookieSprites;
    [SerializeField] private Sprite[] toppingSprites;
    [SerializeField] private Sprite[] toppingPreviewSprites;

    [Header("Cookie Prefab")]
    [SerializeField] private GameObject cookiePrefab;
    // Where the cookie spawns — assign ingredientPanel or a dedicated anchor
    [SerializeField] private Transform cookieSpawnParent;

    private static readonly string[] PossibleToppings = { "Barnacles", "Pearls", "Starfish Sprinkles" };
    private static readonly string[] DoughNames = { "Kelp", "Chum", "Coral", "Jelly" };
    public Sprite GetDoughSprite(int dough) => doughSprites[dough - 1];
    public Sprite GetToppingSprite(int index) => toppingPreviewSprites[index];

    // ─── Workstation State ────────────────────────────────────────
    private int dough = -1;
    private List<string> toppings = new List<string>();
    private float cookQuality = 1f;
    private bool isBaking = false;

    void Awake()
    {
        Instance = this;
    }

    // ─── Lifecycle ────────────────────────────────────────────────
    void Start()
    {
        SetTrashButtonActive(false);
        SetBakeButtonActive(true);
        HideAllMinigamePanels();
        ingredientPanel.SetActive(true);
        doughDisplay.gameObject.SetActive(false);
        toppingPreview.gameObject.SetActive(false);
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
        if (isBaking) return;
        dough = newDough;
        string doughName = DoughNames[newDough - 1];
        Debug.Log("Selected dough: " + doughName);
        UpdateStatus(doughName + " dough selected. Now pick a topping!");

        doughDisplay.sprite = doughSprites[newDough - 1];
        doughDisplay.gameObject.SetActive(true);
    }

    // ─── Step 2: Topping Selection ────────────────────────────────
    public void AddTopping(string topping)
    {
        if (isBaking) return;
        toppings.Clear();
        toppings.Add(topping);
        Debug.Log("Added topping: " + topping);
        UpdateStatus("Topping: " + topping + ". Ready to bake!");

        int toppingIndex = System.Array.IndexOf(PossibleToppings, topping);
        if (toppingIndex >= 0)
        {
            toppingPreview.sprite = toppingPreviewSprites[toppingIndex];
            toppingPreview.gameObject.SetActive(true);
        }
    }

    // ─── Step 3: Bake ─────────────────────────────────────────────
    public void StartBake()
    {
        if (isBaking) return;

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

    // ─── Topping Minigame Complete → Spawn Cookie Prefab ──────────
    private void OnToppingMinigameComplete()
    {
        HideAllMinigamePanels();

        // Resolve sprites
        Sprite cookieSprite = finishedCookieSprites[dough - 1];
        Sprite toppingSprite = null;
        int toppingIndex = System.Array.IndexOf(PossibleToppings, toppings[0]);
        if (toppingIndex >= 0)
            toppingSprite = toppingSprites[toppingIndex];

        // Instantiate and configure the cookie
        GameObject newCookieObj = Instantiate(cookiePrefab, cookieSpawnParent);
        Cookie newCookie = newCookieObj.GetComponent<Cookie>();
        if (newCookie != null)
            newCookie.Configure(dough, toppings, cookQuality, cookieSprite, toppingSprite);
        else
            Debug.LogError("[CookieBuilder] cookiePrefab is missing a Cookie component!");

        // If DragController isn't already set on the prefab, inject it now
        Item itemComponent = newCookieObj.GetComponent<Item>();
        if (itemComponent != null)
            itemComponent.Init(FindObjectOfType<DragController>());

        // Workstation is immediately free for the next cookie
        ResetWorkstation();
        UpdateStatus("Cookie ready! Drag it to your inventory.");
    }

    // ─── Reset workstation state and UI ───────────────────────────
    private void ResetWorkstation()
    {
        dough = -1;
        toppings.Clear();
        cookQuality = 1f;
        isBaking = false;
        HideAllMinigamePanels();

        SetBakeButtonActive(true);
        SetTrashButtonActive(false);
        doughDisplay.sprite = null;
        doughDisplay.gameObject.SetActive(false);
        toppingPreview.sprite = null;
        toppingPreview.gameObject.SetActive(false);
        UpdateStatus("Select a dough and topping, then hit Bake!");
    }

    // ─── Helpers ──────────────────────────────────────────────────
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