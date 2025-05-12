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
            _options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("JobPostingDb").Options;
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
            var result = await repository.GetByIdAsync(jobPosting.Id);
            Assert.NotNull((result));
            Assert.Equal("Test Title", result.Title);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException()
        {
            var db = CreateDbContext();
            var repository = new JobPostingRepository(db);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(999));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllJobPostings()
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
            var jobPosting2 = new JobPosting
            {
                Title = "Test Title2",
                Description = "Test Description2",
                Company = "Test Company2",
                Location = "Test Location2",
                PostedDate = DateTime.Now,
                UserId = "testUserId2"
            };
            await db.JobPostings.AddRangeAsync(jobPosting, jobPosting2);
            await db.SaveChangesAsync();
            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateJobPosting()
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
            jobPosting.Description = "Updated Description";
            await repository.UpdateAsync(jobPosting);
            var result = await db.JobPostings.FindAsync(jobPosting.Id);
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteJobPosting()
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
            await repository.DeleteAsync(jobPosting.Id);
            var result = await db.JobPostings.ToListAsync();
            Assert.DoesNotContain(result, jp => jp.Id == jobPosting.Id);
        }
    }
}