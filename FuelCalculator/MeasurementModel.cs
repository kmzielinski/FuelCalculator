using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace FuelCalculator
    {
        public class MeasurementModel
        {
            public int Id { get; set; }
            public DateTime RefuelingDate { get; set; }
            public int Counter { get; set; }
            public decimal Amount { get; set; }
            public decimal Price { get; set; }

            public decimal Kilometers { get; set; }
            public decimal PricePerLiter { get; set; }
            public decimal PricePer100Km { get; set; }
            public decimal FuelConsumption { get; set; }

            public decimal PriceperLiter { get; set; }

            public decimal PricePre100Km { get; set; }
        }
    }


}
