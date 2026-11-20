using System;

namespace HotelManagement.Core.Exceptions
{
    public class DoubleBookingException : Exception
    {
        public int RoomNumber { get; }
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }

        public DoubleBookingException(int roomNumber, DateTime checkInDate, DateTime checkOutDate)
            : base($"Номер {roomNumber} вже зайнятий у період з {checkInDate:dd.MM.yyyy} по {checkOutDate:dd.MM.yyyy}.")
        {
            RoomNumber = roomNumber;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }

        public DoubleBookingException(string message) : base(message)
        {
        }
    }
}