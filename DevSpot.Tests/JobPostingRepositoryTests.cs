using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devspot.Data;
using Devspot.Models;
using Devspot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DevSpot.Tests
{
    public class JobPostingRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public JobPostingRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("JobPostingDb")
                .Options;
        }

        private ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);

        [Fact]
        public async Task AddAsync_ShouldAddJobPosting()
        {
            // create db
            var db = CreateDbContext();

            //jobPosting repository
            var repository = new JobPostingRepository(db);

            // job posting
            var jobPosting = new JobPosting
            {
                Title = "Test Title",
                Description = "Test Description",
                Company = "Test Company",
                Location = "Test Location",
                PostedDate = DateTime.Now,
                UserId = "testUserId"
            };

            // execute
            await repository.AddAsync(jobPosting);

            // result
            var result = db.JobPostings.FirstOrDefault(x => x.Title == "Test Title");

            // assert
            Assert.NotNull(result);
            Assert.Equal("Test Title", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnJobPosting()
        {
            var db = CreateDbContext();
            
            var repository = new JobPostingRepository(db);

            var jobPosting = new JobPosting
            {
                Title = "Test Title",
                Description = "Test Description",
                Company = "Test Company",
                Location = "Test Location",
                PostedDate = DateTime.Now,
                UserId = "testUserId"
            };

            await db.JobPostings.AddAsync(jobPosting);
            await db.SaveChangesAsync();

            var result = repository.GetByIdAsync(jobPosting.Id);
        }
    }
}
