using App.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using WebPortal.Application.DTO.Button;
using WebPortal.Application.DTO.Screen;
using WebPortal.Application.DTO.Service;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;

namespace WebPortal.Application.Services
{
    public class TicketingScreenService : ITicketingScreenService
    {
        private readonly ITicketingScreenRepository _ticketingScreenRepository;
        private readonly ILogger<ScreenModel> _logger;

        public TicketingScreenService(
            ITicketingScreenRepository ticketingScreenRepository,
            ILogger<ScreenModel> logger)
        {
            _ticketingScreenRepository = ticketingScreenRepository;
            _logger = logger;
        }

        public ScreenResponseDto GetActiveScreen(int branchId, int bankId)
        {
            if (branchId <= 0)
            {
                throw new ArgumentException(
                    "Branch ID must be greater than zero.",
                    nameof(branchId));
            }

            if (bankId <= 0)
            {
                throw new ArgumentException(
                    "Bank ID must be greater than zero.",
                    nameof(bankId));
            }

            try
            {
                ScreenModel screen = _ticketingScreenRepository.GetActiveScreen(branchId, bankId);

                if (screen == null)
                {
                    return null;
                }

                ScreenResponseDto response = new ScreenResponseDto
                {
                    ScreenId = screen.ScreenId,
                    ScreenName = screen.ScreenName,
                    ModifiedAt = screen.ModifiedAt,
                    IsActive = screen.IsActive
                };

                if (screen.Buttons == null)
                {
                    return response;
                }

                foreach (ButtonModel button in screen.Buttons)
                {
                    TicketModel ticketButton = button as TicketModel;

                    if (ticketButton != null && ticketButton.Service != null)
                    {
                        TicketButtonResponseDto ticketResponse = new TicketButtonResponseDto
                        {
                            TicketId = ticketButton.TicketId,
                            Service = new ServiceResponseDto
                            {
                                ServiceId = ticketButton.Service.ServiceId,
                                ServiceNameEN = ticketButton.Service.ServiceNameEN,
                                ServiceNameAR = ticketButton.Service.ServiceNameAR,
                                ServiceStatus = ticketButton.Service.IsActive,
                                ModifiedAt = ticketButton.Service.ModifiedAt,
                                MaxTicketsPerDay = ticketButton.Service.MaxTicketsPerDay,
                                MinimumServiceTime = ticketButton.Service.MinimumServiceTime,
                                MaximumServiceTime = ticketButton.Service.MaximumServiceTime,
                                BankId = ticketButton.Service.BankId
                            }
                        };

                        MapBaseButton(button, ticketResponse);
                        response.Buttons.Add(ticketResponse);
                        continue;
                    }

                    MessageModel messageButton = button as MessageModel;

                    if (messageButton != null)
                    {
                        MessageButtonResponseDto messageResponse = new MessageButtonResponseDto
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
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "SQL error {SqlErrorNumber} while retrieving the ticketing screen for BranchId {BranchId} and BankId {BankId}.",
                    ex.Number,
                    branchId,
                    bankId);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while retrieving the ticketing screen for BranchId {BranchId} and BankId {BankId}.",
                    branchId,
                    bankId);

                throw;
            }
        }

        private static void MapBaseButton(ButtonModel model, BaseButtonResponseDto dto)
        {
            dto.ButtonId = model.ButtonId;
            dto.ButtonNameEN = model.ButtonNameEN;
            dto.ButtonNameAR = model.ButtonNameAR;
            dto.ButtonType = model.ButtonType;
            dto.TypeName = model.TypeName;
            dto.ModifiedAt = model.ModifiedAt;
        }
    }
}