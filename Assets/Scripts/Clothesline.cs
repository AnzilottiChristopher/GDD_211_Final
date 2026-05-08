using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the clothesline of order tickets in the kitchen panel.
/// Call AddTicket when a customer spawns, RemoveTicket when they are served or leave.
/// </summary>
public class Clothesline : MonoBehaviour
{
    [SerializeField] private GameObject orderTicketPrefab;
    [SerializeField] private Transform ticketContainer; // the HorizontalLayoutGroup transform

    // Maps customer slot index → ticket so we can find and remove the right one
    private Dictionary<int, OrderTicket> activeTickets = new Dictionary<int, OrderTicket>();

    private static readonly string[] DoughNames = { "Kelp", "Chum", "Coral", "Jelly" };
    private static readonly string[] PossibleToppings = { "Barnacles", "Pearls", "Starfish Sprinkles" };

    /// <summary>
    /// Called by Customer.Init() after the order is generated.
    /// </summary>
    public void AddTicket(int customerSlotIndex, int dough, string topping)
    {
        if (activeTickets.ContainsKey(customerSlotIndex))
        {
            Debug.LogWarning($"[Clothesline] Ticket for slot {customerSlotIndex} already exists.");
            return;
        }

        GameObject ticketObj = Instantiate(orderTicketPrefab, ticketContainer);
        OrderTicket ticket = ticketObj.GetComponent<OrderTicket>();

        if (ticket == null)
        {
            Debug.LogError("[Clothesline] orderTicketPrefab is missing an OrderTicket component!");
            return;
        }

        // Resolve sprites and names from GameManager (same source Customer uses)
        Sprite doughSprite   = GameManager.Instance.GetDoughSprite(dough);
        int toppingIndex     = System.Array.IndexOf(PossibleToppings, topping);
        Sprite toppingSprite = toppingIndex >= 0
            ? CookieBuilder.Instance.GetToppingSprite(toppingIndex)
            : null;

        string doughName   = dough >= 1 && dough <= DoughNames.Length ? DoughNames[dough - 1] : "";
        string toppingName = topping;

        ticket.Setup(customerSlotIndex, doughSprite, toppingSprite, doughName, toppingName);
        activeTickets[customerSlotIndex] = ticket;
    }

    /// <summary>
    /// Called by Customer.Serve() and Customer.OnPatienceExpired().
    /// </summary>
    public void RemoveTicket(int customerSlotIndex)
    {
        if (activeTickets.TryGetValue(customerSlotIndex, out OrderTicket ticket))
        {
            activeTickets.Remove(customerSlotIndex);
            Destroy(ticket.gameObject);
        }
        else
        {
            Debug.LogWarning($"[Clothesline] No ticket found for slot {customerSlotIndex}.");
        }
    }
}