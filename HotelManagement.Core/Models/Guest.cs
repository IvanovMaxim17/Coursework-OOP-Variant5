using System;

namespace HotelManagement.Core.Models
{
    public class Guest : BaseEntity
    {
        private string _fullName = string.Empty;
        private string _passportNumber = string.Empty;
        private string _phoneNumber = string.Empty;

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ПІБ гостя не може бути порожнім.");
                }
                _fullName = value.Trim();
            }
        }

        public string PassportNumber
        {
            get => _passportNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Номер паспорта не може бути порожнім.");
                }
                _passportNumber = value.Trim();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Номер телефону не може бути порожнім.");
                }
                _phoneNumber = value.Trim();
            }
        }

        public Guest()
        {
        }

        public Guest(string fullName, string passportNumber, string phoneNumber)
        {
            FullName = fullName;
            PassportNumber = passportNumber;
            PhoneNumber = phoneNumber;
        }

        public override string ToString()
        {
            return $"{FullName} | Паспорт: {PassportNumber} | Тел.: {PhoneNumber}";
        }
    }
}