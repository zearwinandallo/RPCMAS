using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

namespace RPCMAS.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PriceChangeRequestController : ControllerBase
    {
        private readonly IPriceChangeRequestService _priceChangeRequestService;

        public PriceChangeRequestController(IPriceChangeRequestService priceChangeRequestService)
        {
            _priceChangeRequestService = priceChangeRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseModel>> GetPriceChangeRequests([FromQuery] PriceChangeRequestFilter filter)
        {
            var requests = new List<PriceChangeRequestHeaderModel>();
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                requests = await _priceChangeRequestService.GetPriceChangeRequests(filter);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = requests });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel>> GetPriceChangeRequestById(Guid id)
        {
            PriceChangeRequestHeaderModel? request = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                request = await _priceChangeRequestService.GetPriceChangeRequestById(id);

                if (request == null)
                {
                    return NotFound(new BaseResponseModel
                    {
                        IsSuccess = false,
                        ErrorMessage = "Price change request not found.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = request });
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseModel>> CreateRequest([FromBody] PriceChangeRequestHeaderModel request)
        {
            PriceChangeRequestHeaderModel? createdRequest = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                createdRequest = await _priceChangeRequestService.CreateRequest(request);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = createdRequest });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BaseResponseModel>> EditDraftRequest(Guid id, [FromBody] PriceChangeRequestHeaderModel request)
        {
            PriceChangeRequestHeaderModel? updatedRequest = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                updatedRequest = await _priceChangeRequestService.EditDraftRequest(id, request);

                if (updatedRequest == null)
                {
                    return NotFound(new BaseResponseModel
                    {
                        IsSuccess = false,
                        ErrorMessage = "Price change request not found.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return Ok(new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = updatedRequest });
        }

        [HttpPost("{id:guid}/submit")]
        public async Task<ActionResult<BaseResponseModel>> SubmitRequest(Guid id)
        {
            return Ok(await ChangeStatus(id, _priceChangeRequestService.SubmitRequest));
        }

        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult<BaseResponseModel>> ApproveRequest(Guid id)
        {
            return Ok(await ChangeStatus(id, _priceChangeRequestService.ApproveRequest));
        }

        [HttpPost("{id:guid}/reject")]
        public async Task<ActionResult<BaseResponseModel>> RejectRequest(Guid id)
        {
            return Ok(await ChangeStatus(id, _priceChangeRequestService.RejectRequest));
        }

        [HttpPost("{id:guid}/apply")]
        public async Task<ActionResult<BaseResponseModel>> ApplyRequest(Guid id)
        {
            return Ok(await ChangeStatus(id, _priceChangeRequestService.ApplyRequest));
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<BaseResponseModel>> CancelRequest(Guid id)
        {
            return Ok(await ChangeStatus(id, _priceChangeRequestService.CancelRequest));
        }

        private static async Task<BaseResponseModel> ChangeStatus(
            Guid id,
            Func<Guid, Task<PriceChangeRequestHeaderModel?>> action)
        {
            PriceChangeRequestHeaderModel? request = null;
            var errorMessage = string.Empty;
            var isSuccess = true;

            try
            {
                request = await action(id);

                if (request == null)
                {
                    return new BaseResponseModel
                    {
                        IsSuccess = false,
                        ErrorMessage = "Price change request not found.",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                isSuccess = false;
            }

            return new BaseResponseModel { IsSuccess = isSuccess, ErrorMessage = errorMessage, Data = request };
        }
    }
}
