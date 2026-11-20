using System;
using System.Collections.Generic;
using HotelManagement.Core.Exceptions;
using HotelManagement.Core.Models;
using HotelManagement.Core.Services;

namespace HotelManagement.UI
{
    public class Menu
    {
        private readonly HotelService _hotelService;

        public Menu(HotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public void Run()
        {
            try
            {
                _hotelService.LoadAllData();
                ConsoleHelper.PrintSuccess("Дані готельного комплексу успішно завантажено.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Помилка завантаження даних: {ex.Message}");
            }

            bool isRunning = true;
            while (isRunning)
            {
                PrintMainMenu();
                int choice = ConsoleHelper.ReadInt("Оберіть пункт меню: ", 0, 12);
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case 1:
                            ShowAllRooms();
                            break;
                        case 2:
                            SearchAvailableRooms();
                            break;
                        case 3:
                            AddNewRoom();
                            break;
                        case 4:
                            RemoveRoom();
                            break;
                        case 5:
                            ShowAllGuests();
                            break;
                        case 6:
                            RegisterGuest();
                            break;
                        case 7:
                            SearchGuests();
                            break;
                        case 8:
                            ShowAndAddExtraServices();
                            break;
                        case 9:
                            CreateBooking();
                            break;
                        case 10:
                            AddServiceToExistingBooking();
                            break;
                        case 11:
                            ShowAllBookings();
                            break;
                        case 12:
                            CancelBooking();
                            break;
                        case 0:
                            _hotelService.SaveAllData();
                            ConsoleHelper.PrintSuccess("Усі дані збережено у JSON-файли. До побачення!");
                            isRunning = false;
                            break;
                    }
                }
                catch (DoubleBookingException ex)
                {
                    ConsoleHelper.PrintError($"Конфлікт бронювання: {ex.Message}");
                }
                catch (InvalidStayDurationException ex)
                {
                    ConsoleHelper.PrintError($"Некоректні дати: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    ConsoleHelper.PrintError($"Некоректні дані: {ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    ConsoleHelper.PrintError($"Операція заборонена: {ex.Message}");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Непередбачена помилка: {ex.Message}");
                }

                if (isRunning)
                {
                    Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
                    Console.ReadLine();
                }
            }
        }

        private void PrintMainMenu()
        {
            Console.Clear();
            ConsoleHelper.PrintInfo("==========================================================");
            ConsoleHelper.PrintInfo("      ГОТЕЛЬНИЙ КОМПЛЕКС ТА СИСТЕМА БРОНЮВАННЯ (ООП)      ");
            ConsoleHelper.PrintInfo("==========================================================");
            Console.WriteLine("  [Номери]");
            Console.WriteLine("   1. Переглянути всі номери (сортування за номером / ціною)");
            Console.WriteLine("   2. Знайти вільні номери на обрані дати");
            Console.WriteLine("   3. Додати новий номер");
            Console.WriteLine("   4. Видалити номер");
            Console.WriteLine("  [Гості]");
            Console.WriteLine("   5. Переглянути всіх гостей");
            Console.WriteLine("   6. Зареєструвати нового гостя");
            Console.WriteLine("   7. Пошук гостя");
            Console.WriteLine("  [Послуги та бронювання]");
            Console.WriteLine("   8. Додаткові послуги (перегляд та додавання)");
            Console.WriteLine("   9. Оформити бронювання номера");
            Console.WriteLine("  10. Додати послугу до існуючого бронювання");
            Console.WriteLine("  11. Переглянути всі бронювання");
            Console.WriteLine("  12. Скасувати бронювання");
            Console.WriteLine("  ----------------------------------------------------------");
            Console.WriteLine("   0. Зберегти дані у файл та вийти");
            ConsoleHelper.PrintInfo("==========================================================");
        }

        private void ShowAllRooms()
        {
            Console.WriteLine("Оберіть спосіб сортування:");
            Console.WriteLine("  1. За номером кімнати");
            Console.WriteLine("  2. За ціною за ніч (від дешевших)");
            int sortChoice = ConsoleHelper.ReadInt("Ваш вибір (1-2): ", 1, 2);

            List<Room> rooms = _hotelService.GetAllRooms(sortChoice == 2);
            if (rooms.Count == 0)
            {
                ConsoleHelper.PrintInfo("Список номерів порожній.");
                return;
            }

            ConsoleHelper.PrintInfo("\n--- Список номерів готелю ---");
            foreach (Room room in rooms)
            {
                Console.WriteLine(room);
            }
        }

        private void SearchAvailableRooms()
        {
            DateTime checkIn = ConsoleHelper.ReadDate("Дата заїзду (ДД.ММ.РРРР): ");
            DateTime checkOut = ConsoleHelper.ReadDate("Дата виїзду (ДД.ММ.РРРР): ");

            Console.Write("Фільтрувати за категорією? (1 - так, 0 - ні): ");
            RoomCategory? category = null;
            if (Console.ReadLine()?.Trim() == "1")
            {
                category = ConsoleHelper.ReadRoomCategory();
            }

            int minCapacity = ConsoleHelper.ReadInt("Мінімальна кількість гостей (0 - будь-яка): ", 0, 10);
            int? capacityFilter = minCapacity > 0 ? minCapacity : null;

            List<Room> available = _hotelService.GetAvailableRooms(checkIn, checkOut, category, capacityFilter);
            if (available.Count == 0)
            {
                ConsoleHelper.PrintInfo("Вільних номерів за вказаними параметрами не знайдено.");
                return;
            }

            ConsoleHelper.PrintSuccess($"\nЗнайдено вільних номерів: {available.Count}");
            foreach (Room room in available)
            {
                Console.WriteLine(room);
            }
        }

        private void AddNewRoom()
        {
            int number = ConsoleHelper.ReadInt("Введіть номер кімнати: ", 1, 9999);
            RoomCategory category = ConsoleHelper.ReadRoomCategory();
            int capacity = ConsoleHelper.ReadInt("Місткість гостей (1-10): ", 1, 10);
            decimal price = ConsoleHelper.ReadDecimal("Ціна за одну ніч (грн): ", 1);

            Room created = _hotelService.AddRoom(number, category, capacity, price);
            ConsoleHelper.PrintSuccess($"Додано: {created}");
        }

        private void RemoveRoom()
        {
            int number = ConsoleHelper.ReadInt("Введіть номер кімнати для видалення: ", 1, 9999);
            bool removed = _hotelService.RemoveRoom(number);
            if (removed)
            {
                ConsoleHelper.PrintSuccess($"Номер {number} успішно видалено.");
            }
            else
            {
                ConsoleHelper.PrintError($"Номер {number} не знайдено.");
            }
        }

        private void ShowAllGuests()
        {
            List<Guest> guests = _hotelService.GetAllGuests();
            if (guests.Count == 0)
            {
                ConsoleHelper.PrintInfo("Список гостей порожній.");
                return;
            }

            ConsoleHelper.PrintInfo("\n--- Зареєстровані гості ---");
            foreach (Guest guest in guests)
            {
                Console.WriteLine(guest);
            }
        }

        private void RegisterGuest()
        {
            string fullName = ConsoleHelper.ReadNonEmptyString("ПІБ гостя: ");
            string passport = ConsoleHelper.ReadNonEmptyString("Номер паспорта: ");
            string phone = ConsoleHelper.ReadNonEmptyString("Номер телефону: ");

            Guest guest = _hotelService.AddGuest(fullName, passport, phone);
            ConsoleHelper.PrintSuccess($"Гостя зареєстровано: {guest}");
        }

        private void SearchGuests()
        {
            string query = ConsoleHelper.ReadNonEmptyString("Введіть ПІБ, паспорт або телефон для пошуку: ");
            List<Guest> found = _hotelService.SearchGuests(query);
            if (found.Count == 0)
            {
                ConsoleHelper.PrintInfo("Гостей не знайдено.");
                return;
            }

            ConsoleHelper.PrintSuccess($"\nЗнайдено гостей: {found.Count}");
            foreach (Guest guest in found)
            {
                Console.WriteLine(guest);
            }
        }

        private void ShowAndAddExtraServices()
        {
            List<ExtraService> services = _hotelService.GetAllExtraServices();
            ConsoleHelper.PrintInfo("\n--- Доступні додаткові послуги ---");
            for (int i = 0; i < services.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {services[i]}");
            }

            Console.Write("\nБажаєте додати нову послугу? (1 - так, 0 - ні): ");
            if (Console.ReadLine()?.Trim() == "1")
            {
                string name = ConsoleHelper.ReadNonEmptyString("Назва нової послуги: ");
                decimal price = ConsoleHelper.ReadDecimal("Вартість послуги (грн): ", 0);
                ExtraService created = _hotelService.AddExtraService(name, price);
                ConsoleHelper.PrintSuccess($"Послугу додано: {created}");
            }
        }

        private void CreateBooking()
        {
            ShowAllRooms();
            Console.WriteLine();
            int roomNumber = ConsoleHelper.ReadInt("Оберіть номер кімнати для бронювання: ", 1, 9999);
            string passport = ConsoleHelper.ReadNonEmptyString("Введіть номер паспорта зареєстрованого гостя: ");
            DateTime checkIn = ConsoleHelper.ReadDate("Дата заїзду (ДД.ММ.РРРР): ");
            DateTime checkOut = ConsoleHelper.ReadDate("Дата виїзду (ДД.ММ.РРРР): ");

            List<Guid> selectedServiceIds = new List<Guid>();
            List<ExtraService> availableServices = _hotelService.GetAllExtraServices();

            if (availableServices.Count > 0)
            {
                ConsoleHelper.PrintInfo("\nДоступні додаткові послуги:");
                for (int i = 0; i < availableServices.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {availableServices[i]}");
                }
                Console.WriteLine("  0. Завершити вибір послуг");

                while (true)
                {
                    int serviceChoice = ConsoleHelper.ReadInt("Додати послугу за номером (0 - далі): ", 0, availableServices.Count);
                    if (serviceChoice == 0)
                    {
                        break;
                    }
                    ExtraService chosen = availableServices[serviceChoice - 1];
                    selectedServiceIds.Add(chosen.Id);
                    ConsoleHelper.PrintSuccess($"Додано послугу: {chosen.Name}");
                }
            }

            Booking booking = _hotelService.CreateBooking(roomNumber, passport, checkIn, checkOut, selectedServiceIds);
            ConsoleHelper.PrintSuccess("\nБронювання успішно оформлено!");
            Console.WriteLine(booking);
        }

        private void AddServiceToExistingBooking()
        {
            List<Booking> bookings = _hotelService.GetAllBookings();
            if (bookings.Count == 0)
            {
                ConsoleHelper.PrintInfo("Активних бронювань немає.");
                return;
            }

            ConsoleHelper.PrintInfo("\n--- Активні бронювання ---");
            for (int i = 0; i < bookings.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {bookings[i]}");
            }

            int bookingIndex = ConsoleHelper.ReadInt("Оберіть номер бронювання зі списку: ", 1, bookings.Count) - 1;

            List<ExtraService> services = _hotelService.GetAllExtraServices();
            if (services.Count == 0)
            {
                ConsoleHelper.PrintInfo("Список додаткових послуг порожній.");
                return;
            }

            ConsoleHelper.PrintInfo("\n--- Доступні послуги ---");
            for (int i = 0; i < services.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {services[i]}");
            }

            int serviceIndex = ConsoleHelper.ReadInt("Оберіть послугу для додавання: ", 1, services.Count) - 1;

            _hotelService.AddServiceToBooking(bookings[bookingIndex].Id, services[serviceIndex].Id);
            ConsoleHelper.PrintSuccess("Послугу додано до бронювання! Оновлена інформація:");
            Console.WriteLine(bookings[bookingIndex]);
        }

        private void ShowAllBookings()
        {
            List<Booking> bookings = _hotelService.GetAllBookings();
            if (bookings.Count == 0)
            {
                ConsoleHelper.PrintInfo("Бронювань поки немає.");
                return;
            }

            ConsoleHelper.PrintInfo("\n--- Усі бронювання готелю ---");
            for (int i = 0; i < bookings.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {bookings[i]}");
            }
        }

        private void CancelBooking()
        {
            List<Booking> bookings = _hotelService.GetAllBookings();
            if (bookings.Count == 0)
            {
                ConsoleHelper.PrintInfo("Бронювань для скасування немає.");
                return;
            }

            ShowAllBookings();
            int index = ConsoleHelper.ReadInt("\nВведіть порядковий номер бронювання для скасування (0 - відміна): ", 0, bookings.Count);
            if (index == 0)
            {
                return;
            }

            bool canceled = _hotelService.CancelBooking(bookings[index - 1].Id);
            if (canceled)
            {
                ConsoleHelper.PrintSuccess("Бронювання успішно скасовано.");
            }
            else
            {
                ConsoleHelper.PrintError("Не вдалося скасувати бронювання.");
            }
        }
    }
}