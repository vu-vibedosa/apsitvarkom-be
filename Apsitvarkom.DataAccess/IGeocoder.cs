using Apsitvarkom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsitvarkom.DataAccess
{
    public interface IGeocoder
    {
        public Task<string> ReverseGeocode(Coordinates cords);
    }
}
