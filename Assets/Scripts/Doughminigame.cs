using UnityEngine;
using UnityEngine.UI;
using System;
using System.Numerics;

public class Doughminigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int circlesRequired = 3; 
    [SerializeField] private float angleThreshold = 5f;

    [Header("UI")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TMPro.TextMeshProUGUI instructionText;

    public event Action OnComplete;

    private float totalAngle;
    private float previousAngle;
    private bool isActive = false;
    private UnityEngine.Vector3 screenCenter;

    void OnEnable()
    {
        screenCenter = new UnityEngine.Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    public void StartMinigame()
    {
        totalAngle = 0f;
        isActive = true;
        previousAngle = GetMouseAngle();
        UpdateProgressBar();

        if(instructionText != null)
        {
            instructionText.text = "Spin the dough! Move mouse clockwise in circles";
        }
    }

    void Update()
    {
        if(!isActive) return;

        float currentAngle = GetMouseAngle();
        float delta = Mathf.DeltaAngle(previousAngle, currentAngle);

        if(delta < -angleThreshold)
        {
            totalAngle += Mathf.Abs(delta);
            UpdateProgressBar();
        }

        previousAngle = currentAngle;

        float requiredAngle = circlesRequired * 360f;
        if(totalAngle >= requiredAngle)
        {
            isActive = false;
            if(progressBar != null) progressBar.fillAmount = 1f;
            if(instructionText != null) instructionText.text = "Dough ready!";

            OnComplete?.Invoke();
        }
    }


    private float GetMouseAngle()
    {
        float x = Input.mousePosition.x - screenCenter.x;
        float y = Input.mousePosition.y - screenCenter.y;

        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            float requiredAngle = circlesRequired * 360;
            progressBar.fillAmount = Mathf.Clamp01(totalAngle / requiredAngle);
        }
    }
}
