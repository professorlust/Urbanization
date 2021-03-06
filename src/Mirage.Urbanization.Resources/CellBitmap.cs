﻿using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public class CellBitmap : BaseBitmap
    {
        private static int _idCounter = default(int);
        public int Id { get; }
        public CellBitmap(Bitmap bitmap) : base(bitmap)
        {
            Id = Interlocked.Increment(ref _idCounter);
        }
    }
}
