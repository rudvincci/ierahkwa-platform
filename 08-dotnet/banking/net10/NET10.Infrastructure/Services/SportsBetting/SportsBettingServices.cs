using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.SportsBetting
{
    public class SportsBettingServices : ISportsBettingServices
    {
        private readonly List<Sport> _sports;
        private readonly List<SportEvent> _events;
        private readonly List<BettingMarket> _markets;
        private readonly List<Odds> _odds;
        private readonly List<Bet> _bets;
        private readonly Dictionary<Guid, BetSlip> _betSlips;
        private readonly List<EventResult> _results;

        public SportsBettingServices()
        {
            _sports = new List<Sport>();
            _events = new List<SportEvent>();
            _markets = new List<BettingMarket>();
            _odds = new List<Odds>();
            _bets = new List<Bet>();
            _betSlips = new Dictionary<Guid, BetSlip>();
            _results = new List<EventResult>();

            InitializeSports();
            InitializeEvents();
            InitializeMarkets();
            InitializeOdds();
        }

        private void InitializeSports()
        {
            _sports.AddRange(new[]
            {
                new Sport { Id = "football", Name = "Football", Icon = "🏈", ActiveEvents = 45, IsLive = true, Priority = 1 },
                new Sport { Id = "basketball", Name = "Basketball", Icon = "🏀", ActiveEvents = 38, IsLive = true, Priority = 2 },
                new Sport { Id = "baseball", Name = "Baseball", Icon = "⚾", ActiveEvents = 22, IsLive = false, Priority = 3 },
                new Sport { Id = "hockey", Name = "Hockey", Icon = "🏒", ActiveEvents = 18, IsLive = true, Priority = 4 },
                new Sport { Id = "soccer", Name = "Soccer", Icon = "⚽", ActiveEvents = 67, IsLive = true, Priority = 1 },
                new Sport { Id = "mma", Name = "MMA/Boxing", Icon = "🥊", ActiveEvents = 8, IsLive = false, Priority = 5 },
                new Sport { Id = "tennis", Name = "Tennis", Icon = "🎾", ActiveEvents = 15, IsLive = true, Priority = 6 },
                new Sport { Id = "golf", Name = "Golf", Icon = "⛳", ActiveEvents = 5, IsLive = false, Priority = 7 }
            });
        }

        private void InitializeEvents()
        {
            var now = DateTime.UtcNow;
            var random = new Random();

            // Football Events
            _events.AddRange(new[]
            {
                new SportEvent
                {
                    Id = "evt-001",
                    SportId = "football",
                    SportName = "Football",
                    HomeTeam = "Kansas City Chiefs",
                    AwayTeam = "Buffalo Bills",
                    HomeTeamLogo = "🏈",
                    AwayTeamLogo = "🏈",
                    StartTime = now.AddHours(2),
                    Status = EventStatus.Scheduled,
                    League = "NFL",
                    Country = "USA",
                    MarketsCount = 12,
                    IsLive = false
                },
                new SportEvent
                {
                    Id = "evt-002",
                    SportId = "football",
                    SportName = "Football",
                    HomeTeam = "Dallas Cowboys",
                    AwayTeam = "Philadelphia Eagles",
                    HomeTeamLogo = "🏈",
                    AwayTeamLogo = "🏈",
                    StartTime = now.AddHours(-1),
                    Status = EventStatus.Live,
                    Score = "21-14",
                    League = "NFL",
                    Country = "USA",
                    MarketsCount = 12,
                    IsLive = true
                }
            });

            // Basketball Events
            _events.AddRange(new[]
            {
                new SportEvent
                {
                    Id = "evt-003",
                    SportId = "basketball",
                    SportName = "Basketball",
                    HomeTeam = "Los Angeles Lakers",
                    AwayTeam = "Boston Celtics",
                    HomeTeamLogo = "🏀",
                    AwayTeamLogo = "🏀",
                    StartTime = now.AddHours(3),
                    Status = EventStatus.Scheduled,
                    League = "NBA",
                    Country = "USA",
                    MarketsCount = 15,
                    IsLive = false
                },
                new SportEvent
                {
                    Id = "evt-004",
                    SportId = "basketball",
                    SportName = "Basketball",
                    HomeTeam = "Golden State Warriors",
                    AwayTeam = "Miami Heat",
                    HomeTeamLogo = "🏀",
                    AwayTeamLogo = "🏀",
                    StartTime = now.AddMinutes(-30),
                    Status = EventStatus.Live,
                    Score = "98-95",
                    League = "NBA",
                    Country = "USA",
                    MarketsCount = 15,
                    IsLive = true
                }
            });

            // Soccer Events
            _events.AddRange(new[]
            {
                new SportEvent
                {
                    Id = "evt-005",
                    SportId = "soccer",
                    SportName = "Soccer",
                    HomeTeam = "Manchester United",
                    AwayTeam = "Liverpool FC",
                    HomeTeamLogo = "⚽",
                    AwayTeamLogo = "⚽",
                    StartTime = now.AddHours(4),
                    Status = EventStatus.Scheduled,
                    League = "Premier League",
                    Country = "England",
                    MarketsCount = 20,
                    IsLive = false
                },
                new SportEvent
                {
                    Id = "evt-006",
                    SportId = "soccer",
                    SportName = "Soccer",
                    HomeTeam = "Real Madrid",
                    AwayTeam = "Barcelona",
                    HomeTeamLogo = "⚽",
                    AwayTeamLogo = "⚽",
                    StartTime = now.AddMinutes(-15),
                    Status = EventStatus.Live,
                    Score = "1-0",
                    League = "La Liga",
                    Country = "Spain",
                    MarketsCount = 20,
                    IsLive = true
                }
            });

            // Baseball Events
            _events.AddRange(new[]
            {
                new SportEvent
                {
                    Id = "evt-007",
                    SportId = "baseball",
                    SportName = "Baseball",
                    HomeTeam = "New York Yankees",
                    AwayTeam = "Boston Red Sox",
                    HomeTeamLogo = "⚾",
                    AwayTeamLogo = "⚾",
                    StartTime = now.AddHours(5),
                    Status = EventStatus.Scheduled,
                    League = "MLB",
                    Country = "USA",
                    MarketsCount = 10,
                    IsLive = false
                }
            });

            // Hockey Events
            _events.AddRange(new[]
            {
                new SportEvent
                {
                    Id = "evt-008",
                    SportId = "hockey",
                    SportName = "Hockey",
                    HomeTeam = "Toronto Maple Leafs",
                    AwayTeam = "Montreal Canadiens",
                    HomeTeamLogo = "🏒",
                    AwayTeamLogo = "🏒",
                    StartTime = now.AddHours(1),
                    Status = EventStatus.Scheduled,
                    League = "NHL",
                    Country = "Canada",
                    MarketsCount = 12,
                    IsLive = false
                }
            });
        }

        private void InitializeMarkets()
        {
            foreach (var evt in _events)
            {
                // Match Winner
                _markets.Add(new BettingMarket
                {
                    Id = $"mkt-{evt.Id}-winner",
                    EventId = evt.Id,
                    Name = "Match Winner",
                    Type = MarketType.MatchWinner,
                    IsActive = true
                });

                // Over/Under
                _markets.Add(new BettingMarket
                {
                    Id = $"mkt-{evt.Id}-overunder",
                    EventId = evt.Id,
                    Name = "Total Goals/Points",
                    Type = MarketType.OverUnder,
                    IsActive = true
                });

                // Both Teams to Score
                if (evt.SportId == "soccer")
                {
                    _markets.Add(new BettingMarket
                    {
                        Id = $"mkt-{evt.Id}-btts",
                        EventId = evt.Id,
                        Name = "Both Teams to Score",
                        Type = MarketType.BothTeamsToScore,
                        IsActive = true
                    });
                }
            }
        }

        private void InitializeOdds()
        {
            var random = new Random();
            foreach (var market in _markets)
            {
                if (market.Type == MarketType.MatchWinner)
                {
                    var evt = _events.First(e => e.Id == market.EventId);
                    _odds.AddRange(new[]
                    {
                        new Odds
                        {
                            Id = $"odds-{market.Id}-home",
                            MarketId = market.Id,
                            Selection = evt.HomeTeam,
                            DecimalOdds = Math.Round(1.5m + (decimal)(random.NextDouble() * 1.5), 2),
                            FractionalOdds = "1/2",
                            AmericanOdds = "+150",
                            Probability = Math.Round(100m / (1.5m + (decimal)(random.NextDouble() * 1.5)), 2),
                            IsActive = true
                        },
                        new Odds
                        {
                            Id = $"odds-{market.Id}-draw",
                            MarketId = market.Id,
                            Selection = "Draw",
                            DecimalOdds = Math.Round(3.0m + (decimal)(random.NextDouble() * 1.0), 2),
                            FractionalOdds = "3/1",
                            AmericanOdds = "+300",
                            Probability = Math.Round(100m / (3.0m + (decimal)(random.NextDouble() * 1.0)), 2),
                            IsActive = true
                        },
                        new Odds
                        {
                            Id = $"odds-{market.Id}-away",
                            MarketId = market.Id,
                            Selection = evt.AwayTeam,
                            DecimalOdds = Math.Round(2.0m + (decimal)(random.NextDouble() * 2.0), 2),
                            FractionalOdds = "2/1",
                            AmericanOdds = "+200",
                            Probability = Math.Round(100m / (2.0m + (decimal)(random.NextDouble() * 2.0)), 2),
                            IsActive = true
                        }
                    });
                }
                else if (market.Type == MarketType.OverUnder)
                {
                    _odds.AddRange(new[]
                    {
                        new Odds
                        {
                            Id = $"odds-{market.Id}-over",
                            MarketId = market.Id,
                            Selection = "Over 2.5",
                            DecimalOdds = Math.Round(1.8m + (decimal)(random.NextDouble() * 0.4), 2),
                            FractionalOdds = "4/5",
                            AmericanOdds = "-125",
                            Probability = Math.Round(100m / (1.8m + (decimal)(random.NextDouble() * 0.4)), 2),
                            IsActive = true
                        },
                        new Odds
                        {
                            Id = $"odds-{market.Id}-under",
                            MarketId = market.Id,
                            Selection = "Under 2.5",
                            DecimalOdds = Math.Round(1.9m + (decimal)(random.NextDouble() * 0.4), 2),
                            FractionalOdds = "9/10",
                            AmericanOdds = "-111",
                            Probability = Math.Round(100m / (1.9m + (decimal)(random.NextDouble() * 0.4)), 2),
                            IsActive = true
                        }
                    });
                }
            }
        }

        // Sports & Events
        public Task<List<Sport>> GetSportsAsync() => Task.FromResult(_sports.OrderBy(s => s.Priority).ToList());

        public Task<List<SportEvent>> GetEventsAsync(string? sportId = null, DateTime? date = null)
        {
            var query = _events.AsQueryable();
            if (!string.IsNullOrEmpty(sportId))
                query = query.Where(e => e.SportId == sportId);
            if (date.HasValue)
                query = query.Where(e => e.StartTime.Date == date.Value.Date);

            var events = query.ToList();
            foreach (var evt in events)
            {
                evt.Markets = _markets.Where(m => m.EventId == evt.Id).ToList();
                foreach (var market in evt.Markets)
                {
                    market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
                }
            }
            return Task.FromResult(events);
        }

        public Task<SportEvent?> GetEventByIdAsync(string eventId)
        {
            var evt = _events.FirstOrDefault(e => e.Id == eventId);
            if (evt != null)
            {
                evt.Markets = _markets.Where(m => m.EventId == evt.Id).ToList();
                foreach (var market in evt.Markets)
                {
                    market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
                }
            }
            return Task.FromResult(evt);
        }

        public Task<List<SportEvent>> GetLiveEventsAsync()
        {
            var liveEvents = _events.Where(e => e.Status == EventStatus.Live).ToList();
            foreach (var evt in liveEvents)
            {
                evt.Markets = _markets.Where(m => m.EventId == evt.Id).ToList();
                foreach (var market in evt.Markets)
                {
                    market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
                }
            }
            return Task.FromResult(liveEvents);
        }

        public Task<List<SportEvent>> GetUpcomingEventsAsync(int hours = 24)
        {
            var cutoff = DateTime.UtcNow.AddHours(hours);
            var upcoming = _events.Where(e => e.Status == EventStatus.Scheduled && e.StartTime <= cutoff).ToList();
            foreach (var evt in upcoming)
            {
                evt.Markets = _markets.Where(m => m.EventId == evt.Id).ToList();
                foreach (var market in evt.Markets)
                {
                    market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
                }
            }
            return Task.FromResult(upcoming);
        }

        // Betting Markets
        public Task<List<BettingMarket>> GetMarketsAsync(string eventId)
        {
            var markets = _markets.Where(m => m.EventId == eventId).ToList();
            foreach (var market in markets)
            {
                market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
            }
            return Task.FromResult(markets);
        }

        public Task<BettingMarket?> GetMarketByIdAsync(string marketId)
        {
            var market = _markets.FirstOrDefault(m => m.Id == marketId);
            if (market != null)
            {
                market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
            }
            return Task.FromResult(market);
        }

        public Task<List<BettingMarket>> GetPopularMarketsAsync()
        {
            var popular = _markets.Take(10).ToList();
            foreach (var market in popular)
            {
                market.Odds = _odds.Where(o => o.MarketId == market.Id).ToList();
            }
            return Task.FromResult(popular);
        }

        // Odds
        public Task<List<Odds>> GetOddsAsync(string marketId) =>
            Task.FromResult(_odds.Where(o => o.MarketId == marketId && o.IsActive).ToList());

        public Task<Odds?> GetOddsByIdAsync(string oddsId) =>
            Task.FromResult(_odds.FirstOrDefault(o => o.Id == oddsId));

        public Task<List<Odds>> GetBestOddsAsync(string eventId, string marketType)
        {
            var markets = _markets.Where(m => m.EventId == eventId && m.Type.ToString() == marketType).ToList();
            var allOdds = new List<Odds>();
            foreach (var market in markets)
            {
                allOdds.AddRange(_odds.Where(o => o.MarketId == market.Id && o.IsActive));
            }
            return Task.FromResult(allOdds.OrderByDescending(o => o.DecimalOdds).ToList());
        }

        // Bets
        public Task<Bet> PlaceBetAsync(PlaceBetRequest request)
        {
            var odds = _odds.FirstOrDefault(o => o.Id == request.OddsId);
            var evt = _events.FirstOrDefault(e => e.Id == request.EventId);
            var market = _markets.FirstOrDefault(m => m.Id == request.MarketId);

            if (odds == null || evt == null || market == null)
                throw new ArgumentException("Invalid bet request");

            var bet = new Bet
            {
                Id = $"bet-{Guid.NewGuid()}",
                UserId = request.UserId,
                EventId = request.EventId,
                EventName = $"{evt.HomeTeam} vs {evt.AwayTeam}",
                MarketId = request.MarketId,
                MarketName = market.Name,
                Selection = odds.Selection,
                Stake = request.Stake,
                Odds = odds.DecimalOdds,
                PotentialWin = Math.Round(request.Stake * odds.DecimalOdds, 2),
                Status = BetStatus.Pending,
                PlacedAt = DateTime.UtcNow
            };

            _bets.Add(bet);
            return Task.FromResult(bet);
        }

        public Task<Bet?> GetBetByIdAsync(string betId) =>
            Task.FromResult(_bets.FirstOrDefault(b => b.Id == betId));

        public Task<List<Bet>> GetUserBetsAsync(Guid userId, BetStatus? status = null)
        {
            var query = _bets.Where(b => b.UserId == userId);
            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);
            return Task.FromResult(query.OrderByDescending(b => b.PlacedAt).ToList());
        }

        public Task<bool> CancelBetAsync(string betId, Guid userId)
        {
            var bet = _bets.FirstOrDefault(b => b.Id == betId && b.UserId == userId);
            if (bet == null || bet.Status != BetStatus.Pending)
                return Task.FromResult(false);

            bet.Status = BetStatus.Cancelled;
            bet.SettledAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        public Task<Bet> CashOutBetAsync(string betId, Guid userId)
        {
            var bet = _bets.FirstOrDefault(b => b.Id == betId && b.UserId == userId);
            if (bet == null || bet.Status != BetStatus.Pending)
                throw new ArgumentException("Bet cannot be cashed out");

            var cashOutAmount = Math.Round(bet.PotentialWin * 0.7m, 2); // 70% of potential win
            bet.Status = BetStatus.CashOut;
            bet.Payout = cashOutAmount;
            bet.SettledAt = DateTime.UtcNow;
            bet.Result = "Cash Out";

            return Task.FromResult(bet);
        }

        // Bet Slips
        public Task<BetSlip> CreateBetSlipAsync(Guid userId)
        {
            var betSlip = new BetSlip
            {
                UserId = userId,
                Selections = new List<BetSlipSelection>(),
                TotalStake = 0,
                TotalPotentialWin = 0,
                CombinedOdds = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _betSlips[userId] = betSlip;
            return Task.FromResult(betSlip);
        }

        public Task<BetSlip?> GetBetSlipAsync(Guid userId)
        {
            if (!_betSlips.TryGetValue(userId, out var betSlip))
                return Task.FromResult<BetSlip?>(null);
            return Task.FromResult<BetSlip?>(betSlip);
        }

        public Task<BetSlip> AddToBetSlipAsync(Guid userId, AddToBetSlipRequest request)
        {
            if (!_betSlips.TryGetValue(userId, out var betSlip))
                betSlip = CreateBetSlipAsync(userId).Result;

            var evt = _events.FirstOrDefault(e => e.Id == request.EventId);
            var market = _markets.FirstOrDefault(m => m.Id == request.MarketId);

            var selection = new BetSlipSelection
            {
                Id = $"sel-{Guid.NewGuid()}",
                EventId = request.EventId,
                EventName = evt != null ? $"{evt.HomeTeam} vs {evt.AwayTeam}" : "Unknown Event",
                MarketId = request.MarketId,
                MarketName = market?.Name ?? "Unknown Market",
                Selection = request.Selection,
                Odds = request.Odds
            };

            betSlip.Selections.Add(selection);
            betSlip.CombinedOdds = betSlip.Selections.Aggregate(1m, (acc, s) => acc * s.Odds);
            betSlip.UpdatedAt = DateTime.UtcNow;

            _betSlips[userId] = betSlip;
            return Task.FromResult(betSlip);
        }

        public Task<bool> RemoveFromBetSlipAsync(Guid userId, string selectionId)
        {
            if (!_betSlips.TryGetValue(userId, out var betSlip))
                return Task.FromResult(false);

            var selection = betSlip.Selections.FirstOrDefault(s => s.Id == selectionId);
            if (selection == null)
                return Task.FromResult(false);

            betSlip.Selections.Remove(selection);
            betSlip.CombinedOdds = betSlip.Selections.Any() 
                ? betSlip.Selections.Aggregate(1m, (acc, s) => acc * s.Odds)
                : 1;
            betSlip.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult(true);
        }

        public Task<BetSlip> ClearBetSlipAsync(Guid userId)
        {
            if (!_betSlips.TryGetValue(userId, out var betSlip))
                betSlip = CreateBetSlipAsync(userId).Result;

            betSlip.Selections.Clear();
            betSlip.TotalStake = 0;
            betSlip.TotalPotentialWin = 0;
            betSlip.CombinedOdds = 1;
            betSlip.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult(betSlip);
        }

        public Task<Bet> PlaceBetSlipAsync(Guid userId, decimal stake)
        {
            if (!_betSlips.TryGetValue(userId, out var betSlip) || !betSlip.Selections.Any())
                throw new ArgumentException("Bet slip is empty");

            var firstSelection = betSlip.Selections.First();
            var evt = _events.FirstOrDefault(e => e.Id == firstSelection.EventId);
            var market = _markets.FirstOrDefault(m => m.Id == firstSelection.MarketId);

            var bet = new Bet
            {
                Id = $"bet-{Guid.NewGuid()}",
                UserId = userId,
                EventId = firstSelection.EventId,
                EventName = firstSelection.EventName,
                MarketId = firstSelection.MarketId,
                MarketName = firstSelection.MarketName,
                Selection = string.Join(" + ", betSlip.Selections.Select(s => s.Selection)),
                Stake = stake,
                Odds = betSlip.CombinedOdds,
                PotentialWin = Math.Round(stake * betSlip.CombinedOdds, 2),
                Status = BetStatus.Pending,
                PlacedAt = DateTime.UtcNow
            };

            _bets.Add(bet);
            betSlip.TotalStake = stake;
            betSlip.TotalPotentialWin = bet.PotentialWin;

            return Task.FromResult(bet);
        }

        // Results & Settlements
        public Task<List<EventResult>> GetResultsAsync(DateTime? date = null, string? sportId = null)
        {
            var query = _results.AsQueryable();
            if (date.HasValue)
                query = query.Where(r => r.StartTime.Date == date.Value.Date);
            if (!string.IsNullOrEmpty(sportId))
                query = query.Where(r => r.SportId == sportId);
            return Task.FromResult(query.OrderByDescending(r => r.StartTime).ToList());
        }

        public Task<EventResult?> GetEventResultAsync(string eventId) =>
            Task.FromResult(_results.FirstOrDefault(r => r.EventId == eventId));

        public Task<bool> SettleBetsAsync(string eventId)
        {
            var evt = _events.FirstOrDefault(e => e.Id == eventId);
            if (evt == null || evt.Status != EventStatus.Finished)
                return Task.FromResult(false);

            var pendingBets = _bets.Where(b => b.EventId == eventId && b.Status == BetStatus.Pending).ToList();
            foreach (var bet in pendingBets)
            {
                // Simplified settlement logic
                var random = new Random();
                var won = random.Next(0, 2) == 1;
                bet.Status = won ? BetStatus.Won : BetStatus.Lost;
                bet.Payout = won ? bet.PotentialWin : 0;
                bet.SettledAt = DateTime.UtcNow;
                bet.Result = won ? "Won" : "Lost";
            }

            return Task.FromResult(true);
        }

        // Statistics
        public Task<BettingStatistics> GetStatisticsAsync(Guid userId)
        {
            var userBets = _bets.Where(b => b.UserId == userId).ToList();
            var stats = new BettingStatistics
            {
                UserId = userId,
                TotalBets = userBets.Count,
                WonBets = userBets.Count(b => b.Status == BetStatus.Won),
                LostBets = userBets.Count(b => b.Status == BetStatus.Lost),
                PendingBets = userBets.Count(b => b.Status == BetStatus.Pending),
                TotalStaked = userBets.Sum(b => b.Stake),
                TotalWon = userBets.Where(b => b.Status == BetStatus.Won).Sum(b => b.Payout ?? 0),
                TotalLost = userBets.Where(b => b.Status == BetStatus.Lost).Sum(b => b.Stake),
                NetProfit = userBets.Where(b => b.Status == BetStatus.Won).Sum(b => b.Payout ?? 0) -
                           userBets.Where(b => b.Status == BetStatus.Lost).Sum(b => b.Stake),
                WinRate = userBets.Any() ? (decimal)userBets.Count(b => b.Status == BetStatus.Won) / userBets.Count * 100 : 0,
                AverageOdds = userBets.Any() ? userBets.Average(b => b.Odds) : 0
            };
            return Task.FromResult(stats);
        }

        public Task<List<PopularBet>> GetPopularBetsAsync()
        {
            var popular = _bets
                .GroupBy(b => new { b.EventId, b.MarketId, b.Selection })
                .Select(g => new PopularBet
                {
                    EventId = g.Key.EventId,
                    EventName = g.First().EventName,
                    MarketName = g.First().MarketName,
                    Selection = g.Key.Selection,
                    BetCount = g.Count(),
                    TotalStake = g.Sum(b => b.Stake)
                })
                .OrderByDescending(p => p.BetCount)
                .Take(10)
                .ToList();
            return Task.FromResult(popular);
        }

        public Task<List<TrendingEvent>> GetTrendingEventsAsync()
        {
            var trending = _events
                .Select(e => new TrendingEvent
                {
                    EventId = e.Id,
                    EventName = $"{e.HomeTeam} vs {e.AwayTeam}",
                    SportName = e.SportName,
                    ViewCount = new Random().Next(1000, 10000),
                    BetCount = _bets.Count(b => b.EventId == e.Id),
                    StartTime = e.StartTime
                })
                .OrderByDescending(t => t.ViewCount)
                .Take(10)
                .ToList();
            return Task.FromResult(trending);
        }
    }
}
