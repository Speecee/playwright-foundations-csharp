using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CarvedRock.End2End
{
    internal class Product
    {
        public Product() { }

        public Product(string name, decimal price, string description, string category, string image) {
        
        Name = name;
        Price = price;
        Description = description;
        Category = category;
        ImgUrl = image;
        
        }

        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? ImgUrl { get; set; }
    }
}
