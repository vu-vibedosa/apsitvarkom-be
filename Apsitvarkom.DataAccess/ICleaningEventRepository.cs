using Apsitvarkom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Apsitvarkom.DataAccess
{
    public interface ICleaningEventRepository : IRepository<CleaningEvent>
    {
        /// <summary>Checks if a <see cref="T"/> has a parent with the specified condition.</summary>
        /// <param name="propertyCondition">Condition for the requested record check.</param>
        /// <returns>False i</returns>
        public Task<bool> ParentExistsByPropertyAsync(Expression<Func<PollutedLocation, bool>> propertyCondition);
    }
}