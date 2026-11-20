using System;

namespace HotelManagement.Core.Exceptions
{
    public class InvalidStayDurationException : Exception
    {
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }

        public InvalidStayDurationException(DateTime checkInDate, DateTime checkOutDate)
            : base($"Некоректна тривалість проживання: дата виїзду ({checkOutDate:dd.MM.yyyy}) має бути пізнішою за дату заїзду ({checkInDate:dd.MM.yyyy}).")
        {
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }

        public InvalidStayDurationException(string message) : base(message)
        {
        }
    }
}