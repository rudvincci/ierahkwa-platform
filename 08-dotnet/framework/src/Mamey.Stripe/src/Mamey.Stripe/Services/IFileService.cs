//using Mamey.Stripe.Models;
//using System.IO;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IFileService
//    {
//        /// <summary>
//        /// Uploads a file to Stripe, typically used for evidence in dispute resolution or verification processes.
//        /// </summary>
//        /// <param name="fileStream">The stream of the file to upload.</param>
//        /// <param name="fileName">The name of the file.</param>
//        /// <param name="purpose">The purpose of the file upload, e.g., 'dispute_evidence' or 'identity_document'.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the upload operation.</param>
//        /// <returns>The created File object containing details of the uploaded file.</returns>
//        Task<File> UploadAsync(Stream fileStream, string fileName, string purpose, string idempotencyKey = null);
//    }
//}
//public class FileService : IFileService
//    {
//        private readonly StripeApiClient _stripeClient;
//        private readonly ILogger<FileService> _logger;

//        public FileService(StripeApiClient stripeClient, ILogger<FileService> logger)
//        {
//            _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<File> UploadAsync(Stream fileStream, string fileName, string purpose, string idempotencyKey = null)
//        {
//            try
//            {
//                // Assuming _stripeClient has a method for uploading files to Stripe
//                if (fileStream == null || fileStream.Length == 0)
//                {
//                    throw new ArgumentException("File stream is empty", nameof(fileStream));
//                }

//                if (string.IsNullOrEmpty(fileName))
//                {
//                    throw new ArgumentException("File name must be provided", nameof(fileName));
//                }

//                if (string.IsNullOrEmpty(purpose))
//                {
//                    throw new ArgumentException("Purpose must be provided", nameof(purpose));
//                }

//                return await _stripeClient.UploadFileAsync(fileStream, fileName, purpose, idempotencyKey);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to upload file {FileName} for purpose {Purpose}", fileName, purpose);
//                throw;
//            }
//        }
//    }