using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBookManager : MonoBehaviour
{
    private OrderBook orderBook;

    void Awake()
    {
        orderBook = new OrderBook();

        // 測試：加入一些訂單
        orderBook.PlaceOrder(new Order(Order.OrderType.Buy, 100f, 10, "Alice"));
        orderBook.PlaceOrder(new Order(Order.OrderType.Sell, 99f, 5, "Bob"));
        orderBook.PlaceOrder(new Order(Order.OrderType.Sell, 98f, 10, "Charlie"));

        orderBook.PrintOrderBook();
        Debug.Log("<color=#ff00ff>OrderBookManager initialized and sample orders placed.</color>");
    }

    // 其他元件可以呼叫這些方法
    public void PlaceBuyOrder(float price, int qty, string playerId)
    {
        orderBook.PlaceOrder(new Order(Order.OrderType.Buy, price, qty, playerId));
    }

    public void PlaceSellOrder(float price, int qty, string playerId)
    {
        orderBook.PlaceOrder(new Order(Order.OrderType.Sell, price, qty, playerId));
    }

    public OrderBook GetOrderBook()
    {
        return orderBook;
    }
}
