using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Data;
using Unity.VisualScripting;

public class CookieBuilder : MonoBehaviour
{
    [Header("Cookie State")]
    [SerializeField] private int dough = -1;
    [SerializeField] private List<string> toppings = new List<string>();

    [Header("Oven")]
    [SerializeField] private float bakeDuration = 8f;
    [SerializeField] private Button bakeButton;
    [SerializeField] private TextMeshProUGUI bakeTimerText;
    [SerializeField] private Button serveButton;

    [Header("FeedBack")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isBaking = false;
    private bool isReady = false;

    void Start()
    {
        SetServeButtonActive(false);
        SetBakeButtonActive(true);
        UpdateStatus("Select a dough and toppings");
    }

    public void SelectDough(int newDough)
    {
        if(isBaking) return;
        dough = newDough;
        // Debug.Log("Selected dough: " + dough);
        UpdateStatus("Dough " + dough + " selected");
    }
    public void AddTopping(string topping)
    {
        if(isBaking) return;
        toppings.Clear();
        toppings.Add(topping);
        // Debug.Log("Added toppings: " + topping);
        UpdateStatus("Topping: " + topping);
    }
    public void ResetCookie()
    {
        if(isBaking) return;
        dough = -1;
        toppings.Clear();
        isReady = false;
        SetServeButtonActive(false);
        SetBakeButtonActive(true);
        UpdateStatus("Cookie reset.");
    }

    public void StartBake()
    {
        Debug.Log("StartBake called — dough: " + dough + " toppings: " + toppings.Count);

        if(isBaking || isReady) return;

        if(dough == -1)
        {
            UpdateStatus("Pick a dough first!");
            return;
        }
        if(toppings.Count == 0)
        {
            UpdateStatus("Add a topping first!");
            return;
        }
        StartCoroutine(BakeRoutine());
    }

    private IEnumerator BakeRoutine()
    {
        isBaking = true;
        SetBakeButtonActive(false);
        float remaining = bakeDuration;

        while(remaining > 0f)
        {
            remaining -= Time.deltaTime;
            if(bakeTimerText != null)
            {
                bakeTimerText.text = "Baking: " + Mathf.CeilToInt(remaining) + "s";
            }
            yield return null;
        }

        isBaking = false;
        isReady = true;

        if(bakeTimerText != null)
        {
            bakeTimerText.text = "";
        }

        UpdateStatus("Cookie ready! Click a customer to serve.");
        SetServeButtonActive(true);
    }

    public void Serve()
    {
        Debug.Log("Serve called, isReady: " + isReady);

        if(!isReady)
        {
            UpdateStatus("Bake the cookie first");
            return;
        }

        Customer target = GameManager.Instance.GetSelectedCustomer();
        Debug.Log("Target customer: " + (target == null ? "NULL" : "slot " + target.SlotIndex));

        if(target == null)
        {
            UpdateStatus("Click a customer first!");
            return;
        }

        bool correct = target.CheckOrder(dough, toppings);
        target.Serve(correct);

        UpdateStatus(correct ? "Perfect order!" : "Wrong order...");

        ResetCookie();
    }

    public int GetDough() => dough;
    public List<string> GetToppings() => toppings;

    private void SetBakeButtonActive(bool active)
    {
        if(bakeButton != null) bakeButton.interactable = active;
    }

    private void SetServeButtonActive(bool active)
    {
        if(serveButton != null) serveButton.interactable = active;
    }

    private void UpdateStatus(string msg)
    {
        Debug.Log("[CookieBuilder] " + msg);
        if(statusText != null) statusText.text = msg;
    }
}
