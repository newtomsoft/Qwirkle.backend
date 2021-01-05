using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.Infra.Repository.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Qwirkle.Core.Tests
{
    public class CreateGameShould
    {
        private CommonUseCase CommonUseCase { get; set; }
        private IRepositoryPort Repository { get; set; }
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
            Repository = new RepositoryAdapter(DbContext);
            CommonUseCase = new CommonUseCase(Repository);
            AddUsers();
        }

        private void AddUsers()
        {
            DbContext.Users.Add(new UserModel { Id = USER1 });
            DbContext.Users.Add(new UserModel { Id = USER2 });
            DbContext.Users.Add(new UserModel { Id = USER3 });
            DbContext.Users.Add(new UserModel { Id = USER4 });
            DbContext.SaveChanges();
        }

        [Fact]
        public void CreateGoodPlayersWithOrder1234()
        {
            var userIds = new List<int> { USER1, USER2, USER3, USER4 };
            var players = CommonUseCase.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 4);
            Assert.Equal(1, players.Count(p => p.IsTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Equal(4, players.Select(p => p.Rack.Tiles.Count == TILES_NUMBER_PER_PLAYER).Count());

        }

        [Fact]
        public void CreateGoodPlayersWithOrder123()
        {
            var userIds = new List<int> { USER1, USER3, USER4 };
            var players = CommonUseCase.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
            Assert.Equal(1, players.Count(p => p.IsTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Equal(3, players.Select(p => p.Rack.Tiles.Count == TILES_NUMBER_PER_PLAYER).Count());
        }

        [Fact]
        public void CreateGoodPlayersWithOrder12()
        {
            var userIds = new List<int> { USER3, USER4 };
            var players = CommonUseCase.CreateGame(userIds);

            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
            Assert.Equal(1, players.Count(p => p.IsTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Equal(2, players.Select(p => p.Rack.Tiles.Count == TILES_NUMBER_PER_PLAYER).Count());
        }

        [Fact]
        public void CreateGoodPlayerWithOrder1()
        {
            var userIds = new List<int> { USER3 };
            var players = CommonUseCase.CreateGame(userIds);
            Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
            Assert.Equal(1, players.Count(p => p.IsTurn));
            Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
            Assert.Single(players.Select(p => p.Rack.Tiles.Count == TILES_NUMBER_PER_PLAYER));
        }
    }
}