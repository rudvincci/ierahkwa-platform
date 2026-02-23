using System.Runtime.CompilerServices;

// RedWebNetwork Microservices
// All Infrastructure layers for RedWebNetwork microservices

// D1-Core-Social
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Users.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Posts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Comments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Reactions.Infrastructure")]

// D2-Communication
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Messages.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Notifications.Infrastructure")]

// D3-Community
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Groups.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Pages.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Events.Infrastructure")]

// D4-Content
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Stories.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Media.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Watch.Infrastructure")]

// D5-Commerce-Gaming
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Marketplace.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Gaming.Infrastructure")]

// D6-Infrastructure
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.ApiGateway.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.RedWebNetwork.Operations.Infrastructure")]

namespace Mamey.Microservice.Infrastructure;





