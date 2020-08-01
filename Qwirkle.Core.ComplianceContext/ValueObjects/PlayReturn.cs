using Qwirkle.Core.CommonContext;
using Qwirkle.Core.ComplianceContext.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qwirkle.Core.ComplianceContext.ValueObjects
{
    public struct PlayReturn
    {
        public PlayReturnCode Code { get; set; }
        public Tile Tile { get; set; }
        public int Points { get; set; }
    }
}
