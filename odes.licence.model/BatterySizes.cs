using System.Collections.Generic;
using System.Linq;

namespace Odes.Licence.Model
{
    public static class BatterySizes
    {
        public static IOrderedEnumerable<BatterySize> Batteries { get; set; }

        static BatterySizes()
        {
            var list = new List<BatterySize>();

            list.AddRange(
                new[]
                    {
                        new BatterySize(){Name = "10k", Number = 10000, Option = "A"}, 
                        new BatterySize(){Name = "20k", Number = 20000, Option = "B"}, 
                        new BatterySize(){Name = "50k", Number = 50000, Option = "C"}, 
                        new BatterySize(){Name = "100k", Number = 100000, Option = "D"}, 
                        new BatterySize(){Name = "250k", Number = 250000, Option = "E"}, 
                        new BatterySize(){Name = "500k", Number = 500000, Option = "F"}, 
                        new BatterySize(){Name = "1M", Number = 1000000, Option = "G"}, 
                    }
                );
            Batteries = list.OrderBy(b => b.Number);
        }

        public static bool IsValidOptionSupplied(string numberOfClicks)
        {
            bool valid;
            switch (numberOfClicks)
            {
                case "A": case "B": case "C": case "D": case "E": case "F": case "G":
                    valid = true;
                    break;
                default:
                    valid = false;
                    break;
            }
            return valid;
        }

        public static int GetClickCountFromOption(string numberOfClicks)
        {
            int clicks=0;
            switch (numberOfClicks)
            {
                case "A":
                    clicks = 10000;
                    break;
                case "B":
                    clicks = 20000;
                    break;
                case "C":
                    clicks = 50000;
                    break;
                case "D":
                    clicks = 100000;
                    break;
                case "E":
                    clicks = 250000;
                    break;
                case "F":
                    clicks = 500000;
                    break;
                case "G":
                    clicks = 1000000;
                    break;
            }
            return clicks;
        }
    }
}