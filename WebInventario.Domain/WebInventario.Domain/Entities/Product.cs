using WebInventario.Domain.Common;

namespace WebInventario.Domain.Entities
{
    public sealed class Product : EntityBase
    {
        public string Name { get; private set; } = string.Empty;
        public Guid CategoryId { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        // EF Core necesita constructor vacío
        private Product() { }

        public Product(string name, Guid categoryId, decimal price, int stock)
        {
            SetName(name);
            SetCategory(categoryId);
            SetPrice(price);
            SetStock(stock);
        }

        public void Update(string name, Guid categoryId, decimal price, int stock)
        {
            SetName(name);
            SetCategory(categoryId);
            SetPrice(price);
            SetStock(stock);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            Name = name.Trim();
        }

        private void SetCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("CategoryId is required.", nameof(categoryId));

            CategoryId = categoryId;
        }

        private void SetPrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

            Price = price;
        }

        private void SetStock(int stock)
        {
            if (stock < 0)
                throw new ArgumentOutOfRangeException(nameof(stock), "El Stock no puede ser negativo.");

            Stock = stock;
        }
    }
}
