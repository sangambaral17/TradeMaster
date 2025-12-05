using FluentAssertions;
using TradeMaster.Core.Entities;
using TradeMaster.Infrastructure.Data;
using TradeMaster.Tests.Helpers;
using Xunit;

namespace TradeMaster.Tests.Integration;

public class ProductManagementTests : IDisposable
{
    private readonly TradeMasterDbContext _context;
    private readonly EfRepository<Product> _productRepository;

    public ProductManagementTests()
    {
        _context = TestDatabaseFactory.CreateContextWithSeedData();
        _productRepository = new EfRepository<Product>(_context);
    }

    public void Dispose()
    {
        TestDatabaseFactory.Cleanup(_context);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task AddProduct_ShouldAddProductToDatabase()
    {
        // Arrange
        var newProduct = new Product
        {
            Name = "New Test Product",
            Sku = "NEW001",
            CategoryId = 1,
            Price = 150.00m,
            Currency = "NPR",
            StockQuantity = 25
        };

        // Act
        await _productRepository.AddAsync(newProduct);
        var allProducts = await _productRepository.GetAllAsync();

        // Assert
        allProducts.Should().Contain(p => p.Name == "New Test Product");
        allProducts.Count().Should().Be(4); // 3 seed + 1 new
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        // Act
        var products = await _productRepository.GetAllAsync();

        // Assert
        products.Should().NotBeNull();
        products.Count().Should().Be(3); // Seed data has 3 products
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetProductById_ShouldReturnCorrectProduct()
    {
        // Act
        var product = await _productRepository.GetByIdAsync(1);

        // Assert
        product.Should().NotBeNull();
        product!.Name.Should().Be("Test Product 1");
        product.Price.Should().Be(100.00m);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateProduct_ShouldUpdateProductInDatabase()
    {
        // Arrange
        var product = await _productRepository.GetByIdAsync(1);
        product!.Price = 125.00m;
        product.StockQuantity = 60;

        // Act
        await _productRepository.UpdateAsync(product);
        var updatedProduct = await _productRepository.GetByIdAsync(1);

        // Assert
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Price.Should().Be(125.00m);
        updatedProduct.StockQuantity.Should().Be(60);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteProduct_ShouldRemoveProductFromDatabase()
    {
        // Arrange
        var product = await _productRepository.GetByIdAsync(1);

        // Act
        await _productRepository.DeleteAsync(product!);
        var allProducts = await _productRepository.GetAllAsync();

        // Assert
        allProducts.Should().NotContain(p => p.Id == 1);
        allProducts.Count().Should().Be(2); // 3 seed - 1 deleted
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SearchProducts_ByName_ShouldReturnMatchingProducts()
    {
        // Act
        var allProducts = await _productRepository.GetAllAsync();
        var searchResults = allProducts.Where(p => 
            p.Name.Contains("Product 1", StringComparison.OrdinalIgnoreCase));

        // Assert
        searchResults.Should().HaveCount(1);
        searchResults.First().Name.Should().Be("Test Product 1");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SearchProducts_BySku_ShouldReturnMatchingProducts()
    {
        // Act
        var allProducts = await _productRepository.GetAllAsync();
        var searchResults = allProducts.Where(p => 
            p.Sku != null && p.Sku.Contains("TEST002", StringComparison.OrdinalIgnoreCase));

        // Assert
        searchResults.Should().HaveCount(1);
        searchResults.First().Sku.Should().Be("TEST002");
    }
}
