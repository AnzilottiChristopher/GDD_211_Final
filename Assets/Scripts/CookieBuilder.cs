using UnityEngine;
using System.Collections.Generic;

public class CookieBuilder : MonoBehaviour
{
    [SerializeField] private int dough = -1;
    [SerializeField] private List<string> toppings = new List<string>();
    
    public void SelectDough(int newDough)
    {
        dough = newDough;
        Debug.Log("Selected dough: " + dough);
    }
    public void AddTopping(string topping)
    {
        toppings.Add(topping);
        Debug.Log("Added topping: " + topping);
    }
    public void ResetCookie()
    {
        dough = -1;
        toppings.Clear();
    }
    
    public int GetDough()
    {
        return dough;
    }
    public List<string> GetToppings()
    {
        return toppings;
    }
}
