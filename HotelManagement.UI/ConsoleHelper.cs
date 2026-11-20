using System;
using System.Globalization;
using HotelManagement.Core.Models;

namespace HotelManagement.UI
{
    public static class ConsoleHelper
    {
        public static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }
                PrintError("Поле не може бути порожнім. Спробуйте ще раз.");
            }
        }

        public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result) && result >= min && result <= max)
                {
                    return result;
                }
                PrintError($"Введіть коректне ціле число (від {min} до {max}).");
            }
        }

        public static decimal ReadDecimal(string prompt, decimal min = 0)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Replace(',', '.');
                if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) && result >= min)
                {
                    return result;
                }
                PrintError($"Введіть коректне число (не менше {min}).");
            }
        }

        public static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (DateTime.TryParseExact(input, new[] { "dd.MM.yyyy", "d.M.yyyy", "yyyy-MM-dd" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date.Date;
                }
                PrintError("Некоректний формат дати. Використовуйте формат ДД.ММ.РРРР (наприклад, 25.11.2026).");
            }
        }

        public static RoomCategory ReadRoomCategory()
        {
            Console.WriteLine("Оберіть категорію номера:");
            Console.WriteLine("  1. Standard");
            Console.WriteLine("  2. Deluxe");
            Console.WriteLine("  3. Suite");
            int choice = ReadInt("Ваш вибір (1-3): ", 1, 3);
            return (RoomCategory)choice;
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Успіх] {message}");
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Помилка] {message}");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}