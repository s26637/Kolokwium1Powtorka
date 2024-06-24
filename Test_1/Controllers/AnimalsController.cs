using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Test_1.Models.DTOs;
using Test_1.Repositories;

namespace Test_1.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalsRepository _animalsRepository;
    public AnimalsController(IAnimalsRepository animalsRepository)
    {
        _animalsRepository = animalsRepository;
    }
    

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAnimal(int id)
    {
        if (!await _animalsRepository.DoesAnimalExist(id))
            return NotFound($"Animal with given ID - {id} doesn't exist");

        var animal = await _animalsRepository.GetAnimal(id);
            
        return Ok(animal);
    }

    [HttpPost]
    public async Task<IActionResult> AddAnimal(NewAnimalWithProcedures newAnimalWithProcedures)
    {
        if (!await _animalsRepository.DoesOwnerExist(newAnimalWithProcedures.OwnerId))
            return NotFound($"Owner with given ID - {newAnimalWithProcedures.OwnerId} doesnis't ext");

        foreach (var procedure in newAnimalWithProcedures.Procedures)
        {
            if (!await _animalsRepository.DoesProcedureExist(procedure.ProcedureId))
                return NotFound($"Procedure with given ID - {procedure.ProcedureId} doesn't exist");
        }
        
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var id = await _animalsRepository.AddAnimal(new NewAnimalDTO()
            {
                Name = newAnimalWithProcedures.Name,
                Type = newAnimalWithProcedures.Type,
                AdmissionDate = newAnimalWithProcedures.AdmissionDate,
                OwnerId = newAnimalWithProcedures.OwnerId
            });

            foreach (var procedure in newAnimalWithProcedures.Procedures)
            {
                await _animalsRepository.AddProcedureAnimal(id, procedure);
            }

            scope.Complete();
        }

        return Created(Request.Path.Value ?? "api/animals", newAnimalWithProcedures);

    }
    
    

}

