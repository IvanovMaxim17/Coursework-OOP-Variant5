using System;
using System.Text;
using HotelManagement.Core.Services;

namespace HotelManagement.UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            HotelService hotelService = new HotelService("HotelData");
            Menu menu = new Menu(hotelService);
            menu.Run();
        }
    }
}