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

        [HttpPost]
        public async Task<ActionResult<BaseResponseModel>> CreateItemCatalog([FromBody] ItemCatalogModel itemCatalog)
        {
            ItemCatalogModel? createdItemCatalog = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                createdItemCatalog = await _itemCatalogService.CreateItemCatalog(itemCatalog);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = createdItemCatalog });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel>> UpdateItemCatalog(Guid id, [FromBody] ItemCatalogModel itemCatalog)
        {
            ItemCatalogModel? updatedItemCatalog = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                itemCatalog.Id = id;
                updatedItemCatalog = await _itemCatalogService.UpdateItemCatalog(itemCatalog);

                if (updatedItemCatalog == null)
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

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = updatedItemCatalog });
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel>> DeleteItemCatalog(Guid id)
        {
            var isDeleted = false;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                isDeleted = await _itemCatalogService.DeleteItemCatalog(id);

                if (!isDeleted)
                {
                    return NotFound(new BaseResponseModel
                    {
                        IsSuccess = false,
                        ErrorMessage = "Item catalog not found.",
                        Data = false
                    });
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = isDeleted });
        }
    }
}
