using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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

    public void Init(int slotIndex)
    {
        SlotIndex = slotIndex;
        patience = maxPatience;

        clickButton = GetComponentInChildren<Button>();
        Debug.Log("Button Found" + (clickButton != null));
        if(clickButton != null)
        {
            clickButton.onClick.AddListener(() => GameManager.Instance.SelectCustomer(this));
        }
        GenerateOrder();
        patienceCoroutine = GameManager.Instance.RunCoroutine(PatienceRoutine());
    }
    private IEnumerator PatienceRoutine()
    {
        while(patience > 0f && !isServed)
        {
            yield return null;
            patience -= Time.deltaTime;
            UpdatePatienceUI();
        }

        if(!isServed)
        {
            patience = 0f;
            OnPatienceExpired();
        }
    }
    
    private void OnTimerEnd()
    {
        Debug.Log("Customer is leaving");
        
        //Add more game logic
    }
    
    private void UpdatePatienceUI()
    {
        if(patienceText != null)
        {
            int seconds = Mathf.CeilToInt(patience);
            patienceText.text = seconds.ToString() + "s";
        }
    }
    private void GenerateOrder()
    {
        targetToppings.Clear();

        targetDough = Random.Range(1, 5);
        targetToppings.Add(PossibleToppings[Random.Range(0, PossibleToppings.Length)]);

        UpdateOrderUI();
    }

    private void UpdateOrderUI()
    {
        if(orderText != null)
            orderText.text = "Dough: " + targetDough + "\nTopping: " + targetToppings[0];
    }

    public bool CheckOrder(int servedDough, List<string> servedToppings)
    {
        if(servedDough != targetDough) return false;
        if(servedToppings == null || servedToppings.Count == 0) return false;
        if(!targetToppings.Contains(servedToppings[0])) return false;
        return true;
    }

    public void Serve(bool correct)
    {
        if(isServed) return;
        isServed = true;

        if(patienceCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(patienceCoroutine);
        }
        if(correct)
        {
            int points = 10 + Mathf.FloorToInt(patience);
            GameManager.Instance.AddScore(points);
            Debug.Log("Correct order! + " + points + " points");
        }
        else
        {
            Debug.Log("Wrong order - customer unhappy");
        }

        GameManager.Instance.ReleaseSlot(SlotIndex);
    }

    private void OnPatienceExpired()
    {
        if(isServed) return;
        isServed = true;

        if(patienceCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(patienceCoroutine);
        }
        Debug.Log("Customer left - patience ran out");
        GameManager.Instance.ReleaseSlot(SlotIndex);
    }
}
