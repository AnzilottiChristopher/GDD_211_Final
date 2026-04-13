using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.InputSystem;

public class Ovenminigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float bakeDuration = 8f;
    [SerializeField] private float maxTemp = 100f;
    [SerializeField] private float startTemp = 50f;
    [SerializeField] private float heatRate = 35f;
    [SerializeField] private float coolRate = 28;
    [SerializeField] private float greenZoneMin = 40f;
    [SerializeField] private float greenZoneMax = 85f;

    [Header("Green Zone Movement")]
    [SerializeField] private float zoneSpeed = 3f;        // How fast the zone moves
    [SerializeField] private float zoneMoveAmount = 20f;   // How far it travels up/down

    [Header("UI")]
    [SerializeField] private Image tempFillBar;
    [SerializeField] private Image greenZoneIndicator;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI instructionText;

    public event Action<float> OnComplete;

    private float currentTemp;
    private float timeRemaining;
    private float burnAccumulator = 0f;
    private bool isActive = false;

    // Store the original zone values
    private float baseZoneMin;
    private float baseZoneMax;
    private float zoneOffset = 0f;

    void OnEnable()
    {
        UpdateGreenZoneUI();
    }

    public void StartMinigame()
    {
        float progress = GameManager.Instance.GetDayProgress();
        zoneSpeed = Mathf.Lerp(3f, 20f, progress); //! Change values as wanted
        zoneMoveAmount = Mathf.Lerp(10f, 35f, progress); //! Change values as wanted
        heatRate = Mathf.Lerp(35f, 45f, progress); //! Change values as wanted
        coolRate = Mathf.Lerp(28f, 38f, progress); //! Change values as wanted
        currentTemp = startTemp;
        timeRemaining = bakeDuration;
        burnAccumulator = 0f;
        isActive = true;

        baseZoneMin = greenZoneMin;
        baseZoneMax = greenZoneMax;
        zoneOffset = 0f;

        if (instructionText != null)
            instructionText.text = "Hold SPACE to keep the oven hot! Stay in the green zone.";
    }

    void Update()
    {
        if (!isActive) return;

        // Move the green zone using a sine wave
        zoneOffset = Mathf.Sin(Time.time * zoneSpeed) * zoneMoveAmount;

        float currentMin = Mathf.Clamp(baseZoneMin + zoneOffset, 0f, maxTemp);
        float currentMax = Mathf.Clamp(baseZoneMax + zoneOffset, 0f, maxTemp);

        // Heat / cool
        if (Keyboard.current.spaceKey.isPressed)
            currentTemp += heatRate * Time.deltaTime;
        else
            currentTemp -= coolRate * Time.deltaTime;

        currentTemp = Mathf.Clamp(currentTemp, 0f, maxTemp);

        bool inGreenZone = currentTemp >= currentMin && currentTemp <= currentMax;

        if (!inGreenZone)
            burnAccumulator += Time.deltaTime;

        UpdateUI(inGreenZone, currentMin, currentMax);

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isActive = false;
            FinishBake();
        }
    }

    private void UpdateGreenZoneUI()
    {
        UpdateGreenZoneUI(greenZoneMin, greenZoneMax);
    }

    private void UpdateGreenZoneUI(float zoneMin, float zoneMax)
    {
        if (greenZoneIndicator == null || tempFillBar == null) return;

        RectTransform zoneRect = greenZoneIndicator.GetComponent<RectTransform>();
        zoneRect.anchorMin = new Vector2(0, zoneMin / maxTemp);
        zoneRect.anchorMax = new Vector2(1, zoneMax / maxTemp);
        zoneRect.offsetMin = Vector2.zero;
        zoneRect.offsetMax = Vector2.zero;
    }

    private void UpdateUI(bool inGreenZone, float currentMin, float currentMax)
    {
        // Update the green zone indicator position every frame
        UpdateGreenZoneUI(currentMin, currentMax);

        if (tempFillBar != null)
            tempFillBar.fillAmount = currentTemp / maxTemp;

        if (timerText != null)
            timerText.text = "Baking: " + Mathf.CeilToInt(timeRemaining) + "s";

        if (statusText != null)
        {
            if (currentTemp < currentMin)
                statusText.text = "Too cold! Hold SPACE!";
            else if (currentTemp > currentMax)
                statusText.text = "Too hot! Release SPACE!";
            else
                statusText.text = "Perfect temperature";
        }
    }

    private void FinishBake()
    {
        float burnRatio = Mathf.Clamp01(burnAccumulator / bakeDuration);
        float quality = 1f - burnRatio;

        if (statusText != null)
            statusText.text = quality > 0.7f ? "Perfectly baked!" : quality > 0.4f ? "A bit burnt..." : "Burnt!";

        OnComplete?.Invoke(quality);
    }
}