using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Infra.Persistance;
using Qwirkle.Infra.Persistance.Adapters;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class CreateGameShould
    {
        private ComplianceService ComplianceService { get; set; }
        private ICompliancePersistance Persistance { get; set; }
        private DefaultDbContext DbContext { get; set; }

        private const int USER1 = 71;
        private const int USER2 = 21;
        private const int USER3 = 3;
        private const int USER4 = 14;

        public CreateGameShould()
        {
            var factory = new ConnectionFactory();
            DbContext = factory.CreateContextForInMemory();
            Persistance = new CompliancePersistanceAdapter(DbContext);
            ComplianceService = new ComplianceService(Persistance);
            AddUsers();
        }

        private void AddUsers()
        {
            DbContext.Users.Add(new UserPersistance { Id = USER1 });
            DbContext.Users.Add(new UserPersistance { Id = USER2 });
            DbContext.Users.Add(new UserPersistance { Id = USER3 });
            DbContext.Users.Add(new UserPersistance { Id = USER4 });
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
            Assert.DoesNotContain(players.Select(p => p.GameTurn), gameTurn => gameTurn);
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.DoesNotContain(players.Select(p => p.Tiles), tiles => tiles.Count > 0);

        }

        [Fact]
        public void CreateGoodPlayersWithOrder123()
        {
            var userIds = new List<int> { USER1, USER3, USER4 };
            var players = ComplianceService.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
            Assert.DoesNotContain(players.Select(p => p.GameTurn), gameTurn => gameTurn);
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.DoesNotContain(players.Select(p => p.Tiles), tiles => tiles.Count > 0);
        }

        [Fact]
        public void CreateGoodPlayersWithOrder12()
        {
            var userIds = new List<int> { USER3, USER4 };
            var players = ComplianceService.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.DoesNotContain(players.Select(p => p.GameTurn), gameTurn => gameTurn);
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.DoesNotContain(players.Select(p => p.Tiles), tiles => tiles.Count > 0);
        }

        [Fact]
        public void CreateGoodPlayerWithOrder1()
        {
            var userIds = new List<int> { USER3 };
            var players = ComplianceService.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.DoesNotContain(players.Select(p => p.GameTurn), gameTurn => gameTurn);
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.DoesNotContain(players.Select(p => p.Tiles), tiles => tiles.Count > 0);
        }
    }
}
