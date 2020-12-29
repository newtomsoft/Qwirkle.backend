﻿using Qwirkle.Core.CommonContext;

namespace Qwirkle.Core.PlayerContext.Entities
{
    public class Tile
    {
        public int Id { get; }
        public byte RackPosition { get; }
        public TileColor Color { get; }
        public TileForm Form { get; }


        public Tile(int id, TileColor color, TileForm form, byte rackPosition)
        {
            Id = id;
            RackPosition = rackPosition;
            Color = color;
            Form = form;
        }

        public Tile(int tileId, TileColor color, TileForm form)
        {
            Id = tileId;
            Color = color;
            Form = form;
        }
    }
}
