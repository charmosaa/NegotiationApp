namespace NegotiationApp.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal BasePrice { get; private set; }

        private Product() { }

        public Product(string name, decimal basePrice)
        {
            Id = Guid.NewGuid();
            Name = name;
            BasePrice = basePrice;
        }

        public void UpdateProduct(string name, decimal basePrice)
        {
            Name = name;
            BasePrice = basePrice;
        }
    }
}