using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;

using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Raffle
{
    public class RaffleServices : IRaffleServices
    {
        private readonly List<NET10.Core.Interfaces.Raffle> _raffles;
        private readonly List<RaffleTicket> _tickets;
        private readonly List<RaffleDraw> _draws;
        private readonly List<RaffleDrawResult> _results;
        private readonly List<RaffleWinner> _winners;

        public RaffleServices()
        {
            _raffles = new List<NET10.Core.Interfaces.Raffle>();
            _tickets = new List<RaffleTicket>();
            _draws = new List<RaffleDraw>();
            _results = new List<RaffleDrawResult>();
            _winners = new List<RaffleWinner>();

            InitializeRaffles();
        }

        private void InitializeRaffles()
        {
            var now = DateTime.UtcNow;
            _raffles.AddRange(new[]
            {
                new NET10.Core.Interfaces.Raffle
                {
                    Id = "car-tesla",
                    Name = "Tesla Model 3",
                    Description = "Gana un Tesla Model 3 completamente nuevo",
                    Type = RaffleType.SinglePrize,
                    Category = "Car",
                    TicketPrice = 25.00m,
                    TotalTickets = 10000,
                    SoldTickets = 3420,
                    StartDate = now.AddDays(-30),
                    DrawDate = now.AddDays(15),
                    Status = RaffleStatus.Active,
                    Prizes = new List<Prize>
                    {
                        new Prize { Id = "prize-1", Name = "Tesla Model 3", Description = "2024 Tesla Model 3", Value = 45000, Quantity = 1 }
                    },
                    ImageUrl = "https://ierahkwa.gov/raffles/tesla.jpg",
                    TotalValue = 45000
                },
                new NET10.Core.Interfaces.Raffle
                {
                    Id = "house-villa",
                    Name = "Villa en la Playa",
                    Description = "Hermosa villa de 3 habitaciones frente al mar",
                    Type = RaffleType.SinglePrize,
                    Category = "House",
                    TicketPrice = 50.00m,
                    TotalTickets = 5000,
                    SoldTickets = 1890,
                    StartDate = now.AddDays(-20),
                    DrawDate = now.AddDays(30),
                    Status = RaffleStatus.Active,
                    Prizes = new List<Prize>
                    {
                        new Prize { Id = "prize-2", Name = "Villa Playa", Description = "Villa 3BR frente al mar", Value = 250000, Quantity = 1 }
                    },
                    ImageUrl = "https://ierahkwa.gov/raffles/villa.jpg",
                    TotalValue = 250000
                },
                new NET10.Core.Interfaces.Raffle
                {
                    Id = "travel-world",
                    Name = "Viaje alrededor del Mundo",
                    Description = "Viaje de 30 días para 2 personas",
                    Type = RaffleType.SinglePrize,
                    Category = "Travel",
                    TicketPrice = 15.00m,
                    TotalTickets = 8000,
                    SoldTickets = 5230,
                    StartDate = now.AddDays(-10),
                    DrawDate = now.AddDays(5),
                    Status = RaffleStatus.Active,
                    Prizes = new List<Prize>
                    {
                        new Prize { Id = "prize-3", Name = "Viaje Mundial", Description = "30 días, 2 personas", Value = 15000, Quantity = 1 }
                    },
                    ImageUrl = "https://ierahkwa.gov/raffles/travel.jpg",
                    TotalValue = 15000
                },
                new NET10.Core.Interfaces.Raffle
                {
                    Id = "tech-bundle",
                    Name = "Bundle Tecnológico Premium",
                    Description = "iPhone 15 Pro, MacBook Pro, iPad Pro, Apple Watch",
                    Type = RaffleType.MultiPrize,
                    Category = "Tech",
                    TicketPrice = 10.00m,
                    TotalTickets = 12000,
                    SoldTickets = 8765,
                    StartDate = now.AddDays(-5),
                    DrawDate = now.AddDays(2),
                    Status = RaffleStatus.Active,
                    Prizes = new List<Prize>
                    {
                        new Prize { Id = "prize-4-1", Name = "iPhone 15 Pro", Value = 1200, Quantity = 1 },
                        new Prize { Id = "prize-4-2", Name = "MacBook Pro", Value = 2500, Quantity = 1 },
                        new Prize { Id = "prize-4-3", Name = "iPad Pro", Value = 1000, Quantity = 1 },
                        new Prize { Id = "prize-4-4", Name = "Apple Watch", Value = 500, Quantity = 1 }
                    },
                    ImageUrl = "https://ierahkwa.gov/raffles/tech.jpg",
                    TotalValue = 5200
                }
            });
        }

        // Raffles
        public Task<List<NET10.Core.Interfaces.Raffle>> GetRafflesAsync(RaffleStatus? status = null)
        {
            var query = _raffles.AsQueryable();
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            return Task.FromResult(query.OrderBy(r => r.DrawDate).ToList());
        }

        public Task<NET10.Core.Interfaces.Raffle?> GetRaffleByIdAsync(string raffleId) =>
            Task.FromResult(_raffles.FirstOrDefault(r => r.Id == raffleId));

        public Task<List<NET10.Core.Interfaces.Raffle>> GetActiveRafflesAsync() =>
            Task.FromResult(_raffles.Where(r => r.Status == RaffleStatus.Active).ToList());

        public Task<List<NET10.Core.Interfaces.Raffle>> GetUpcomingRafflesAsync() =>
            Task.FromResult(_raffles.Where(r => r.Status == RaffleStatus.Upcoming).ToList());

        // Tickets
        public Task<RaffleTicket> PurchaseTicketAsync(PurchaseRaffleTicketRequest request)
        {
            var raffle = _raffles.FirstOrDefault(r => r.Id == request.RaffleId);
            if (raffle == null)
                throw new ArgumentException("Raffle not found");

            if (raffle.AvailableTickets < request.Quantity)
                throw new InvalidOperationException("Not enough tickets available");

            var tickets = new List<RaffleTicket>();
            var ticketNumberBase = _tickets.Count(t => t.RaffleId == request.RaffleId) + 1;

            for (int i = 0; i < request.Quantity; i++)
            {
                var ticket = new RaffleTicket
                {
                    Id = $"ticket-{Guid.NewGuid()}",
                    UserId = request.UserId,
                    RaffleId = request.RaffleId,
                    RaffleName = raffle.Name,
                    TicketNumber = $"{request.RaffleId}-{ticketNumberBase + i:D6}",
                    Price = raffle.TicketPrice,
                    PurchasedAt = DateTime.UtcNow,
                    Status = RaffleTicketStatus.Active
                };

                _tickets.Add(ticket);
                tickets.Add(ticket);
            }

            raffle.SoldTickets += request.Quantity;
            if (raffle.SoldTickets >= raffle.TotalTickets)
                raffle.Status = RaffleStatus.SoldOut;

            return Task.FromResult(tickets.First());
        }

        public Task<RaffleTicket?> GetTicketByIdAsync(string ticketId) =>
            Task.FromResult(_tickets.FirstOrDefault(t => t.Id == ticketId));

        public Task<List<RaffleTicket>> GetUserTicketsAsync(Guid userId, string? raffleId = null)
        {
            var query = _tickets.Where(t => t.UserId == userId);
            if (!string.IsNullOrEmpty(raffleId))
                query = query.Where(t => t.RaffleId == raffleId);
            return Task.FromResult(query.OrderByDescending(t => t.PurchasedAt).ToList());
        }

        public Task<int> GetTicketCountAsync(string raffleId) =>
            Task.FromResult(_tickets.Count(t => t.RaffleId == raffleId));

        public Task<int> GetUserTicketCountAsync(Guid userId, string raffleId) =>
            Task.FromResult(_tickets.Count(t => t.UserId == userId && t.RaffleId == raffleId));

        // Draws
        public Task<RaffleDraw> CreateDrawAsync(string raffleId)
        {
            var raffle = _raffles.FirstOrDefault(r => r.Id == raffleId);
            if (raffle == null)
                throw new ArgumentException("Raffle not found");

            var draw = new RaffleDraw
            {
                Id = $"draw-{Guid.NewGuid()}",
                RaffleId = raffleId,
                RaffleName = raffle.Name,
                DrawTime = raffle.DrawDate,
                Status = RaffleDrawStatus.Scheduled,
                TotalTickets = raffle.SoldTickets
            };

            _draws.Add(draw);
            return Task.FromResult(draw);
        }

        public Task<RaffleDraw?> GetDrawByIdAsync(string drawId) =>
            Task.FromResult(_draws.FirstOrDefault(d => d.Id == drawId));

        public Task<RaffleDraw?> GetRaffleDrawAsync(string raffleId) =>
            Task.FromResult(_draws.FirstOrDefault(d => d.RaffleId == raffleId));

        public Task<RaffleDrawResult> ExecuteDrawAsync(string raffleId)
        {
            var raffle = _raffles.FirstOrDefault(r => r.Id == raffleId);
            var draw = _draws.FirstOrDefault(d => d.RaffleId == raffleId);
            if (raffle == null)
                throw new ArgumentException("Raffle not found");

            if (draw == null)
                draw = CreateDrawAsync(raffleId).Result;

            var tickets = _tickets.Where(t => t.RaffleId == raffleId && t.Status == RaffleTicketStatus.Active).ToList();
            var random = new Random();
            var winners = new List<RaffleWinner>();

            foreach (var prize in raffle.Prizes)
            {
                for (int i = 0; i < prize.Quantity; i++)
                {
                    if (tickets.Count == 0) break;

                    var winningTicket = tickets[random.Next(tickets.Count)];
                    tickets.Remove(winningTicket);

                    winningTicket.IsWinner = true;
                    winningTicket.PrizeId = prize.Id;
                    winningTicket.PrizeName = prize.Name;
                    winningTicket.PrizeValue = prize.Value;
                    winningTicket.Status = RaffleTicketStatus.Won;

                    var winner = new RaffleWinner
                    {
                        TicketId = winningTicket.Id,
                        TicketNumber = winningTicket.TicketNumber,
                        UserId = winningTicket.UserId,
                        RaffleId = raffleId,
                        RaffleName = raffle.Name,
                        PrizeId = prize.Id,
                        PrizeName = prize.Name,
                        PrizeValue = prize.Value,
                        WonAt = DateTime.UtcNow,
                        IsClaimed = false
                    };

                    winners.Add(winner);
                    _winners.Add(winner);
                }
            }

            draw.Status = RaffleDrawStatus.Completed;
            draw.WinningTicketNumbers = winners.Select(w => w.TicketNumber).ToList();
            draw.PrizeWinners = winners.ToDictionary(w => w.PrizeId, w => w.TicketNumber);

            var result = new RaffleDrawResult
            {
                DrawId = draw.Id,
                RaffleId = raffleId,
                RaffleName = raffle.Name,
                DrawTime = draw.DrawTime,
                Winners = winners,
                IsSettled = true
            };

            _results.Add(result);
            raffle.Status = RaffleStatus.Completed;

            return Task.FromResult(result);
        }

        // Winners
        public Task<List<RaffleWinner>> GetWinnersAsync(string raffleId) =>
            Task.FromResult(_winners.Where(w => w.RaffleId == raffleId).OrderByDescending(w => w.PrizeValue).ToList());

        public Task<List<RaffleWinner>> GetUserWinsAsync(Guid userId) =>
            Task.FromResult(_winners.Where(w => w.UserId == userId).OrderByDescending(w => w.WonAt).ToList());

        public Task<bool> ClaimPrizeAsync(string ticketId, Guid userId)
        {
            var winner = _winners.FirstOrDefault(w => w.TicketId == ticketId && w.UserId == userId);
            if (winner == null || winner.IsClaimed)
                return Task.FromResult(false);

            winner.IsClaimed = true;
            winner.ClaimedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        // Statistics
        public Task<RaffleStatistics> GetStatisticsAsync()
        {
            var stats = new RaffleStatistics
            {
                TotalRaffles = _raffles.Count,
                ActiveRaffles = _raffles.Count(r => r.Status == RaffleStatus.Active),
                CompletedRaffles = _raffles.Count(r => r.Status == RaffleStatus.Completed),
                TotalTicketsSold = _tickets.Count,
                TotalRevenue = _tickets.Sum(t => t.Price),
                TotalPrizesAwarded = _winners.Sum(w => w.PrizeValue),
                TotalWinners = _winners.Count,
                BiggestPrize = _winners.Max(w => (decimal?)w.PrizeValue)
            };

            return Task.FromResult(stats);
        }

        public Task<RaffleStatistics> GetRaffleStatisticsAsync(string raffleId)
        {
            var raffle = _raffles.FirstOrDefault(r => r.Id == raffleId);
            if (raffle == null)
                return Task.FromResult(new RaffleStatistics());

            var raffleTickets = _tickets.Where(t => t.RaffleId == raffleId).ToList();
            var raffleWinners = _winners.Where(w => w.RaffleId == raffleId).ToList();

            var stats = new RaffleStatistics
            {
                TotalRaffles = 1,
                ActiveRaffles = raffle.Status == RaffleStatus.Active ? 1 : 0,
                CompletedRaffles = raffle.Status == RaffleStatus.Completed ? 1 : 0,
                TotalTicketsSold = raffleTickets.Count,
                TotalRevenue = raffleTickets.Sum(t => t.Price),
                TotalPrizesAwarded = raffleWinners.Sum(w => w.PrizeValue),
                TotalWinners = raffleWinners.Count,
                BiggestPrize = raffleWinners.Max(w => (decimal?)w.PrizeValue)
            };

            return Task.FromResult(stats);
        }

        public Task<RaffleStatistics> GetUserStatisticsAsync(Guid userId)
        {
            var userTickets = _tickets.Where(t => t.UserId == userId).ToList();
            var userWins = _winners.Where(w => w.UserId == userId).ToList();

            var stats = new RaffleStatistics
            {
                TotalTicketsSold = userTickets.Count,
                TotalRevenue = userTickets.Sum(t => t.Price),
                TotalPrizesAwarded = userWins.Sum(w => w.PrizeValue),
                TotalWinners = userWins.Count,
                BiggestPrize = userWins.Max(w => (decimal?)w.PrizeValue)
            };

            return Task.FromResult(stats);
        }
    }
}
