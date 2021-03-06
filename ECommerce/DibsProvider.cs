﻿using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Composite.Data;

using CompositeC1Contrib.ECommerce.Data.Types;

namespace CompositeC1Contrib.ECommerce
{
    public class DibsProvider : PaymentProvider
    {
        private const string StatusOk = "2";
        private const string UniqueOID = "yes";

        protected string Md5Secret { get; private set; }
        protected string Md5Secret2 { get; private set; }

        protected override string PaymentWindowEndpoint
        {
            get { return "https://payment.architrade.com/paymentweb/start.action"; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            Md5Secret = ExtractConfigurationValue(config, "md5Secret", true);
            Md5Secret2 = ExtractConfigurationValue(config, "md5Secret2", true);

            base.Initialize(name, config);
        }

        public override string GeneratePaymentWindow(IShopOrder order, Uri currentUri)
        {
            // http://tech.dibs.dk/integration_methods/flexwin/parameters/

            var currency = ResolveCurrency(order);
            var amount = GetMinorCurrencyUnit(order.OrderTotal, currency).ToString("0", CultureInfo.InvariantCulture);
            var acceptUrl = ParseUrl(ContinueUrl, currentUri);
            var paytype = String.IsNullOrEmpty(PaymentMethods) ? "DK" : PaymentMethods;

            // optional parameters
            var test = IsTestMode ? "yes" : String.Empty;
            var md5Key = CalcMd5Key(MerchantId, order.Id, currency, amount);
            var cancelUrl = ParseUrl(CancelUrl, currentUri);
            var callbackUrl = ParseUrl(CallbackUrl, currentUri);

            var data = new NameValueCollection
            {
                {"merchant", MerchantId},
                {"amount", amount},
                {"accepturl", acceptUrl},
                {"orderid", order.Id},
                {"currency", ((int)currency).ToString()},
                {"uniqueoid", UniqueOID},
                {"test", test},
                {"md5key", md5Key},
                {"lang", Language},
                {"cancelurl", cancelUrl},
                {"callbackurl", callbackUrl},
                {"paytype", paytype}
            };

            return GetFormPost(order, data);
        }

        public override async Task<string> ResolveOrderIdFromRequestAsync(HttpRequestBase request)
        {
            return await Task.Run(() =>
            {
                var form = request.Form;

                return GetFormString("orderid", form);
            });
        }

        public override async Task<IShopOrder> HandleCallbackAsync(HttpContextBase context)
        {
            // http://tech.dibs.dk/integration_methods/flexwin/return_pages/

            var orderid = await ResolveOrderIdFromRequestAsync(context.Request);

            using (var data = new DataConnection())
            {
                var order = data.Get<IShopOrder>().SingleOrDefault(f => f.Id == orderid);
                if (order == null)
                {
                    ECommerceLog.WriteLog("Error, no order with number " + orderid);

                    return null;
                }

                if (order.PaymentStatus == (int)PaymentStatus.Authorized)
                {
                    order.WriteLog("debug", "Payment is already authorized");

                    return order;
                }

                var form = context.Request.Form;

                var statuscode = GetFormString("statuscode", form);
                if (statuscode != StatusOk)
                {
                    order.WriteLog("debug", "Error in status, values is " + statuscode + " but " + StatusOk + " was expected");

                    return order;
                }

                var authkey = GetFormString("authkey", form);
                var transact = GetFormString("transact", form);
                var currency = ResolveCurrency(order);
                var amount = GetMinorCurrencyUnit(order.OrderTotal, currency).ToString("0", CultureInfo.InvariantCulture);

                var isValid = authkey == CalcAuthKey(transact, currency, amount);
                if (!isValid)
                {
                    order.WriteLog("debug", "Error, MD5 Check doesn't match. This may just be an error in the setting or it COULD be a hacker trying to fake a completed order");

                    return order;
                }

                var paymentRequest = data.Get<IPaymentRequest>().Single(r => r.ShopOrderId == order.Id);

                paymentRequest.Accepted = true;
                paymentRequest.AuthorizationData = OrderDataToXml(form);
                paymentRequest.AuthorizationTransactionId = transact;
                paymentRequest.PaymentMethod = GetFormString("paytype", form);

                data.Update(paymentRequest);

                order.PaymentStatus = (int)PaymentStatus.Authorized;

                data.Update(order);

                order.WriteLog("authorized");

                return order;
            }
        }

        private string CalcMd5Key(string merchantId, string orderId, Currency currency, string amount)
        {
            var sb = new StringBuilder();

            using (var md5 = MD5.Create())
            {
                var s = Md5Secret + String.Format("merchant={0}&orderid={1}&currency={2}&amount={3}", merchantId, orderId, (int)currency, amount);
                var bytes = Encoding.ASCII.GetBytes(s);
                var hash = md5.ComputeHash(bytes);

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                s = Md5Secret2 + sb;
                bytes = Encoding.ASCII.GetBytes(s);
                hash = md5.ComputeHash(bytes);

                sb.Length = 0;

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        private string CalcAuthKey(string transact, Currency currency, string amount)
        {
            var sb = new StringBuilder();

            using (var md5 = MD5.Create())
            {
                var s = Md5Secret + String.Format("transact={0}&amount={1}&currency={2}", transact, amount, (int)currency);
                var bytes = Encoding.ASCII.GetBytes(s);
                var hash = md5.ComputeHash(bytes);

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                s = Md5Secret2 + sb;
                bytes = Encoding.ASCII.GetBytes(s);
                hash = md5.ComputeHash(bytes);

                sb.Length = 0;

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();
        }
    }
}
