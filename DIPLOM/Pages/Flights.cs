using DIPLOM.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIPLOM.Pages
{
    internal class Flights
    {
        // Добавьте эти свойства в класс Flights (если их нет)
        public virtual Airports DepartureAirport { get; set; }
        public virtual Airports ArrivalAirport { get; set; }
        public virtual Aircrafts Aircraft { get; set; }
    }
}
