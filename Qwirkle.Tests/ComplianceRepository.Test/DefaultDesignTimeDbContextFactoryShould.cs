using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.Ports;
using Qwirkle.Core.ValueObjects;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.Infra.Repository.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Qwirkle.Core.ComplianceRepository.Tests
{
    public class DefaultDesignTimeDbContextFactoryShould
    {
        #region private attributs

        #endregion



        #region private methods

        #endregion

        [Fact]
        public void Prout()
        {
            DefaultDesignTimeDbContextFactory designDbContextFactory = new DefaultDesignTimeDbContextFactory();
            designDbContextFactory.CreateDbContext(null);
        }
    }
}
