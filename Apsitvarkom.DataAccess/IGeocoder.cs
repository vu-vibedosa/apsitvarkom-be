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
        Task<string> ReverseGeocodeAsync(Coordinates cords);
    }
}
