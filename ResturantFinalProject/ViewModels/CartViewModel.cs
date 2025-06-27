namespace ResturantFinalProject.ViewModels
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    }
     
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
    public class CartAndOrdersViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public OrderViewModel ActiveOrder { get; set; }
        public decimal CartTotal => CartItems.Sum(i => i.Price * i.Quantity);
    }

   
}