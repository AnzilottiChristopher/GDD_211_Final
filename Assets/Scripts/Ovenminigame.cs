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
    [SerializeField] private float heatRate = 25f;
    [SerializeField] private float coolRate = 15f;
    [SerializeField] private float greenZoneMin = 40f;
    [SerializeField] private float greenZoneMax = 85f;

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

    void OnEnable()
    {
        if(greenZoneIndicator != null && tempFillBar != null)
        {
            RectTransform barRect = tempFillBar.GetComponent<RectTransform>();
            RectTransform zoneRect = greenZoneIndicator.GetComponent<RectTransform>();
            float barHeight = barRect.rect.height;
            float zoneBottom = (greenZoneMin / maxTemp) * barHeight;
            float zoneTop = (greenZoneMax / maxTemp) * barHeight;

            zoneRect.anchorMin = new Vector2(0, greenZoneMin / maxTemp);
            zoneRect.anchorMax = new Vector2(1, greenZoneMax / maxTemp);
            zoneRect.offsetMin = Vector2.zero;
            zoneRect.offsetMax = Vector2.zero;
        }
    }

    public void StartMinigame()
    {
        currentTemp = startTemp;
        timeRemaining = bakeDuration;
        burnAccumulator = 0f;
        isActive = true;

        if(instructionText != null)
        {
            instructionText.text = "Hold SPACE to keep the oven hot! Stay in the green zone.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive) return;

        if(Keyboard.current.spaceKey.isPressed)
        {
            currentTemp += heatRate * Time.deltaTime;
        }
        else
        {
            currentTemp -= coolRate * Time.deltaTime;
        }

        currentTemp = Mathf.Clamp(currentTemp, 0f, maxTemp);

        bool inGreenZone = currentTemp >= greenZoneMin && currentTemp <= greenZoneMax;
        if (!inGreenZone)
        {
            burnAccumulator += Time.deltaTime;
        }

        UpdateUI(inGreenZone);

        timeRemaining -= Time.deltaTime;
        if(timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isActive = false;
            FinishBake();
        }
    }

    private void UpdateUI(bool inGreenZone)
    {
        if(tempFillBar != null)
        {
            tempFillBar.fillAmount = currentTemp / maxTemp;
        }
        if(timerText != null)
        {
            timerText.text = "Baking: " + Mathf.CeilToInt(timeRemaining) + "s";
        }
        if(statusText != null)
        {
            if(currentTemp < greenZoneMin)
            {
                statusText.text = "Too cold! Hold SPACE!";
            }
            else if(currentTemp > greenZoneMax)
            {
                statusText.text = "Too hot! Release SPACE!";
            }
            else
            {
                statusText.text = "Perfect temperature";
            }
        }
    }

    private void FinishBake()
    {
        float burnRatio = Mathf.Clamp01(burnAccumulator / bakeDuration);
        float quality = 1f - burnRatio;

        if(statusText != null)
        {
            statusText.text = quality > 0.7f ? "Perfectly baked!" : quality > 0.4f ? "A bit burnt..." : "Burnt!";

            OnComplete?.Invoke(quality);
        }
    }
}
