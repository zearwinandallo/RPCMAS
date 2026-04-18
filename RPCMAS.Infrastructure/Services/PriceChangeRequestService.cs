using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Interfaces;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Services
{
    public class PriceChangeRequestService : IPriceChangeRequestService
    {
        private readonly IPriceChangeRequestRepository _priceChangeRequestRepository;
        private readonly IDistributedCache _distributedCache;
        public PriceChangeRequestService(
            IPriceChangeRequestRepository priceChangeRequestRepository,
            IDistributedCache distributedCache)
        {
            _priceChangeRequestRepository = priceChangeRequestRepository;
            _distributedCache = distributedCache;
        }

        public async Task<List<PriceChangeRequestHeaderModel>> GetPriceChangeRequests(PriceChangeRequestFilter filter)
        {
            if (HasFilter(filter))
            {
                return await _priceChangeRequestRepository.GetPriceChangeRequests(filter);
            }

            var cacheValue = await _distributedCache.GetStringAsync("list_request");

            if (!string.IsNullOrEmpty(cacheValue))
            {
                return JsonConvert.DeserializeObject<List<PriceChangeRequestHeaderModel>>(cacheValue) ?? new List<PriceChangeRequestHeaderModel>();
            }
            var requests = await _priceChangeRequestRepository.GetPriceChangeRequests(filter);

            await _distributedCache.SetStringAsync("list_request", JsonConvert.SerializeObject(requests));
            return requests;
        }

        public async Task<PriceChangeRequestHeaderModel?> GetPriceChangeRequestById(Guid id)
        {
            var request = await _priceChangeRequestRepository.GetPriceChangeRequestById(id);

            if (request == null)
            {
                return null;
            }

            request.Details = await _priceChangeRequestRepository.GetDetailsByHeaderId(id);

            return request;
        }

        public async Task<PriceChangeRequestHeaderModel> CreateRequest(PriceChangeRequestHeaderModel request)
        {
            var newRequest = new PriceChangeRequestHeaderModel
            {
                Id = Guid.NewGuid(),
                RequestNumber = GenerateRequestNumber(),
                RequestDate = DateTime.Now,
                Department = request.Department,
                RequestedBy = request.RequestedBy,
                ChangeType = request.ChangeType,
                ReasonOrJustification = request.ReasonOrJustification,
                Status = RequestStatusEnum.Draft
            };

            newRequest.Details = await BuildDetails(request.Details, newRequest.Id);

            await _priceChangeRequestRepository.AddPriceChangeRequest(newRequest);
            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");

            return newRequest;
        }

        public async Task<PriceChangeRequestHeaderModel?> EditDraftRequest(Guid id, PriceChangeRequestHeaderModel request)
        {
            var existingRequest = await _priceChangeRequestRepository.GetPriceChangeRequestHeaderByIdForUpdate(id);

            if (existingRequest == null)
            {
                return null;
            }

            EnsureDraftOnly(existingRequest, "edited");

            var rebuiltDetails = await BuildDetails(request.Details, existingRequest.Id);

            existingRequest.Department = request.Department;
            existingRequest.RequestedBy = request.RequestedBy;
            existingRequest.ChangeType = request.ChangeType;
            existingRequest.ReasonOrJustification = request.ReasonOrJustification;
            await _priceChangeRequestRepository.SaveChanges();
            await _priceChangeRequestRepository.DeletePriceChangeRequestDetailsByHeaderId(existingRequest.Id);
            await _priceChangeRequestRepository.AddPriceChangeRequestDetails(rebuiltDetails);

            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");

            return await _priceChangeRequestRepository.GetPriceChangeRequestById(existingRequest.Id);
        }

        public async Task<PriceChangeRequestHeaderModel?> SubmitRequest(Guid id)
        {
            var request = await _priceChangeRequestRepository.GetPriceChangeRequestByIdForUpdate(id);

            if (request == null)
            {
                return null;
            }

            EnsureDraftOnly(request, "submitted");
            ValidateDetails(request.Details);

            request.Status = RequestStatusEnum.Submitted;
            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");

            return request;
        }

        public async Task<PriceChangeRequestHeaderModel?> ApproveRequest(Guid id)
        {
            var request = await _priceChangeRequestRepository.GetPriceChangeRequestByIdForUpdate(id);

            if (request == null)
            {
                return null;
            }

            EnsureStatus(request, RequestStatusEnum.Submitted, "approved");

            request.Status = RequestStatusEnum.Approved;
            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");

            return request;
        }

        public async Task<PriceChangeRequestHeaderModel?> RejectRequest(Guid id)
        {
            var request = await _priceChangeRequestRepository.GetPriceChangeRequestByIdForUpdate(id);

            if (request == null)
            {
                return null;
            }

            EnsureStatus(request, RequestStatusEnum.Submitted, "rejected");

            request.Status = RequestStatusEnum.Rejected;
            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");

            return request;
        }

        public async Task<PriceChangeRequestHeaderModel?> ApplyRequest(Guid id)
        {
            var request = await _priceChangeRequestRepository.GetPriceChangeRequestByIdForUpdate(id);

            if (request == null)
            {
                return null;
            }

            EnsureStatus(request, RequestStatusEnum.Approved, "applied");

            foreach (var detail in request.Details)
            {
                var item = await _priceChangeRequestRepository.GetItemCatalogBySku(detail.SKU);

                if (item == null)
                {
                    throw new Exception($"Item with SKU '{detail.SKU}' not found.");
                }

                item.CurrentPrice = detail.ProposedNewPrice;
            }

            request.Status = RequestStatusEnum.Applied;
            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");
            await _distributedCache.RemoveAsync("list_itemCatalog");

            return request;
        }

        public async Task<PriceChangeRequestHeaderModel?> CancelRequest(Guid id)
        {
            var request = await _priceChangeRequestRepository.GetPriceChangeRequestByIdForUpdate(id);

            if (request == null)
            {
                return null;
            }

            EnsureDraftOnly(request, "cancelled");

            request.Status = RequestStatusEnum.Cancelled;
            await _priceChangeRequestRepository.SaveChanges();

            await _distributedCache.RemoveAsync("list_request");

            return request;
        }

        private async Task<List<PriceChangeRequestDetailModel>> BuildDetails(List<PriceChangeRequestDetailModel> details, Guid headerId)
        {
            ValidateDetails(details);

            var result = new List<PriceChangeRequestDetailModel>();

            foreach (var detail in details)
            {
                var item = await _priceChangeRequestRepository.GetItemCatalogBySku(detail.SKU);

                if (item == null)
                {
                    throw new Exception($"Item with SKU '{detail.SKU}' not found.");
                }

                if (detail.ProposedNewPrice == item.CurrentPrice)
                {
                    throw new Exception($"Proposed new price for SKU '{detail.SKU}' cannot be equal to current price.");
                }

                result.Add(new PriceChangeRequestDetailModel
                {
                    Id = Guid.NewGuid(),
                    SKU = item.SKU,
                    ItemName = item.ItemName,
                    CurrentPrice = item.CurrentPrice,
                    ProposedNewPrice = detail.ProposedNewPrice,
                    MarkdownPercentage = detail.MarkdownPercentage,
                    EffectiveDate = detail.EffectiveDate,
                    Remarks = detail.Remarks,
                    PriceChangeRequestHeaderId = headerId
                });
            }

            return result;
        }

        private static void ValidateDetails(List<PriceChangeRequestDetailModel> details)
        {
            if (details == null || details.Count == 0)
            {
                throw new Exception("A request must contain at least one item.");
            }

            foreach (var detail in details)
            {
                if (detail.ProposedNewPrice <= 0)
                {
                    throw new Exception($"Proposed new price for SKU '{detail.SKU}' must be greater than zero.");
                }
            }
        }

        private static void EnsureDraftOnly(PriceChangeRequestHeaderModel request, string action)
        {
            EnsureStatus(request, RequestStatusEnum.Draft, action);
        }

        private static void EnsureStatus(PriceChangeRequestHeaderModel request, RequestStatusEnum expectedStatus, string action)
        {
            if (request.Status != expectedStatus)
            {
                throw new Exception($"Only {expectedStatus} requests can be {action}.");
            }
        }

        private static string GenerateRequestNumber()
        {
            return $"PCR-{DateTime.Now:yyyyMMddHHmmssfff}";
        }

        private static bool HasFilter(PriceChangeRequestFilter filter)
        {
            return !string.IsNullOrWhiteSpace(filter.RequestNumber)
                || filter.Status.HasValue
                || !string.IsNullOrWhiteSpace(filter.Department)
                || filter.ChangeType.HasValue
                || filter.RequestDate.HasValue;
        }

       
    }
}
