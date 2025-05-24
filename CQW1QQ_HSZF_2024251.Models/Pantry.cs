using System.Text.Json;

namespace CQW1QQ_HSZF_2024251.Models
{
    public class Pantry : Storage
    {
        public Pantry()
        {
        }

        public Pantry(int capacity) : base(capacity)
        {
        }

        public Pantry(int capacity, ICollection<int> productIds, ICollection<Product> products) : base(capacity, productIds, products)
        {
        }
        #region JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Pantry FromJson(string json)
        {
            return JsonSerializer.Deserialize<Pantry>(json);
        }
        #endregion
    }
}
