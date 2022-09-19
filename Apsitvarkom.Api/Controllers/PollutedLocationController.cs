using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Microsoft.AspNetCore.Mvc;


namespace Apsitvarkom.Api.Controllers  
{
    [ApiController]
    [Route("/api/[controller]")]

    public class PollutedLocationController : ControllerBase
    {
        /*temporary list PollutedLocations. Latter on we will have stored data in file or database. Then we will create
        static DataContext object _context and in constructor
        declare _context value to DataContext object
        */
        
        private static IEnumerable<PollutedLocationDTO> PollutedLocations = new List<PollutedLocationDTO>
        {

            new PollutedLocationDTO
            {
                Id = "5be2354e-2500-4289-bbe2-66210592e17f",
                Location = new LocationDTO
                {
                    Latitude = 12.1651,
                    Longitude = 121.151,
                },
                Radius = 10,
                Severity = "MODERATE",
                Spotted  =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ttt"),
                Progress = 15,
                Notes = "This is test notes"

            }
        };
        /*@Parameters : Object Id 
         * @Returns : List of objects that are still in List
         * @Action : Deleted object from list
         */

        [HttpGet("{id}")]
        public async Task<ActionResult<PollutedLocationDTO>> GetById(string id)
        {
            var PollutedLocation = PollutedLocations.FirstOrDefault(PollutedPlace => PollutedPlace.Id == id);
            if (PollutedLocation == null)
            {
                return BadRequest("Polluted location is not found - wrong id provided or id is not provided at all");
            }
            return Ok(PollutedLocation);
        }
        /*@Parameters : 
         * @Returns : List of all objects
         * @Action : Returns all list of objects 
         */
        [HttpGet]
        public async Task<ActionResult<List<PollutedLocationDTO>>> GetAll()
        {

            return Ok(PollutedLocations);
        }
        /*@Parameters : Object
         * @Returns : Inserted Object
         * @Action : Inserts Object into List
         */

        [HttpPost]
        public async Task<ActionResult<IEnumerable<PollutedLocationDTO>>> Insert(PollutedLocationDTO PolutedLocation)
        {

            var List = PollutedLocations.ToList();
            List.Add(PolutedLocation);

            PollutedLocations = List;
            return Ok(PolutedLocation);
        }

        /*@Parameters : Object 
         * @Returns : Updated Object
         * @Action : Updates given object by its Id 
         */

        [HttpPut]
        public async Task<ActionResult<List<PollutedLocation>>> Update(PollutedLocationDTO PollutedLocation)
        {

            var list = PollutedLocations.ToList();
            if(list.Remove(list.Where(predicate: x => x.Id == PollutedLocation.Id).FirstOrDefault())){
                list.Add(PollutedLocation);
                PollutedLocations = list;
                return Ok(PollutedLocation);
            }
            return BadRequest();

        }
        /*@Parameters : Object Id 
         * @Returns : List of objects that are still in List
         * @Action : Deleted object from list
         */

        [HttpDelete("{id}")]
        public async Task<ActionResult<PollutedLocationDTO>> Delete(string id)
        {
            var FoundPollutionLocation = PollutedLocations.FirstOrDefault(PollutedPlace => PollutedPlace.Id == id);
            if (FoundPollutionLocation == null)
            {
                return BadRequest("Heroe not found");
            }
            var PollutedLocationList = PollutedLocations.ToList();
            PollutedLocationList.Remove(FoundPollutionLocation);
            PollutedLocations = PollutedLocationList;
            return Ok(PollutedLocations);
        }
    }
}
