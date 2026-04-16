using Microsoft.AspNetCore.Mvc;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;

namespace RPCMAS.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCatalogController : ControllerBase
    {
        private readonly IItemCatalogService _itemCatalogService;
        public ItemCatalogController(IItemCatalogService itemCatalogService)
        {
            _itemCatalogService = itemCatalogService;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseModel>> GetItemCatalogs()
        {
            var itemCatalogs = new List<ItemCatalogModel>();
            var errorMessage = string.Empty;

            try
            {
                itemCatalogs = await _itemCatalogService.GetItemCatalogs();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return Ok(new BaseResponseModel { IsSuccess = true, ErrorMessage = errorMessage, Data = itemCatalogs });
        }

    }
}
