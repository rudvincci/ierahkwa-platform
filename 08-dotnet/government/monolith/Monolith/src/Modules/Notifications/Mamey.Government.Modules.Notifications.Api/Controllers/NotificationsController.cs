using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.Government.Modules.Notifications.Core.DTO;
using Mamey.Government.Modules.Notifications.Core.Hubs;
using Mamey.Government.Modules.Notifications.Core.Queries;
using Mamey.CQRS.Queries;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Mamey.MicroMonolith.Abstractions.Dispatchers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.Notifications.Api.Controllers;

    // [Authorize(Policy)]
    // [Authorize]
    internal class NotificationsController : BaseController
    {
        private const string Policy = "notifications";
        private readonly IContext _context;
        private readonly IDispatcher _dispatcher;
        private readonly IHubContext<NotificationHub> _hubContext;
        private static readonly List<NotificationDetailsDto> Notifications = new();
        public NotificationsController(IContext context, IDispatcher dispatcher, IHubContext<NotificationHub> hubContext)
        {

            _context = context;
            _dispatcher = dispatcher;
            _hubContext = hubContext;
        }

        // [HttpGet("{userId:guid}")]
        // [SwaggerOperation("Get notification")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // public async Task<ActionResult<NotificationDetailsDto>> GetAsync(Guid userId)
        //     => OkOrNotFound(await _dispatcher.QueryAsync(new GetNotification(userId)));

        [HttpGet]
        [SwaggerOperation("Browse notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedResult<NotificationDto>>> BrowseAsync([FromQuery] ListNotifications query)
            => Ok(await _dispatcher.QueryAsync(query));
        
        [HttpPost]
        [SwaggerOperation("Browse notifications")]
        public async Task<ActionResult> CreateNotification([FromBody] NotificationDetailsDto notification)
        {
            Notifications.Add(notification);

            if (notification.UserId is null)
            {
                // Broadcast to all users
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            }
            else
            {
                // Send to a specific user
                await _hubContext.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification);
            }

            return Ok();
        }

    }

