namespace IERAHKWA.Platform.Models;

// ============= E-COMMERCE / MARKETPLACE MODELS =============

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SellerId { get; set; } = "";
    public string SellerName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string SubCategory { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; } // For sales
    public string Currency { get; set; } = "USD";
    public int Stock { get; set; }
    public List<string> Images { get; set; } = new();
    public List<ProductVariant> Variants { get; set; } = new();
    public Dictionary<string, string> Specifications { get; set; } = new();
    public decimal Rating { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;
    public int SalesCount { get; set; } = 0;
    public List<string> Tags { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProductVariant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = ""; // e.g., "Size", "Color"
    public string Value { get; set; } = ""; // e.g., "XL", "Blue"
    public decimal? PriceModifier { get; set; }
    public int Stock { get; set; }
    public string? Sku { get; set; }
}

public class ProductReview
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Avatar { get; set; }
    public int Rating { get; set; } // 1-5
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public List<string>? Images { get; set; }
    public int HelpfulCount { get; set; } = 0;
    public bool IsVerifiedPurchase { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// CART
public class Cart
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public List<CartItem> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Shipping { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? CouponCode { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CartItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string? ProductImage { get; set; }
    public string? VariantId { get; set; }
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}

public class AddToCartRequest
{
    public string UserId { get; set; } = "";
    public string ProductId { get; set; } = "";
    public string? VariantId { get; set; }
    public int Quantity { get; set; } = 1;
}

// ORDERS
public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrderNumber { get; set; } = "";
    public string UserId { get; set; } = "";
    public List<OrderItem> Items { get; set; } = new();
    public ShippingAddress ShippingAddress { get; set; } = new();
    public ShippingAddress? BillingAddress { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string PaymentStatus { get; set; } = "pending"; // pending, paid, failed, refunded
    public string OrderStatus { get; set; } = "pending"; // pending, confirmed, processing, shipped, delivered, cancelled
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Shipping { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? TrackingNumber { get; set; }
    public string? TrackingUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string? ProductImage { get; set; }
    public string? VariantId { get; set; }
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
    public string SellerId { get; set; } = "";
}

public class ShippingAddress
{
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string AddressLine1 { get; set; } = "";
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Country { get; set; } = "";
}

public class CreateOrderRequest
{
    public string UserId { get; set; } = "";
    public ShippingAddress ShippingAddress { get; set; } = new();
    public string PaymentMethod { get; set; } = "crypto"; // crypto, card, paypal
    public string? CouponCode { get; set; }
}

// COUPONS
public class ShopCoupon
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";
    public string Type { get; set; } = "percentage"; // percentage, fixed
    public decimal Value { get; set; }
    public decimal? MinimumPurchase { get; set; }
    public decimal? MaximumDiscount { get; set; }
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}

// WISHLIST
public class Wishlist
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public List<WishlistItem> Items { get; set; } = new();
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class WishlistItem
{
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string? ProductImage { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

// SELLER
public class Seller
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string StoreName { get; set; } = "";
    public string Description { get; set; } = "";
    public string? Logo { get; set; }
    public string? Banner { get; set; }
    public decimal Rating { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;
    public int ProductsCount { get; set; } = 0;
    public int SalesCount { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public decimal TotalRevenue { get; set; } = 0;
    public decimal Balance { get; set; } = 0;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

// SHIPPING
public class ShippingOption
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Carrier { get; set; } = "";
    public decimal Price { get; set; }
    public string EstimatedDelivery { get; set; } = "";
    public bool IsExpress { get; set; }
}

// CATEGORIES
public class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public int ProductsCount { get; set; }
    public List<Category> SubCategories { get; set; } = new();
}
