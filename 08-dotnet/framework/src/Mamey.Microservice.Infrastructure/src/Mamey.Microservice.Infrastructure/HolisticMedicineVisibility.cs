using System.Runtime.CompilerServices;

// HolisticMedicine Microservices
// All Infrastructure layers for HolisticMedicine domain services

// Critical Priority Services
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Products.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Inventories.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Patients.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Compliances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Auth.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Securities.Infrastructure")]

// High Priority Services
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Practitioners.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.POS.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Orders.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Deliveries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Payments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.API.Infrastructure")]

// Medium Priority Services
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Analytics.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Marketings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Vendors.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Forecastings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Feedbacks.Infrastructure")]

// Low Priority Services
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Recommendations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Educations.Infrastructure")]

// Supporting Services
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Notifications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Operations.Infrastructure")]

namespace Mamey.Microservice.Infrastructure;




