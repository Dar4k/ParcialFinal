using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Domain.Services;

using Domain.Entities;

public static class OrderService
{
    private static ImmutableList<Order> _lastOrders = ImmutableList<Order>.Empty;

    public static IReadOnlyList<Order> LastOrders => _lastOrders;

    public static Order CreateTerribleOrder(string customer, string product, int qty, decimal price)
    {
        if (customer == null)
            throw new ArgumentNullException(nameof(customer), "El nombre del cliente no puede ser nulo");
            
        if (product == null)
            throw new ArgumentNullException(nameof(product), "El nombre del producto no puede ser nulo");

        var o = new Order 
        { 
            Id = new Random().Next(1, 9999999), 
            CustomerName = customer, 
            ProductName = product, 
            Quantity = qty, 
            UnitPrice = price 
        };
        
        _lastOrders = _lastOrders.Add(o);
        Infrastructure.Logging.Logger.Log($"Created order {o.Id} for {customer}");
        return o;
    }
}
