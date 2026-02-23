// namespace Mamey.ApplicationName.Modules.Saga.Api.Sagas.ProductManagement;

// public class CreateProductSaga : ISaga, 
//     ISagaStartAction<CreateProduct>, 
//     ISagaAction<ProductCreatedEvent>, 
//     ISagaAction<CreateProductRejectedEvent>
// {
//     private readonly IProductService _productService;

//     public CreateProductSaga(IProductService productService)
//     {
//         _productService = productService;
//     }

//     public async Task HandleAsync(CreateProduct command, ISagaContext context)
//     {
//         try
//         {
//             var productId = await _productService.CreateProductAsync(command);
//             await Task.CompletedTask; // Additional business logic can be inserted here.
//         }
//         catch (Exception ex)
//         {
//             throw new SagaActionRejectedException("Failed to create product", ex);
//         }
//     }

//     public Task CompensateAsync(CreateProduct command, ISagaContext context)
//     {
//         // Logic to compensate the transaction in case of failure.
//         return Task.CompletedTask;
//     }

//     public Task HandleAsync(ProductCreatedEvent @event, ISagaContext context)
//     {
//         // Logic to finalize saga
//         return Task.CompletedTask;
//     }

//     public Task HandleAsync(CreateProductRejected @event, ISagaContext context)
//     {
//         // Logic for handling rejection
//         return Task.CompletedTask;
//     }
// }
