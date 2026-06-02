using Microsoft.AspNetCore.Mvc;
using QSmart.Application.Interfaces;
using QSmart.Application.DTOs.Branch;
using QSmart.Domain.Entities;


namespace QSmart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchController : ControllerBase
{
    private readonly IBranchRepository _branchRepository;

    public BranchController(
        IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var branches =
            await _branchRepository.GetAllAsync();

        return Ok(branches);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBranchRequest request)
    {
    var branch = new Branch
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Code = request.Code,
        IsActive = true
    };

    await _branchRepository.AddAsync(branch);

    await _branchRepository.SaveChangesAsync();

    return Ok(branch);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    Guid id,
        UpdateBranchRequest request)
    {
        var branch =
            await _branchRepository
                .GetByIdAsync(id);

        if (branch == null)
        {
            return NotFound();
        }

        branch.Name = request.Name;
        branch.Code = request.Code;
        branch.IsActive = request.IsActive;

        await _branchRepository.SaveChangesAsync();

        return Ok(branch);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);

        if (branch == null)
            return NotFound("Branch not found.");

        await _branchRepository.DeleteAsync(branch);

        return Ok("Branch deleted successfully.");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);

        if (branch == null)
            return NotFound("Branch not found.");

        return Ok(branch);
    }
}