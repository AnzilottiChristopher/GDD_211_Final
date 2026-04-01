using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    [Header("Customer Patience Levels")]
    [SerializeField] private TextMeshProUGUI patienceText;
    [SerializeField] private float patience = 20f;
    
    [Header("References")]
    [SerializeField] private CookieBuilder cookieBuilder;
    [SerializeField] private TextMeshProUGUI orderText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Order Data")]
    [SerializeField] private int targetDough;
    [SerializeField] private List<string> targetToppings = new List<string>();

    private string[] possibleToppings = { "Krill", "Seaweed", "Starfish Sprinkles" };
    private void Update() {
        if(patience > 0)
        {
            patience -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            patience = 0;
            OnTimerEnd();
        }
    }
    
    private void OnTimerEnd()
    {
        Debug.Log("Customer is leaving");
        
        //Add more game logic
    }
    
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt( patience / 60 );
        int seconds = Mathf.FloorToInt( patience % 60 );
        int milliseconds = Mathf.FloorToInt((patience * 1000) % 1000);

        patienceText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    private void GenerateOrder()
    {
        targetToppings.Clear();

        targetDough = Random.Range(1, 5);
        targetToppings.Add(possibleToppings[Random.Range(0, possibleToppings.Length)]);

        UpdateOrderUI();
    }

    private void UpdateOrderUI()
    {
        orderText.text = "Dough: " + targetDough + "\nTopping: " + targetToppings[0];
    }
    private bool CheckOrder()
    {
        if(cookieBuilder.GetDough() != targetDough) return false;

        List<string> playerToppings = cookieBuilder.GetToppings();
        
        if(playerToppings.Count == 0)
        {
            return false;
        }
        if(!targetToppings.Contains(playerToppings[0]))
        {
            return false;
        }
        
        return true;
    }
}
