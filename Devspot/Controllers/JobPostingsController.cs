using Devspot.Constants;
using Devspot.Models;
using Devspot.Repositories;
using Devspot.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Devspot.Controllers;

[Authorize]
public class JobPostingsController : Controller
{
    private readonly IRepository<JobPosting> _repository;
    private readonly UserManager<IdentityUser> _userManager;

    public JobPostingsController(IRepository<JobPosting> repository, UserManager<IdentityUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    // GET
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(Roles.Employer))
        {
            var allJobPostings = await _repository.GetAllAsync();
            var userId = _userManager.GetUserId(User);
            var filteredJobPostings = allJobPostings.Where(jp => jp.UserId == userId);

            return View(filteredJobPostings);
        }

        var jobPostings = await _repository.GetAllAsync();

        return View(jobPostings);
    }

    [Authorize(Roles = "Admin,Employer")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employer")]
    public async Task<IActionResult> Create(JobPostingViewModel jobPostingVm)
    {
        if (ModelState.IsValid)
        {
            var jobPosting = new JobPosting
            {
                Title = jobPostingVm.Title,
                Description = jobPostingVm.Description,
                Company = jobPostingVm.Company,
                Location = jobPostingVm.Location,
                UserId = _userManager.GetUserId(User)
            };
            await _repository.AddAsync(jobPosting);
            return RedirectToAction(nameof(Index));
        }

        return View(jobPostingVm);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,Employer")]
    public async Task<IActionResult> Delete(int id)
    {
        var jobPosting = await _repository.GetByIdAsync(id);

        if (jobPosting == null)
        {
            NotFound("Job posting is not found.");
        }

        var userId = _userManager.GetUserId(User);

        if (User.IsInRole(Roles.Admin) == false && jobPosting.UserId != userId)
        {
            return Forbid();
        }

        await _repository.DeleteAsync(id);

        return Ok();
    }
}