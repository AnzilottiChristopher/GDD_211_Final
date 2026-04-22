using UnityEngine;
using UnityEngine.UI;

public class DoughButton : MonoBehaviour
{
    [SerializeField] private CookieBuilder cookieBuilder;
    [SerializeField] private int doughIndex;

    public void OnClick()
    {
        cookieBuilder.SelectDough(doughIndex, GetComponent<Button>());
    }
}