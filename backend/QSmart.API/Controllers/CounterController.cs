using Microsoft.AspNetCore.Mvc;
using QSmart.Application.DTOs.Counter;
using QSmart.Application.Interfaces;
using QSmart.Domain.Entities;

namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CounterController : ControllerBase
{
    private readonly ICounterRepository _counterRepository;

    public CounterController(
        ICounterRepository counterRepository)
    {
        _counterRepository = counterRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var counters =
            await _counterRepository.GetAllAsync();

        return Ok(counters);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var counter =
            await _counterRepository.GetByIdAsync(id);

        if (counter == null)
            return NotFound();

        return Ok(counter);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCounterRequest request)
    {
        var counter = new Counter
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BranchId = request.BranchId,
            IsActive = true
        };

        await _counterRepository.AddAsync(counter);
        await _counterRepository.SaveChangesAsync();

        return Ok(counter);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCounterRequest request)
    {
        var counter =
            await _counterRepository.GetByIdAsync(id);

        if (counter == null)
            return NotFound();

        counter.Name = request.Name;
        counter.BranchId = request.BranchId;
        counter.IsActive = request.IsActive;

        await _counterRepository.SaveChangesAsync();

        return Ok(counter);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
    var counter = await _counterRepository.GetByIdAsync(id);

    if (counter == null)
        return NotFound();

    counter.IsActive = false;

    await _counterRepository.SaveChangesAsync();

    return Ok("Counter disabled successfully.");
    }



    [HttpPut("{id}/enable")]
    public async Task<IActionResult> Enable(Guid id)
    {
        var counter = await _counterRepository.GetByIdAsync(id);

        if (counter == null)
            return NotFound();

        counter.IsActive = true;

        await _counterRepository.SaveChangesAsync();

        return Ok(counter);
    }

    [HttpPut("{id}/disable")]
    public async Task<IActionResult> Disable(Guid id)
    {
        var counter = await _counterRepository.GetByIdAsync(id);

        if (counter == null)
            return NotFound();

        counter.IsActive = false;

        await _counterRepository.SaveChangesAsync();

        return Ok(counter);
    }
}