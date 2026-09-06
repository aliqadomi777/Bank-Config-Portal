using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using WebPortal.API.Models;
using WebPortal.API.Security;
using WebPortal.Application.DTO.Button;
using WebPortal.Application.DTO.Screen;
using WebPortal.Application.Interfaces;

namespace WebPortal.API.Controllers
{
    [RoutePrefix("api/v1/ticketing-screen-design")]
    public class TicketingScreenDesignController : ApiController
    {
        private readonly ITicketingScreenService _ticketingScreenService;

        public TicketingScreenDesignController(ITicketingScreenService ticketingScreenService)
        {
            _ticketingScreenService = ticketingScreenService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Get([FromUri] int? branchId = null)
        {
            if (!ModelState.IsValid ||
                !branchId.HasValue ||
                branchId.Value <= 0)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    new ApiErrorModel
                    {
                        Code = "INVALID_BRANCH_ID",
                        Message =
                            "Branch ID is required and must be " +
                            "a valid integer greater than zero."
                    });
            }

            int bankId;

            if (!TryGetBankId(out bankId))
            {
                return Content(HttpStatusCode.Unauthorized, new ApiErrorModel
                {
                    Code = "INVALID_AUTHENTICATION",
                    Message = "The authenticated session does not contain valid bank information."
                });
            }

            try
            {
                ScreenResponseDto screen = _ticketingScreenService.GetActiveScreen(branchId ?? 0, bankId);

                if (screen == null)
                {
                    return Content(HttpStatusCode.NotFound, new ApiErrorModel
                    {
                        Code = "TICKETING_SCREEN_NOT_FOUND",
                        Message = "An active branch and active ticketing screen could not be found."
                    });
                }

                return Ok(MapScreen(screen));
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.InternalServerError, new ApiErrorModel
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An unexpected error occurred while retrieving the ticketing screen."
                });
            }
        }

        private bool TryGetBankId(out int bankId)
        {
            bankId = 0;
            var principal = RequestContext.Principal as ClaimsPrincipal;

            if (principal == null ||
                principal.Identity == null ||
                !principal.Identity.IsAuthenticated)
            {
                return false;
            }

            Claim bankIdClaim = principal.FindFirst(AuthenticationConstants.BankIdClaimType);

            return bankIdClaim != null && int.TryParse(bankIdClaim.Value, out bankId) &&
                   bankId > 0;
        }

        private static ScreenResponseModel MapScreen(ScreenResponseDto screen)
        {
            ScreenResponseModel response = new ScreenResponseModel
            {
                ScreenId = screen.ScreenId,
                ScreenName = screen.ScreenName,
                ModifiedAt = screen.ModifiedAt,
                Buttons = new List<BaseButtonResponseModel>(),
                IsActive = screen.IsActive
            };

            foreach (BaseButtonResponseDto button in screen.Buttons)
            {
                TicketButtonResponseDto ticketButton = button as TicketButtonResponseDto;

                if (ticketButton != null && ticketButton.Service != null)
                {
                    TicketButtonResponseModel ticketResponse = new TicketButtonResponseModel
                    {
                        TicketId = ticketButton.TicketId,
                        ServiceId = ticketButton.Service.ServiceId,
                        ServiceNameEN = ticketButton.Service.ServiceNameEN,
                        ServiceNameAR = ticketButton.Service.ServiceNameAR,
                        MaxTicketsPerDay = ticketButton.Service.MaxTicketsPerDay,
                        MinimumServiceTime = ticketButton.Service.MinimumServiceTime,
                        MaximumServiceTime = ticketButton.Service.MaximumServiceTime
                    };

                    MapBaseButton(button, ticketResponse);
                    response.Buttons.Add(ticketResponse);
                    continue;
                }

                MessageButtonResponseDto messageButton = button as MessageButtonResponseDto;

                if (messageButton != null)
                {
                    MessageButtonResponseModel messageResponse = new MessageButtonResponseModel
                    {
                        MessageId = messageButton.MessageId,
                        MessageEN = messageButton.MessageEN,
                        MessageAR = messageButton.MessageAR
                    };

                    MapBaseButton(button, messageResponse);
                    response.Buttons.Add(messageResponse);
                }
            }

            return response;
        }

        private static void MapBaseButton(BaseButtonResponseDto dto, BaseButtonResponseModel model)
        {
            model.ButtonId = dto.ButtonId;
            model.ButtonNameEN = dto.ButtonNameEN;
            model.ButtonNameAR = dto.ButtonNameAR;
            model.ButtonType = dto.ButtonType;
            model.TypeName = dto.TypeName;
            model.ModifiedAt = dto.ModifiedAt;
        }
    }
}