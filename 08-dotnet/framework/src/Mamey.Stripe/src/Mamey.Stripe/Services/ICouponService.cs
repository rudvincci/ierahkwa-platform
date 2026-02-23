//using Mamey.Stripe.Models;
//using Mamey.Stripe.Services;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface ICouponService
//    {
//        /// <summary>
//        /// Creates a new coupon that can be applied to subscriptions or charges.
//        /// </summary>
//        /// <param name="request">Parameters for the coupon creation, including discount type, amount, duration, and eligibility criteria.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created Coupon object.</returns>
//        Task<Coupon> CreateAsync(CouponRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing coupon by its ID.
//        /// </summary>
//        /// <param name="couponId">The unique identifier of the coupon to retrieve.</param>
//        /// <returns>The requested Coupon object.</returns>
//        Task<Coupon> RetrieveAsync(string couponId);

//        /// <summary>
//        /// Updates an existing coupon, typically to modify its eligibility criteria or expiration date.
//        /// </summary>
//        /// <param name="couponId">The ID of the coupon to update.</param>
//        /// <param name="updateRequest">Updated information for the coupon.</param>
//        /// <returns>The updated Coupon object.</returns>
//        Task<Coupon> UpdateAsync(string couponId, CouponUpdateRequest updateRequest);

//        /// <summary>
//        /// Deletes a coupon, making it inactive and unavailable for future use.
//        /// </summary>
//        /// <param name="couponId">The ID of the coupon to delete.</param>
//        /// <returns>A boolean indicating the success of the deletion.</returns>
//        Task<bool> DeleteAsync(string couponId);

//        /// <summary>
//        /// Lists all coupons, optionally filtered by parameters such as active status or creation date.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of coupons.</param>
//        /// <returns>A list of Coupon objects.</returns>
//        Task<IEnumerable<Coupon>> ListAsync(CouponListRequest request);
//    }
//}
//public class CouponService : ICouponService
//    {
//        private readonly ILogger<CouponService> _logger;

//        public CouponService(ILogger<CouponService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        //public async Task<Coupon> CreateAsync(CouponRequest request, string idempotencyKey = null)
//        //{
//        //    try
//        //    {
//        //        var options = new CouponCreateOptions
//        //        {
//        //            Duration = request.Duration,
//        //            AmountOff = request.AmountOff,
//        //            PercentOff = request.PercentOff,
//        //            DurationInMonths = request.DurationInMonths,
//        //            MaxRedemptions = request.MaxRedemptions,
//        //            // Additional options based on CouponRequest
//        //        };
//        //        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//        //        var service = new CouponService();
//        //        var coupon = await service.CreateAsync(options, requestOptions);
//        //        return new Coupon { /* Map Stripe Coupon to your Coupon model */ };
//        //    }
//        //    catch (StripeException ex)
//        //    {
//        //        _logger.LogError(ex, "Error creating coupon with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//        //        throw new ApplicationException("Failed to create coupon.", ex);
//        //    }
//        //}

//    public Task<bool> DeleteAsync(string couponId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<IEnumerable<Coupon>> ListAsync(CouponListRequest request)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Coupon> RetrieveAsync(string couponId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Coupon> UpdateAsync(string couponId, CouponUpdateRequest updateRequest)
//    {
//        throw new NotImplementedException();
//    }

//    // Implement other methods similarly, focusing on securely managing coupon details and handling exceptions

//}