using CQW1QQ_HSZF_2024251.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CQW1QQ_HSZF_2024251.Models
{
    public class Storage : DbEntity
    {
        protected Storage(int capacity, ICollection<int> productIds, ICollection<Product> products)
        {
            Capacity = capacity;
            ProductIds = productIds;
            Products = products;
        }
        protected Storage()
        {
            ProductIds = [];
            Products = [];
        }
        protected Storage(int capacity)
        {
            Capacity = capacity;
            ProductIds = [];
            Products = [];
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Capacity { get; set; }
        public ICollection<int> ProductIds { get; set; }
        [NotMapped]
        public virtual ICollection<Product> Products { get; set; }
    }
}
