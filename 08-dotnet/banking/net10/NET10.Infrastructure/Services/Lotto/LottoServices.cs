using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Lotto
{
    public class LottoServices : ILottoServices
    {
        private readonly List<LottoGame> _games;
        private readonly List<LottoTicket> _tickets;
        private readonly List<LottoDraw> _draws;
        private readonly List<DrawResult> _results;
        private readonly List<LottoWinner> _winners;

        public LottoServices()
        {
            _games = new List<LottoGame>();
            _tickets = new List<LottoTicket>();
            _draws = new List<LottoDraw>();
            _results = new List<DrawResult>();
            _winners = new List<LottoWinner>();

            InitializeGames();
        }

        private void InitializeGames()
        {
            var now = DateTime.UtcNow;
            _games.AddRange(new[]
            {
                new LottoGame
                {
                    Id = "classic",
                    Name = "Lotto Clásico",
                    Type = "Classic",
                    Description = "Lotería tradicional con sorteos diarios",
                    TicketPrice = 2.00m,
                    NumberCount = 6,
                    MaxNumber = 49,
                    BonusNumberCount = 1,
                    NextDrawTime = now.AddDays(1).Date.AddHours(20),
                    CurrentJackpot = 5000000,
                    IsActive = true,
                    PrizeTiers = new Dictionary<string, decimal>
                    {
                        { "6+Bonus", 5000000 },
                        { "6", 100000 },
                        { "5+Bonus", 10000 },
                        { "5", 1000 },
                        { "4+Bonus", 500 },
                        { "4", 50 },
                        { "3+Bonus", 20 },
                        { "3", 5 }
                    }
                },
                new LottoGame
                {
                    Id = "express",
                    Name = "Lotto Express",
                    Type = "Express",
                    Description = "Sorteos cada 4 horas",
                    TicketPrice = 1.00m,
                    NumberCount = 5,
                    MaxNumber = 35,
                    BonusNumberCount = 0,
                    NextDrawTime = now.AddHours(4),
                    CurrentJackpot = 250000,
                    IsActive = true,
                    PrizeTiers = new Dictionary<string, decimal>
                    {
                        { "5", 250000 },
                        { "4", 5000 },
                        { "3", 100 },
                        { "2", 10 }
                    }
                },
                new LottoGame
                {
                    Id = "mega",
                    Name = "Mega Lotto",
                    Type = "Mega",
                    Description = "El premio más grande",
                    TicketPrice = 5.00m,
                    NumberCount = 7,
                    MaxNumber = 50,
                    BonusNumberCount = 2,
                    NextDrawTime = now.AddDays(7).Date.AddHours(21),
                    CurrentJackpot = 50000000,
                    IsActive = true,
                    PrizeTiers = new Dictionary<string, decimal>
                    {
                        { "7+2Bonus", 50000000 },
                        { "7+1Bonus", 1000000 },
                        { "7", 50000 },
                        { "6+2Bonus", 10000 },
                        { "6+1Bonus", 1000 },
                        { "6", 500 },
                        { "5+2Bonus", 100 },
                        { "5+1Bonus", 50 },
                        { "5", 25 }
                    }
                }
            });
        }

        // Lotto Games
        public Task<List<LottoGame>> GetGamesAsync() => Task.FromResult(_games);

        public Task<LottoGame?> GetGameByIdAsync(string gameId) =>
            Task.FromResult(_games.FirstOrDefault(g => g.Id == gameId));

        public Task<List<LottoGame>> GetActiveGamesAsync() =>
            Task.FromResult(_games.Where(g => g.IsActive).ToList());

        // Tickets
        public Task<LottoTicket> PurchaseTicketAsync(PurchaseTicketRequest request)
        {
            var game = _games.FirstOrDefault(g => g.Id == request.GameId);
            if (game == null)
                throw new ArgumentException("Game not found");

            var tickets = new List<LottoTicket>();
            var random = new Random();

            for (int i = 0; i < request.Quantity; i++)
            {
                var numbers = request.QuickPick
                    ? GenerateRandomNumbers(game.NumberCount, game.MaxNumber, random)
                    : request.Numbers;

                var bonusNumbers = request.QuickPick && game.BonusNumberCount > 0
                    ? GenerateRandomNumbers(game.BonusNumberCount, game.MaxNumber, random)
                    : request.BonusNumbers;

                var ticket = new LottoTicket
                {
                    Id = $"ticket-{Guid.NewGuid()}",
                    UserId = request.UserId,
                    GameId = request.GameId,
                    GameName = game.Name,
                    Numbers = numbers,
                    BonusNumbers = bonusNumbers,
                    Price = game.TicketPrice,
                    PurchasedAt = DateTime.UtcNow,
                    Status = LottoTicketStatus.Active
                };

                _tickets.Add(ticket);
                tickets.Add(ticket);
            }

            return Task.FromResult(tickets.First());
        }

        private List<int> GenerateRandomNumbers(int count, int max, Random random)
        {
            var numbers = new HashSet<int>();
            while (numbers.Count < count)
            {
                numbers.Add(random.Next(1, max + 1));
            }
            return numbers.OrderBy(n => n).ToList();
        }

        public Task<LottoTicket?> GetTicketByIdAsync(string ticketId) =>
            Task.FromResult(_tickets.FirstOrDefault(t => t.Id == ticketId));

        public Task<List<LottoTicket>> GetUserTicketsAsync(Guid userId, string? gameId = null)
        {
            var query = _tickets.Where(t => t.UserId == userId);
            if (!string.IsNullOrEmpty(gameId))
                query = query.Where(t => t.GameId == gameId);
            return Task.FromResult(query.OrderByDescending(t => t.PurchasedAt).ToList());
        }

        public Task<List<LottoTicket>> GetWinningTicketsAsync(string drawId) =>
            Task.FromResult(_tickets.Where(t => t.DrawId == drawId && t.Status == LottoTicketStatus.Won).ToList());

        // Draws
        public Task<List<LottoDraw>> GetDrawsAsync(string? gameId = null, int limit = 50)
        {
            var query = _draws.AsQueryable();
            if (!string.IsNullOrEmpty(gameId))
                query = query.Where(d => d.GameId == gameId);
            return Task.FromResult(query.OrderByDescending(d => d.DrawTime).Take(limit).ToList());
        }

        public Task<LottoDraw?> GetDrawByIdAsync(string drawId) =>
            Task.FromResult(_draws.FirstOrDefault(d => d.Id == drawId));

        public Task<LottoDraw?> GetNextDrawAsync(string gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game == null || !game.NextDrawTime.HasValue)
                return Task.FromResult<LottoDraw?>(null);

            var nextDraw = _draws.FirstOrDefault(d => d.GameId == gameId && d.Status == LottoDrawStatus.Scheduled);
            if (nextDraw == null)
            {
                nextDraw = new LottoDraw
                {
                    Id = $"draw-{Guid.NewGuid()}",
                    GameId = gameId,
                    GameName = game.Name,
                    DrawTime = game.NextDrawTime.Value,
                    Status = LottoDrawStatus.Scheduled,
                    TotalTickets = _tickets.Count(t => t.GameId == gameId && string.IsNullOrEmpty(t.DrawId)),
                    TotalPrizePool = game.CurrentJackpot ?? 0,
                    Jackpot = game.CurrentJackpot
                };
                _draws.Add(nextDraw);
            }

            return Task.FromResult<LottoDraw?>(nextDraw);
        }

        public Task<LottoDraw?> GetLastDrawAsync(string gameId) =>
            Task.FromResult(_draws.Where(d => d.GameId == gameId && d.Status == LottoDrawStatus.Completed)
                .OrderByDescending(d => d.DrawTime).FirstOrDefault());

        public Task<LottoDraw> CreateDrawAsync(string gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game == null)
                throw new ArgumentException("Game not found");

            var draw = new LottoDraw
            {
                Id = $"draw-{Guid.NewGuid()}",
                GameId = gameId,
                GameName = game.Name,
                DrawTime = DateTime.UtcNow,
                Status = LottoDrawStatus.Scheduled,
                TotalTickets = _tickets.Count(t => t.GameId == gameId && string.IsNullOrEmpty(t.DrawId)),
                TotalPrizePool = game.CurrentJackpot ?? 0,
                Jackpot = game.CurrentJackpot
            };

            _draws.Add(draw);
            return Task.FromResult(draw);
        }

        // Results
        public Task<DrawResult?> GetDrawResultAsync(string drawId)
        {
            var draw = _draws.FirstOrDefault(d => d.Id == drawId);
            if (draw == null)
                return Task.FromResult<DrawResult?>(null);

            var result = _results.FirstOrDefault(r => r.DrawId == drawId);
            if (result == null && draw.Status == LottoDrawStatus.Completed)
            {
                result = new DrawResult
                {
                    DrawId = drawId,
                    GameId = draw.GameId,
                    GameName = draw.GameName,
                    DrawTime = draw.DrawTime,
                    WinningNumbers = draw.WinningNumbers ?? new List<int>(),
                    BonusNumbers = draw.BonusNumbers ?? new List<int>(),
                    WinnersByTier = new Dictionary<string, int>(),
                    PrizeByTier = new Dictionary<string, decimal>(),
                    TotalPrizePool = draw.TotalPrizePool,
                    IsSettled = true
                };
                _results.Add(result);
            }

            return Task.FromResult(result);
        }

        public Task<List<DrawResult>> GetRecentResultsAsync(int limit = 10) =>
            Task.FromResult(_results.OrderByDescending(r => r.DrawTime).Take(limit).ToList());

        public Task<bool> CheckTicketWinAsync(string ticketId, string drawId)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == ticketId);
            var draw = _draws.FirstOrDefault(d => d.Id == drawId);
            if (ticket == null || draw == null || draw.WinningNumbers == null)
                return Task.FromResult(false);

            // Simplified win check
            var matches = ticket.Numbers.Count(n => draw.WinningNumbers.Contains(n));
            var bonusMatches = ticket.BonusNumbers?.Count(b => draw.BonusNumbers?.Contains(b) ?? false) ?? 0;

            var won = matches >= 3;
            if (won)
            {
                ticket.Status = LottoTicketStatus.Won;
                ticket.DrawId = drawId;
            }

            return Task.FromResult(won);
        }

        // Winners
        public Task<List<LottoWinner>> GetWinnersAsync(string? drawId = null, int limit = 50)
        {
            var query = _winners.AsQueryable();
            if (!string.IsNullOrEmpty(drawId))
                query = query.Where(w => w.DrawId == drawId);
            return Task.FromResult(query.OrderByDescending(w => w.WinAmount).Take(limit).ToList());
        }

        public Task<List<LottoWinner>> GetBigWinnersAsync(int limit = 10) =>
            Task.FromResult(_winners.OrderByDescending(w => w.WinAmount).Take(limit).ToList());

        // Statistics
        public Task<LottoStatistics> GetStatisticsAsync()
        {
            var stats = new LottoStatistics
            {
                TotalGames = _games.Count,
                ActiveGames = _games.Count(g => g.IsActive),
                TotalDraws = _draws.Count,
                TotalTicketsSold = _tickets.Count,
                TotalPrizePool = _draws.Sum(d => d.TotalPrizePool),
                TotalPayouts = _winners.Sum(w => w.WinAmount),
                TotalWinners = _winners.Count,
                BiggestJackpot = _draws.Max(d => d.Jackpot),
                LastDrawTime = _draws.OrderByDescending(d => d.DrawTime).FirstOrDefault()?.DrawTime
            };

            return Task.FromResult(stats);
        }

        public Task<LottoStatistics> GetUserStatisticsAsync(Guid userId)
        {
            var userTickets = _tickets.Where(t => t.UserId == userId).ToList();
            var userWins = _winners.Where(w => w.UserId == userId).ToList();

            var stats = new LottoStatistics
            {
                TotalTicketsSold = userTickets.Count,
                TotalPayouts = userWins.Sum(w => w.WinAmount),
                TotalWinners = userWins.Count,
                BiggestJackpot = userWins.Max(w => (decimal?)w.WinAmount)
            };

            return Task.FromResult(stats);
        }
    }
}
