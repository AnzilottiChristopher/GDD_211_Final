using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CookieBuilder cookieBuilder;
    [SerializeField] private Text orderText;
    [SerializeField] private Text resultText;

    [Header("Order Data")]
    [SerializeField] private int targetDough;
    [SerializeField] private List<string> targetToppings = new List<string>();
    
    private string[] possibleToppings = { "Krill", "Seaweed", "Starfish Sprinkles" };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateOrder();
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
    
    public void Serve()
    {
        bool correct = CheckOrder();

        if(correct)
        {
            resultText.text = "Correct";
        }
        else
        {
            resultText.text = "Wrong!";
        }
        
        cookieBuilder.ResetCookie();
        GenerateOrder();
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
