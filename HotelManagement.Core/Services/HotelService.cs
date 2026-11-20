using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HotelManagement.Core.Comparers;
using HotelManagement.Core.Exceptions;
using HotelManagement.Core.Interfaces;
using HotelManagement.Core.Models;
using HotelManagement.Core.Repositories;

namespace HotelManagement.Core.Services
{
    public class HotelService
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Guest> _guestRepository;
        private readonly IRepository<ExtraService> _serviceRepository;
        private readonly IRepository<Booking> _bookingRepository;

        private readonly JsonFileService<Room> _roomFileService;
        private readonly JsonFileService<Guest> _guestFileService;
        private readonly JsonFileService<ExtraService> _serviceFileService;
        private readonly JsonFileService<Booking> _bookingFileService;

        public HotelService(string dataDirectory = "HotelData")
        {
            _roomRepository = new Repository<Room>();
            _guestRepository = new Repository<Guest>();
            _serviceRepository = new Repository<ExtraService>();
            _bookingRepository = new Repository<Booking>();

            _roomFileService = new JsonFileService<Room>(Path.Combine(dataDirectory, "rooms.json"));
            _guestFileService = new JsonFileService<Guest>(Path.Combine(dataDirectory, "guests.json"));
            _serviceFileService = new JsonFileService<ExtraService>(Path.Combine(dataDirectory, "services.json"));
            _bookingFileService = new JsonFileService<Booking>(Path.Combine(dataDirectory, "bookings.json"));
        }

        public void LoadAllData()
        {
            _roomRepository.ReplaceAll(_roomFileService.Load());
            _guestRepository.ReplaceAll(_guestFileService.Load());
            _serviceRepository.ReplaceAll(_serviceFileService.Load());
            _bookingRepository.ReplaceAll(_bookingFileService.Load());

            if (_roomRepository.GetAll().Count == 0 && _serviceRepository.GetAll().Count == 0)
            {
                SeedDefaultData();
            }
        }

        public void SaveAllData()
        {
            _roomFileService.Save(_roomRepository.GetAll());
            _guestFileService.Save(_guestRepository.GetAll());
            _serviceFileService.Save(_serviceRepository.GetAll());
            _bookingFileService.Save(_bookingRepository.GetAll());
        }

        private void SeedDefaultData()
        {
            _roomRepository.Add(new Room(101, RoomCategory.Standard, 2, 1200m));
            _roomRepository.Add(new Room(102, RoomCategory.Standard, 1, 950m));
            _roomRepository.Add(new Room(201, RoomCategory.Deluxe, 3, 2100m));
            _roomRepository.Add(new Room(301, RoomCategory.Suite, 4, 3500m));

            _serviceRepository.Add(new ExtraService("Сніданок (шведський стіл)", 300m));
            _serviceRepository.Add(new ExtraService("СПА-зона та басейн", 650m));
            _serviceRepository.Add(new ExtraService("Трансфер з вокзалу/аеропорту", 500m));
        }

        public Room AddRoom(int roomNumber, RoomCategory category, int capacity, decimal pricePerNight)
        {
            if (_roomRepository.Find(r => r.RoomNumber == roomNumber).Any())
            {
                throw new ArgumentException($"Номер кімнати {roomNumber} вже існує в системі.");
            }

            Room room = new Room(roomNumber, category, capacity, pricePerNight);
            _roomRepository.Add(room);
            return room;
        }

        public bool RemoveRoom(int roomNumber)
        {
            Room? room = _roomRepository.Find(r => r.RoomNumber == roomNumber).FirstOrDefault();
            if (room == null)
            {
                return false;
            }

            bool hasBookings = _bookingRepository.Find(b => b.RoomId == room.Id).Any();
            if (hasBookings)
            {
                throw new InvalidOperationException($"Неможливо видалити номер {roomNumber}, оскільки для нього є активні бронювання.");
            }

            return _roomRepository.Remove(room.Id);
        }

        public List<Room> GetAllRooms(bool sortByPrice = false)
        {
            List<Room> rooms = _roomRepository.GetAll();
            if (sortByPrice)
            {
                rooms.Sort(new RoomPriceComparer(true));
            }
            else
            {
                rooms.Sort();
            }
            return rooms;
        }

