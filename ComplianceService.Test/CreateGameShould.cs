﻿using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Infra.Persistence;
using Qwirkle.Infra.Persistence.Adapters;
using Qwirkle.Infra.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class CreateGameShould
    {
        private ComplianceService ComplianceService { get; set; }
        private ICompliancePersistence Persistence { get; set; }
        private DefaultDbContext DbContext { get; set; }

        private const int USER1 = 71;
        private const int USER2 = 21;
        private const int USER3 = 3;
        private const int USER4 = 14;
        private const int TILES_NUMBER_PER_PLAYER = 6;

        public CreateGameShould()
        {
            var factory = new ConnectionFactory();
            DbContext = factory.CreateContextForInMemory();
            Persistence = new CompliancePersistenceAdapter(DbContext);
            ComplianceService = new ComplianceService(Persistence);
            AddUsers();
        }

        private void AddUsers()
        {
            DbContext.Users.Add(new UserPersistence { Id = USER1 });
            DbContext.Users.Add(new UserPersistence { Id = USER2 });
            DbContext.Users.Add(new UserPersistence { Id = USER3 });
            DbContext.Users.Add(new UserPersistence { Id = USER4 });
            DbContext.SaveChanges();
        }

        [Fact]
        public void CreateGoodPlayersWithOrder1234()
        {
            var userIds = new List<int> { USER1, USER2, USER3, USER4 };
            var players = ComplianceService.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 4);
            Assert.Equal(1, players.Count(p => p.GameTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Equal(4, players.Select(p => p.Tiles.Count == TILES_NUMBER_PER_PLAYER).Count());

        }

        [Fact]
        public void CreateGoodPlayersWithOrder123()
        {
            var userIds = new List<int> { USER1, USER3, USER4 };
            var players = ComplianceService.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
            Assert.Equal(1, players.Count(p => p.GameTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Equal(3, players.Select(p => p.Tiles.Count == TILES_NUMBER_PER_PLAYER).Count());
        }

        [Fact]
        public void CreateGoodPlayersWithOrder12()
        {
            var userIds = new List<int> { USER3, USER4 };
            var players = ComplianceService.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Equal(1, players.Count(p => p.GameTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Equal(2, players.Select(p => p.Tiles.Count == TILES_NUMBER_PER_PLAYER).Count());
        }

        [Fact]
        public void CreateGoodPlayerWithOrder1()
        {
            var userIds = new List<int> { USER3 };
            var players = ComplianceService.CreateGame(userIds);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Equal(1, players.Count(p => p.GameTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Single(players.Select(p => p.Tiles.Count == TILES_NUMBER_PER_PLAYER));
        }
    }
}