using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Customer Spawning")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform[] customerSlots;
    [SerializeField] private float spawnInterfalMin = 8f;
    [SerializeField] private float spawnInterfalMax = 20f;
    private Queue<int> pendingSpawns = new Queue<int>();
    [SerializeField] private GameObject customerPanel;
    //[SerializeField] private Transform canvasParent;

    private Customer[] slotOccupants;

    private Customer selectedCustomer = null;

    [Header("Day Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float dayDuration = 120;
    private float currentTime;
    private bool isRunning = false;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;
    private int score = 0;


    void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slotOccupants = new Customer[customerSlots.Length];
        currentTime = dayDuration;
        isRunning = true;
        UpdateScoreUI();
        StartCoroutine(SpawnRoutine());
    }

    private void Update() {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        if(currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnDayEnd();
        }
        UpdateTimerUI();
    }

    private void OnDayEnd()
    {
        Debug.Log("Time has run out");
        Debug.Log("FINAL SCORE: " + score);
        //Game Over logic
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt( currentTime / 60 );
        int seconds = Mathf.FloorToInt( currentTime % 60 );
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(2f);

        while(isRunning)
        {
            float wait = Random.Range(spawnInterfalMin, spawnInterfalMax);
            yield return new WaitForSeconds(wait);

            if(isRunning) TrySpawnCustomer();
        }
    }

    private void TrySpawnCustomer()
    {
        for(int i = 0; i < slotOccupants.Length; i++)
        {
            if(slotOccupants[i] == null)
            {
                spawnOnAlive(i);
                return;
            }
        }
    }
    private void spawnOnAlive(int slotIndex)
    {
        if(!customerPanel.activeSelf)
        {
            pendingSpawns.Enqueue(slotIndex);
            return;
        }
        SpawnCustomerAtSlot(slotIndex);
    }
    public void onCustomerPanelEnable()
    {
        while(pendingSpawns.Count > 0)
        {
            int slotIndex = pendingSpawns.Dequeue();
            SpawnCustomerAtSlot(slotIndex);
        }
    }
    private void SpawnCustomerAtSlot(int slotIndex)
    {
        GameObject customerPanel = GameObject.Find("Customer Panel");
        Transform panelTransform = customerPanel.transform;

        GameObject go = Instantiate(customerPrefab, panelTransform);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = customerSlots[slotIndex].GetComponent<RectTransform>().anchoredPosition;
        //rt.localScale = Vector3.one;
        Customer c = go.GetComponentInChildren<Customer>();
        if(c == null)
        {
            Debug.LogError("No Customer Component found");
            return;
        }
        c.Init(slotIndex);
        slotOccupants[slotIndex] = c;
    }

    public void ReleaseSlot(int slotIndex)
    {
        if(slotOccupants[slotIndex] != null)
        {
            if(selectedCustomer == slotOccupants[slotIndex])
            {
                selectedCustomer = null;
}
            Destroy(slotOccupants[slotIndex].gameObject);
            slotOccupants[slotIndex] = null;
        }
    }

    public void SelectCustomer(Customer customer)
    {
        selectedCustomer = customer;
        Debug.Log("Selected Customer in slot " + customer.SlotIndex);
    }

    public Customer GetSelectedCustomer() => selectedCustomer;

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if(scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public Coroutine RunCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
    
    public float GetDayProgress()
    {
        return 1f - (currentTime - dayDuration);
    }
}
