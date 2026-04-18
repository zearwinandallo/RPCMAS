using Moq;
using Microsoft.Extensions.Caching.Distributed;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Infrastructure.Services;

namespace RPCMAS.Tests.Services;

[TestFixture]
public class ItemCatalogServiceTests
{
    private Mock<IItemCatalogRepository> _repositoryMock = default!;
    private Mock<IDistributedCache> _cacheMock = default!;
    private ItemCatalogService _service = default!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IItemCatalogRepository>();
        _cacheMock = new Mock<IDistributedCache>();
        _service = new ItemCatalogService(_repositoryMock.Object, _cacheMock.Object);
    }

    [Test]
    public async Task GetItemCatalogs_ReturnsItemsFromRepository()
    {
        var items = new List<ItemCatalogModel>
        {
            new() { ItemName = "Test Item" }
        };

        _repositoryMock.Setup(x => x.GetItemCatalogs("Housewares")).ReturnsAsync(items);

        var result = await _service.GetItemCatalogs("Housewares");

        Assert.That(result, Is.SameAs(items));
    }

    [Test]
    public async Task GetItemCatalogs_CallsRepositoryWithSameFilter()
    {
        _repositoryMock.Setup(x => x.GetItemCatalogs(null)).ReturnsAsync(new List<ItemCatalogModel>());

        await _service.GetItemCatalogs(null);

        _repositoryMock.Verify(x => x.GetItemCatalogs(null), Times.Once);
    }

    [Test]
    public async Task GetItemCatalogs_WithFilter_DoesNotReadFromCache()
    {
        _repositoryMock.Setup(x => x.GetItemCatalogs("Shoes")).ReturnsAsync(new List<ItemCatalogModel>());

        await _service.GetItemCatalogs("Shoes");

        Assert.That(_cacheMock.Invocations, Is.Empty);
    }

    [Test]
    public async Task GetItemCatalogById_ReturnsItemFromRepository()
    {
        var itemId = Guid.NewGuid();
        var item = new ItemCatalogModel { Id = itemId, ItemName = "Test Item" };

        _repositoryMock.Setup(x => x.GetItemCatalogById(itemId)).ReturnsAsync(item);

        var result = await _service.GetItemCatalogById(itemId);

        Assert.That(result, Is.SameAs(item));
    }

    [Test]
    public async Task GetItemCatalogById_ReturnsNullWhenRepositoryReturnsNull()
    {
        var itemId = Guid.NewGuid();

        _repositoryMock.Setup(x => x.GetItemCatalogById(itemId)).ReturnsAsync((ItemCatalogModel?)null);

        var result = await _service.GetItemCatalogById(itemId);

        Assert.That(result, Is.Null);
    }
}
