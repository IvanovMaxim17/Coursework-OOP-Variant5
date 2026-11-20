using System;

namespace HotelManagement.Core.Models
{
    public class Room : BaseEntity, IComparable<Room>
    {
        private int _roomNumber;
        private RoomCategory _category;
        private int _capacity;
        private decimal _pricePerNight;

        public int RoomNumber
        {
            get => _roomNumber;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Номер кімнати має бути додатним числом.");
                }
                _roomNumber = value;
            }
        }

        public RoomCategory Category
        {
            get => _category;
            set => _category = value;
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0 || value > 10)
                {
                    throw new ArgumentException("Місткість номера має бути від 1 до 10 гостей.");
                }
                _capacity = value;
            }
        }

        public decimal PricePerNight
        {
            get => _pricePerNight;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Ціна за ніч має бути більшою за 0.");
                }
                _pricePerNight = value;
            }
        }

        public Room()
        {
        }

        public Room(int roomNumber, RoomCategory category, int capacity, decimal pricePerNight)
        {
            RoomNumber = roomNumber;
            Category = category;
            Capacity = capacity;
            PricePerNight = pricePerNight;
        }

        public int CompareTo(Room? other)
        {
            if (other == null)
            {
                return 1;
            }
            return RoomNumber.CompareTo(other.RoomNumber);
        }

        public override string ToString()
        {
            return $"Номер #{RoomNumber} [{Category}] | Місткість: {Capacity} ос. | Ціна: {PricePerNight:F2} грн/ніч";
        }
    }
}