using System;

namespace HotelManagement.Core.Models
{
    public class ExtraService : BaseEntity
    {
        private string _name = string.Empty;
        private decimal _price;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Назва послуги не може бути порожньою.");
                }
                _name = value.Trim();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Вартість послуги не може бути від'ємною.");
                }
                _price = value;
            }
        }

        public ExtraService()
        {
        }

        public ExtraService(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"{Name} ({Price:F2} грн)";
        }
    }
}