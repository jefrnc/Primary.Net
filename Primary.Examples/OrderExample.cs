﻿using Primary.Data;
using Primary.Data.Orders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class OrderExample
    {
        private static async Task Main()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            // Get a valid instrument and price
            var instruments = api.GetAllInstruments().Result;
            var instrument = instruments.Last( i => i.Symbol == Tests.Build.DollarFutureSymbol() );

            var today = DateTime.Today;
            var prices = api.GetHistoricalTrades(instrument, today.AddDays(-3), today).Result;

            var order = new Order
            {
                Instrument = instrument,
                Expiration = OrderExpiration.Day,
                Type = OrderType.Limit,
                Price = prices.Last().Price,
                Side = OrderSide.Sell,
                Quantity = 100
            };

            var orderId = await api.SubmitOrder(Api.DemoAccount, order);
            Console.WriteLine($"{orderId.ClientId} / {orderId.Proprietary}");

            var retrievedOrder = await api.GetOrder(orderId);
            Console.WriteLine($"{retrievedOrder.Status} / {retrievedOrder.StatusText}");

            Console.WriteLine($"{retrievedOrder.Quantity} / {retrievedOrder.DisplayQuantity}" +
                $"/ {retrievedOrder.LastQuantity} / {retrievedOrder.CumulativeQuantity} / {retrievedOrder.LeavesQuantity}");
        }
    }
}
