﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwirkle.Web.Api.VueModels
{
    public class TileViewModel
    {
        public int PlayerId { get; set; }
        public int TileId { get; set; }
        public sbyte X { get; set; }
        public sbyte Y { get; set; }
    }
}