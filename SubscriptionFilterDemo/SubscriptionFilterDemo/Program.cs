using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionDemo
{
    class Program
    {
        private const string RuleName = "ajfilter";

        public static void Main(string[] args)
        {
            MainAsync().Wait();

        }

        static async Task MainAsync()
        {
            string sbconn = "Endpoint=sb://bigfoot.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=TKPypdXL9BCiiX8C7xGKFOkNGZLA0CZYESJtkSL3qag=";

            SubscriptionClient s = new SubscriptionClient(sbconn, "demo", "aj");
            try
            {
                await s.RemoveRuleAsync(RuleDescription.DefaultRuleName);
            }
            catch (Exception) { }
            try
            {
                await s.RemoveRuleAsync(RuleName);
            }
            catch (Exception) { }

            await s.AddRuleAsync(new RuleDescription
            {
                Filter = new SqlFilter("sys.To = 'Store2'"),
                Action = new SqlRuleAction("SET sys.Label='aj'"),
                Name = RuleName
            });

            // check if filter is being created
            var data = await s.GetRulesAsync();
            foreach(var d in data)
            {
                Console.WriteLine(d.Name.ToString());
            }
            Console.ReadKey();

            var entityPath = EntityNameHelper.FormatSubscriptionPath("demo", "aj");
            var receiver = new MessageReceiver(sbconn, entityPath, ReceiveMode.PeekLock, RetryPolicy.Default, 100);

            while (true)
            {
                IList<Message> messages = await receiver.ReceiveAsync(10, TimeSpan.FromSeconds(2));

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        lock (Console.Out)
                        {
                            Item item = message.As<Item>();
                            IDictionary<string, object> myUserProperties = message.UserProperties;
                            Console.WriteLine($"StoreId={myUserProperties["StoreId"]}");

                            if (message.Label != null)
                            {
                                Console.WriteLine($"Label={message.Label}");
                            }

                            Console.WriteLine(
                                $"Item data: Price={item.getPrice()}, Color={item.getColor()}, Category={item.getItemCategory()}");
                        }

                        await receiver.CompleteAsync(message.SystemProperties.LockToken);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }


    class Item
    {
        public string theColor;
        public double thePrice;
        public string ItemCategory;

        public Item()
        {
        }

        public Item(int color, int price, int ItmCat)
        {
            this.setColor(color);
            this.setPrice(price);
            this.setItemCategory(ItmCat);
        }

        public string getColor()
        {
            return theColor;
        }

        public double getPrice()
        {
            return thePrice;
        }

        public string getItemCategory()
        {
            return ItemCategory;
        }

        public void setColor(int color)
        {
            string[] Color = { "Red", "Green", "Blue", "Orange", "Yellow" };
            this.theColor = Color[color];
        }

        public void setPrice(int price)
        {
            double[] Price = { 1.4, 2.3, 3.2, 4.1, 5.1 };
            this.thePrice = Price[price];
        }

        public void setItemCategory(int ItmCat)
        {
            string[] CategoryList = { "Vegetables", "Beverage", "Meat", "Bread", "Other" };
            this.ItemCategory = CategoryList[ItmCat];
        }
    }


    public static class Extensions
    {
        public static T As<T>(this Message message) where T : class
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));
        }
        public static Message AsMessage(this object obj)
        {
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static bool Any(this IList<Message> collection)
        {
            return collection != null && collection.Count > 0;
        }
    }
}
