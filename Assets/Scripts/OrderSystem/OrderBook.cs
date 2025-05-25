using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Order
{
    public enum OrderType { Buy, Sell }

    public OrderType Type;
    public float Price;
    public int Quantity;
    public string PlayerId;
    public DateTime Timestamp;

    public Order(OrderType type, float price, int quantity, string playerId)
    {
        Type = type;
        Price = price;
        Quantity = quantity;
        PlayerId = playerId;
        Timestamp = DateTime.UtcNow;
    }
}

public class Trade
{
    public string BuyerId;
    public string SellerId;
    public float Price;
    public int Quantity;
    public DateTime Timestamp;

    public Trade(string buyerId, string sellerId, float price, int quantity)
    {
        BuyerId = buyerId;
        SellerId = sellerId;
        Price = price;
        Quantity = quantity;
        Timestamp = DateTime.UtcNow;
    }
}

public class OrderBook
{
    private List<Order> buyOrders = new();
    private List<Order> sellOrders = new();
    private List<Trade> tradeHistory = new();

    public void PlaceOrder(Order order)
    {
        if (order.Type == Order.OrderType.Buy)
            buyOrders.Add(order);
        else
            sellOrders.Add(order);

        MatchOrders();
    }

    private void MatchOrders()
    {
        // 排序買單（價格高 → 低，時間早 → 晚）
        buyOrders = buyOrders.OrderByDescending(o => o.Price).ThenBy(o => o.Timestamp).ToList();
        // 排序賣單（價格低 → 高，時間早 → 晚）
        sellOrders = sellOrders.OrderBy(o => o.Price).ThenBy(o => o.Timestamp).ToList();

        while (buyOrders.Count > 0 && sellOrders.Count > 0)
        {
            var buy = buyOrders[0];
            var sell = sellOrders[0];

            if (buy.Price >= sell.Price)
            {
                int tradedQuantity = Math.Min(buy.Quantity, sell.Quantity);
                float tradePrice = sell.Price; // 你也可以用 (buy.Price + sell.Price) / 2f

                Trade trade = new Trade(
                    buyerId: buy.PlayerId,
                    sellerId: sell.PlayerId,
                    price: tradePrice,
                    quantity: tradedQuantity
                );

                tradeHistory.Add(trade);
                Debug.Log($"<color=#ff00ff>[TRADE] <color=#00ff00>{tradedQuantity}</color> units @ <color=#00ff00>{tradePrice}</color> between <color=#00ff00>{buy.PlayerId}</color> and <color=#00ff00>{sell.PlayerId}</color></color>");
                buy.Quantity -= tradedQuantity;
                sell.Quantity -= tradedQuantity;

                if (buy.Quantity == 0) buyOrders.RemoveAt(0);
                if (sell.Quantity == 0) sellOrders.RemoveAt(0);
            }
            else
            {
                break; // 沒有可以成交的訂單了
            }
        }
    }

    public List<Order> GetTopBids(int depth = 5)
    {
        return buyOrders
            .OrderByDescending(o => o.Price)
            .ThenBy(o => o.Timestamp)
            .Take(depth)
            .ToList();
    }

    public List<Order> GetTopAsks(int depth = 5)
    {
        return sellOrders
            .OrderBy(o => o.Price)
            .ThenBy(o => o.Timestamp)
            .Take(depth)
            .ToList();
    }

    public List<Trade> GetTradeHistory(int limit = 50)
    {
        return tradeHistory
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToList();
    }

    public void PrintOrderBook()
    {
        Debug.Log("----- ORDER BOOK -----");
        Debug.Log(" BIDS:");
        foreach (var bid in GetTopBids())
            Debug.Log($"<color=#ff00ff>  [B] <color=#00ff00>{bid.Quantity}</color> @ <color=#00ff00>{bid.Price}</color></color>");

        Debug.Log(" ASKS:");
        foreach (var ask in GetTopAsks())
            Debug.Log($"<color=#ff00ff>  [S] <color=#00ff00>{ask.Quantity}</color> @ <color=#00ff00>{ask.Price}</color></color>");
    }
}

public class Program
{
    public static void Main()
    {
        var orderBook = new OrderBook();

        // 模擬一些訂單
        orderBook.PlaceOrder(new Order(Order.OrderType.Buy, 100f, 10, "Alice"));
        orderBook.PlaceOrder(new Order(Order.OrderType.Sell, 99f, 5, "Bob"));
        orderBook.PlaceOrder(new Order(Order.OrderType.Sell, 98f, 10, "Charlie"));

        orderBook.PrintOrderBook();
    }
}