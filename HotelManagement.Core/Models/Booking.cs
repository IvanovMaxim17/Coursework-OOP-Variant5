using System;
using System.Collections.Generic;
using System.Linq;
using HotelManagement.Core.Exceptions;

namespace HotelManagement.Core.Models
{
    public class Booking : BaseEntity
    {
        private DateTime _checkInDate;
        private DateTime _checkOutDate;
        private List<ExtraService> _services = new List<ExtraService>();

        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public decimal RoomPricePerNight { get; set; }

        public Guid GuestId { get; set; }
        public string GuestFullName { get; set; } = string.Empty;

        public DateTime CheckInDate
        {
            get => _checkInDate;
            set => _checkInDate = value.Date;
        }

        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set => _checkOutDate = value.Date;
        }

        public List<ExtraService> Services
        {
            get => _services;
            set => _services = value ?? new List<ExtraService>();
        }

        public int NightsCount
        {
            get
            {
                int days = (CheckOutDate.Date - CheckInDate.Date).Days;
                return days > 0 ? days : 0;
            }
        }

        public decimal ServicesTotalPrice => Services.Sum(s => s.Price);

        public decimal TotalPrice => (RoomPricePerNight * NightsCount) + ServicesTotalPrice;

        public Booking()
        {
        }

        public Booking(Room room, Guest guest, DateTime checkInDate, DateTime checkOutDate)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room), "Номер не може бути порожнім.");
            }
            if (guest == null)
            {
                throw new ArgumentNullException(nameof(guest), "Гість не може бути порожнім.");
            }

            SetDates(checkInDate, checkOutDate);

            RoomId = room.Id;
            RoomNumber = room.RoomNumber;
            RoomPricePerNight = room.PricePerNight;
            GuestId = guest.Id;
            GuestFullName = guest.FullName;
        }

        public void SetDates(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkOutDate.Date <= checkInDate.Date)
            {
                throw new InvalidStayDurationException(checkInDate, checkOutDate);
            }

            CheckInDate = checkInDate.Date;
            CheckOutDate = checkOutDate.Date;
        }

        public void AddService(ExtraService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Послуга не може бути порожньою.");
            }
            Services.Add(service);
        }

        public override string ToString()
        {
            string servicesText = Services.Count > 0
                ? string.Join(", ", Services.Select(s => s.Name))
                : "без додаткових послуг";

            return $"Бронювання номера #{RoomNumber} | Гість: {GuestFullName} | " +
                   $"{CheckInDate:dd.MM.yyyy} - {CheckOutDate:dd.MM.yyyy} ({NightsCount} ноч.) | " +
                   $"Послуги: {servicesText} | Разом: {TotalPrice:F2} грн";
        }
    }
}