using CQW1QQ_HSZF_2024251.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CQW1QQ_HSZF_2024251.Models
{
    public class Product(int id, string name, double quantity, double criticalLevel, DateTime bestBefore, bool storeInFridge)
    {


        [Key]
        public int Id { get; set; } = id;
        public string Name { get; set; } = name;
        public double Quantity { get; set; } = quantity;
        public double CriticalLevel { get; set; } = criticalLevel;
        public DateTime BestBefore { get; set; } = bestBefore;
        public bool StoreInFridge { get; set; } = storeInFridge;
        [NotMapped]
        public virtual Storage? Storage { get; set; }

        #region JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Product FromJson(string json)
        {
            return JsonSerializer.Deserialize<Product>(json);
        }

        #endregion



    }

}
