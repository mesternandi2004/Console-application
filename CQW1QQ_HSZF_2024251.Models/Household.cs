using System;

namespace CQW1QQ_HSZF_2024251.Models
{
    public class Household
    {
        public Fridge Fridge { get; set; }
        public Pantry Pantry { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Person> Persons { get; set; }
    }
}
