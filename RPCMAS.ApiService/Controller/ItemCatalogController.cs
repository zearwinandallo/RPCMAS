using Microsoft.AspNetCore.Mvc;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

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
        public async Task<ActionResult<BaseResponseModel>> GetItemCatalogs([FromQuery] string? filter = null)
        {
            var itemCatalogs = new List<ItemCatalogModel>();
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                itemCatalogs = await _itemCatalogService.GetItemCatalogs(filter);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = itemCatalogs });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel>> GetItemCatalogById(Guid id)
        {
            ItemCatalogModel? itemCatalog = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                itemCatalog = await _itemCatalogService.GetItemCatalogById(id);

                if (itemCatalog == null)
                {
                    return NotFound(new BaseResponseModel
                    {
                        IsSuccess = false,
                        ErrorMessage = "Item catalog not found.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = itemCatalog });
        }
    }
}
