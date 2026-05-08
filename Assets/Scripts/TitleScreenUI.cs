using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenUI : MonoBehaviour
{
    [Header("Tutorial Panel")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Tutorial Pages")]
    [SerializeField] private GameObject[] tutorialPages;

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Page Indicator (optional)")]
    [SerializeField] private TextMeshProUGUI pageIndicatorText;

    private int currentPage = 0;

    void Start()
    {
        // Make sure tutorial is hidden on start
        tutorialPanel.SetActive(false);
    }

    // ─── Called by Tutorial Button ────────────────────────────────
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        currentPage = 0;
        ShowPage(currentPage);
    }

    // ─── Called by Close Button ───────────────────────────────────
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    // ─── Called by Next Button ────────────────────────────────────
    public void NextPage()
    {
        if (currentPage >= tutorialPages.Length - 1) return;
        currentPage++;
        ShowPage(currentPage);
    }

    // ─── Called by Previous Button ────────────────────────────────
    public void PreviousPage()
    {
        if (currentPage <= 0) return;
        currentPage--;
        ShowPage(currentPage);
    }

    // ─── Show the correct page and update navigation ──────────────
    private void ShowPage(int index)
    {
        // Hide all pages then show the current one
        for (int i = 0; i < tutorialPages.Length; i++)
            tutorialPages[i].SetActive(i == index);

        // Update nav button visibility
        if (previousButton != null)
            previousButton.gameObject.SetActive(index > 0);

        if (nextButton != null)
            nextButton.gameObject.SetActive(index < tutorialPages.Length - 1);

        // Update page indicator e.g. "1 / 3"
        if (pageIndicatorText != null)
            pageIndicatorText.text = $"{index + 1} / {tutorialPages.Length}";
    }
}