using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CQW1QQ_HSZF_2024251.Models
{
    public class Person : DbEntity
    {
        public Person()
        {
            FavoriteProductIds = [];
            FavoriteProducts = [];

        }

        public Person(string name, ICollection<int> favoriteProductIds, bool responsibleForPurchase, ICollection<Product> favoriteProducts)
        {
            Name = name;
            FavoriteProductIds = favoriteProductIds;
            ResponsibleForPurchase = responsibleForPurchase;
            FavoriteProducts = favoriteProducts;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<int> FavoriteProductIds { get; set; }
        public bool ResponsibleForPurchase { get; set; }
        [NotMapped]
        public virtual ICollection<Product> FavoriteProducts { get; set; }

        #region JSON
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Person FromJson(string json)
        {
            return JsonSerializer.Deserialize<Person>(json);
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
