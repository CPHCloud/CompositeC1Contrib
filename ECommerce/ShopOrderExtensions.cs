﻿using System;
using System.Collections.Generic;
using System.Linq;

using Composite;
using Composite.Data;

using CompositeC1Contrib.ECommerce.Configuration;
using CompositeC1Contrib.ECommerce.Data.Types;

namespace CompositeC1Contrib.ECommerce
{
    public static class ShopOrderExtensions
    {
        public static void CreatePaymentRequest(this IShopOrder order)
        {
            var defaultProvider = ECommerceSection.GetSection().DefaultProvider;

            CreatePaymentRequest(order, defaultProvider);
        }

        public static void CreatePaymentRequest(this IShopOrder order, string provider)
        {
            Verify.ArgumentCondition(ECommerce.Providers.ContainsKey(provider), "provider", String.Format("Provider '{0}' doesn't exist", provider));

            using (var data = new DataConnection())
            {
                var request = data.CreateNew<IPaymentRequest>();

                request.ShopOrderId = order.Id;
                request.ProviderName = provider;
                request.Accepted = false;

                data.Add(request);
            }
        }

        public static IShopOrderLog WriteLog(this IShopOrder order, string logTitle)
        {
            return WriteLog(order, logTitle, null);
        }

        public static IShopOrderLog WriteLog(this IShopOrder order, string logTitle, string logData)
        {
            using (var data = new DataConnection())
            {
                var entry = data.CreateNew<IShopOrderLog>();

                entry.Id = Guid.NewGuid();
                entry.ShopOrderId = order.Id;
                entry.Timestamp = DateTime.UtcNow;
                entry.Title = logTitle;
                entry.Data = logData;

                return data.Add(entry);
            }
        }

        public static IEnumerable<IShopOrderLog> GetLog(this IShopOrder order)
        {
            using (var data = new DataConnection())
            {
                return data.Get<IShopOrderLog>().Where(l => l.ShopOrderId == order.Id).ToList();
            }
        }
    }
}
