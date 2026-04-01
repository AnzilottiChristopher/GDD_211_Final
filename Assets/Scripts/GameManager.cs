using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private List<Customer> customers = new List<Customer>();

    [Header("Main Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 60f;
    [SerializeField] private bool isCountdown = true;
    private float currentTime;
    private bool isRunning = false;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isRunning = true;
        currentTime = isCountdown ? startTime : 0f;
        UpdateTimerDisplay();
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
        
        //Spawn Customers/Orders
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
    
    
    // public void Serve()
    // {
    //     bool correct = CheckOrder();

    //     if(correct)
    //     {
    //         resultText.text = "Correct";
    //     }
    //     else
    //     {
    //         resultText.text = "Wrong!";
    //     }
    // }
    
    
    private void GenerateCustomer()
    {
        Customer customer = new Customer();

    }
}