        public List<Room> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate, RoomCategory? category = null, int? minCapacity = null)
        {
            if (checkOutDate.Date <= checkInDate.Date)
            {
                throw new InvalidStayDurationException(checkInDate, checkOutDate);
            }

            List<Room> rooms = _roomRepository.Find(room =>
            {
                if (category.HasValue && room.Category != category.Value)
                {
                    return false;
                }
                if (minCapacity.HasValue && room.Capacity < minCapacity.Value)
                {
                    return false;
                }
                return IsRoomAvailable(room.Id, checkInDate, checkOutDate);
            });

            rooms.Sort();
            return rooms;
        }

        public Guest AddGuest(string fullName, string passportNumber, string phoneNumber)
        {
            if (_guestRepository.Find(g => g.PassportNumber.Equals(passportNumber.Trim(), StringComparison.OrdinalIgnoreCase)).Any())
            {
                throw new ArgumentException($"Гість із паспортом '{passportNumber}' вже зареєстрований.");
            }

            Guest guest = new Guest(fullName, passportNumber, phoneNumber);
            _guestRepository.Add(guest);
            return guest;
        }

        public List<Guest> GetAllGuests()
        {
            return _guestRepository.GetAll();
        }

        public List<Guest> SearchGuests(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return GetAllGuests();
            }

            string normalized = query.Trim();
            return _guestRepository.Find(g =>
                g.FullName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                g.PassportNumber.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                g.PhoneNumber.Contains(normalized, StringComparison.OrdinalIgnoreCase));
        }

        public ExtraService AddExtraService(string name, decimal price)
        {
            if (_serviceRepository.Find(s => s.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)).Any())
            {
                throw new ArgumentException($"Послуга '{name}' вже існує.");
            }

            ExtraService service = new ExtraService(name, price);
            _serviceRepository.Add(service);
            return service;
        }

        public List<ExtraService> GetAllExtraServices()
        {
            return _serviceRepository.GetAll();
        }

        public bool IsRoomAvailable(Guid roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            List<Booking> roomBookings = _bookingRepository.Find(b => b.RoomId == roomId);
            foreach (Booking existing in roomBookings)
            {
                bool overlaps = checkInDate.Date < existing.CheckOutDate.Date && checkOutDate.Date > existing.CheckInDate.Date;
                if (overlaps)
                {
                    return false;
                }
            }
            return true;
        }

        public Booking CreateBooking(int roomNumber, string guestPassport, DateTime checkInDate, DateTime checkOutDate, List<Guid>? serviceIds = null)
        {
            if (checkOutDate.Date <= checkInDate.Date)
            {
                throw new InvalidStayDurationException(checkInDate, checkOutDate);
            }

            Room? room = _roomRepository.Find(r => r.RoomNumber == roomNumber).FirstOrDefault();
            if (room == null)
            {
                throw new ArgumentException($"Номер кімнати {roomNumber} не знайдено.");
            }

            Guest? guest = _guestRepository.Find(g => g.PassportNumber.Equals(guestPassport.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (guest == null)
            {
                throw new ArgumentException($"Гостя з паспортом '{guestPassport}' не знайдено.");
            }

            if (!IsRoomAvailable(room.Id, checkInDate, checkOutDate))
            {
                throw new DoubleBookingException(room.RoomNumber, checkInDate, checkOutDate);
            }

            Booking booking = new Booking(room, guest, checkInDate, checkOutDate);

            if (serviceIds != null && serviceIds.Count > 0)
            {
                foreach (Guid serviceId in serviceIds)
                {
                    ExtraService? service = _serviceRepository.GetById(serviceId);
                    if (service != null)
                    {
                        booking.AddService(service);
                    }
                }
            }

            _bookingRepository.Add(booking);
            return booking;
        }

        public void AddServiceToBooking(Guid bookingId, Guid serviceId)
        {
            Booking? booking = _bookingRepository.GetById(bookingId);
            if (booking == null)
            {
                throw new ArgumentException("Бронювання не знайдено.");
            }

            ExtraService? service = _serviceRepository.GetById(serviceId);
            if (service == null)
            {
                throw new ArgumentException("Додаткову послугу не знайдено.");
            }

            booking.AddService(service);
        }

        public bool CancelBooking(Guid bookingId)
        {
            return _bookingRepository.Remove(bookingId);
        }

        public List<Booking> GetAllBookings()
        {
            return _bookingRepository.GetAll();
        }

        public List<Booking> GetBookingsByGuestPassport(string passportNumber)
        {
            Guest? guest = _guestRepository.Find(g => g.PassportNumber.Equals(passportNumber.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (guest == null)
            {
                return new List<Booking>();
            }

            return _bookingRepository.Find(b => b.GuestId == guest.Id);
        }
    }
}