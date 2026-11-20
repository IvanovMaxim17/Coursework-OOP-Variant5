using System.Collections.Generic;
using HotelManagement.Core.Models;

namespace HotelManagement.Core.Comparers
{
    public class RoomPriceComparer : IComparer<Room>
    {
        private readonly bool _ascending;

        public RoomPriceComparer(bool ascending = true)
        {
            _ascending = ascending;
        }

        public int Compare(Room? x, Room? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int result = x.PricePerNight.CompareTo(y.PricePerNight);
            return _ascending ? result : -result;
        }
    }
}