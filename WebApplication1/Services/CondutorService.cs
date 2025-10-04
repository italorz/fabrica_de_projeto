using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Services;

public class CondutorService
{
    private readonly ICondutorRepository _condutorRepository;
    private readonly ILogger<CondutorController> _logger;

    public CondutorService(ICondutorRepository condutorRepository, ILogger<CondutorController> logger)
    {
        _condutorRepository = condutorRepository;
        _logger = logger;
    }

    async Task<ActionResult<IEnumerable<Condutor>>> getAllCondutor(){
    
        return 
    
    }
}



