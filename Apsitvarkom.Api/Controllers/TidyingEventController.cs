using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TidyingEventController : ControllerBase
{
    private readonly IRepository<TidyingEvent> _repository;
    private readonly IMapper _mapper;

    public TidyingEventController(IRepository<TidyingEvent> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
}