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
    
    [Header("Main Timer")]
    [SerializeField] private Text timerText;
    [SerializeField] private float startTime = 60f;
    [SerializeField] private bool isCountdown = true;
    private float currentTime;
    private bool isRunning = false;
    
    private string[] possibleToppings = { "Krill", "Seaweed", "Starfish Sprinkles" };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = isCountdown ? startTime : 0f;
        UpdateTimerDisplay();
        GenerateOrder();
    }

    private void Update() {
        if (!isRunning) return;

        if(isCountdown)
        {
            if(currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                currentTime = 0;
                isRunning = false;
                OnTimerEnd();
            }
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("Time has run out");
        //Game Over logic
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt( currentTime / 60 );
        int seconds = Mathf.FloorToInt( currentTime % 60 );
        int milliseconds = Mathf.FloorToInt((currentTime * 1000) % 1000);

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
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
