using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Toppingminigame : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int sequenceLength = 4;
    [SerializeField] private int baseSequenceLength = 3;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI sequenceText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    public event Action OnComplete;

    private static readonly KeyCode[] PossibleKeys =
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K
    };

    private List<KeyCode> sequence = new List<KeyCode>();
    private int currentIndex = 0;
    private bool isActive = false;

    public void StartMinigame()
    {
        float difficulty = GameManager.Instance.GetDifficultyMultiplier();
        sequenceLength = Mathf.RoundToInt(baseSequenceLength * difficulty);

        GenerateSequence();
        currentIndex = 0;
        isActive = true;

        UpdateSequenceUI();
        UpdateProgressUI();

        if(instructionText != null)
        {
            instructionText.text = "Press the keys in order!";
        }
        if(feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive) return;

        foreach(KeyCode key in PossibleKeys)
        {
            Key inputKey = ConvertToInputSystemKey(key);
            if(Keyboard.current[inputKey].wasPressedThisFrame)
            {
                HandleKeyPress(key);
                break;
            }
        }
    }
    private Key ConvertToInputSystemKey(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.A: return Key.A;
            case KeyCode.S: return Key.S;
            case KeyCode.D: return Key.D;
            case KeyCode.F: return Key.F;
            case KeyCode.G: return Key.G;
            case KeyCode.H: return Key.H;
            case KeyCode.J: return Key.J;
            case KeyCode.K: return Key.K;
            default: return Key.None;
        }
    }

    private void GenerateSequence()
    {
        sequence.Clear();
        for(int i = 0; i < sequenceLength; i++)
        {
            sequence.Add(PossibleKeys[UnityEngine.Random.Range(0, PossibleKeys.Length)]);
        }
    }
    private void UpdateSequenceUI()
    {
        if (sequenceText == null) return;
        
        string display = "";
        for (int i = 0; i < sequence.Count; i++)
        {
            display += sequence[i].ToString();
            if (i < sequence.Count - 1) display += " - ";
        }
        sequenceText.text = display;
    }
   private void UpdateProgressUI()
    {
        if (progressText == null) return;

        string display = "";
        for (int i = 0; i < sequence.Count; i++)
        {
            if (i < currentIndex)
            {
                display += "<color=green>" + sequence[i].ToString() + "</color>";
                //Debug.Log("Key " + i + " should be green");
            }
            else
                display += sequence[i].ToString();

            if (i < sequence.Count - 1) display += " - ";
        }

        //Debug.Log("Setting progressText to: " + display);
        progressText.text = display;
        //progressText.text = "<color=green>TEST</color>";

    }
    private void HandleKeyPress(KeyCode key)
    {
        if(key == sequence[currentIndex])
        {
            currentIndex++;
            UpdateProgressUI();

            if(feedbackText != null)
            {
                feedbackText.text = "";
            }
            if(currentIndex >= sequence.Count)
            {
                isActive = false;
                if(feedbackText != null)
                {
                    feedbackText.text = "Perfect!";
                }
                OnComplete?.Invoke();
            }
        }
        else
        {
            currentIndex = 0;
            UpdateProgressUI();

            if(feedbackText != null)
            {
                feedbackText.text = "Wrong! Start over";
            }
        }
    }
}
