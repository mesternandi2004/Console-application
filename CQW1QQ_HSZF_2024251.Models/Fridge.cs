using System.Text.Json;

namespace CQW1QQ_HSZF_2024251.Models
{
    public class Fridge : Storage
    {
        public Fridge()
        {
        }

        public Fridge(int capacity) : base(capacity)
        {
        }

        public Fridge(int capacity, ICollection<int> productIds, ICollection<Product> products) : base(capacity, productIds, products)
        {
        }

        #region JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Fridge FromJson(string json)
        {
            return JsonSerializer.Deserialize<Fridge>(json);
        }
        #endregion
    }
}
