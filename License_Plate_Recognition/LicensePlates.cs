using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace License_Plate_Recognition
{
    public class LicensePlates
    {
        public LicensePlates(string plateNumber, DateTime time)
        {
            this.PlateNumber = plateNumber;
            this.Time = time;
        }
        public string PlateNumber;
        public DateTime Time;
    }
}
