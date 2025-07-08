using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using SchoolChoice.DeferredAcceptance.Services;
using SchoolChoice.DeferredAcceptance.Models;

namespace SchoolChoice.DeferredAcceptance.Tests
{
    public class DeferredAcceptanceTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MockDeferredAcceptanceService _mockService;

        public DeferredAcceptanceTests()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            services.AddScoped<MockDeferredAcceptanceService>();
            
            _serviceProvider = services.BuildServiceProvider();
            _mockService = _serviceProvider.GetService<MockDeferredAcceptanceService>();
        }

        [Fact]
        public async Task TestSiblingAssignment_SmithFamily()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var smithSiblings = result.Assignments.Where(a => a.StudentName.Contains("Smith")).ToList();
            Assert.Equal(2, smithSiblings.Count);

            var smithSchools = smithSiblings.Select(s => s.AssignedSchool).Distinct().ToList();
            Assert.Single(smithSchools);
            Assert.True(smithSchools.Count == 1, "Smith siblings should be at the same school");
        }

        [Fact]
        public async Task TestSiblingAssignment_JohnsonFamily()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var johnsonSiblings = result.Assignments.Where(a => a.StudentName.Contains("Johnson")).ToList();
            Assert.Equal(2, johnsonSiblings.Count);
            
            var johnsonSchools = johnsonSiblings.Select(s => s.AssignedSchool).Distinct().ToList();
            Assert.Single(johnsonSchools);

            var assignedSchool = johnsonSchools.First();
            Assert.True(assignedSchool.StartsWith("HIGH"), "Johnson siblings should be at a high school");
        }

        [Fact]
        public async Task TestSiblingFallback_GarciaFamily()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var garciaSiblings = result.Assignments.Where(a => a.StudentName.Contains("Garcia")).ToList();
            Assert.Equal(2, garciaSiblings.Count);

            var garciaSchools = garciaSiblings.Select(s => s.AssignedSchool).Distinct().ToList();
            Assert.Single(garciaSchools);
        }

        [Fact]
        public async Task TestSiblingPreferenceBoost_RodriguezFamily()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var rodriguezSiblings = result.Assignments.Where(a => a.StudentName.Contains("Rodriguez")).ToList();
            Assert.Equal(2, rodriguezSiblings.Count);
            
            var rodriguezSchools = rodriguezSiblings.Select(s => s.AssignedSchool).Distinct().ToList();
            Assert.Single(rodriguezSchools);
            
            Assert.True(rodriguezSiblings.All(s => !string.IsNullOrEmpty(s.SiblingLocationCode)));
        }

        [Fact]
        public async Task TestSiblingPreferenceBoost_TorresFamily()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var torres = result.Assignments.FirstOrDefault(a => a.StudentName.Contains("Torres"));
            Assert.NotNull(torres);
            
            Assert.NotNull(torres.AssignedSchool);
            Assert.True(!string.IsNullOrEmpty(torres.SiblingLocationCode), "Torres should have sibling location data");
        }

        [Fact]
        public async Task TestPriorityOrdering()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);

            var ryan = result.Assignments.First(a => a.StudentName.Contains("Ryan"));
            Assert.True(ryan.PreferenceRank > 0, "Ryan should get assigned to one of his preferences");

            var johnsonAssignments = result.Assignments.Where(a => a.StudentName.Contains("Johnson")).ToList();
            Assert.True(johnsonAssignments.All(a => a.PreferenceRank > 0), "Johnson family should get assigned");
        }

        [Fact]
        public async Task TestAllStudentsAssigned()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            Assert.Empty(result.UnmatchedStudents);
            Assert.Equal(13, result.Assignments.Count); 
        }

        [Fact]
        public async Task TestSiblingGroupIntegrity()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var siblingGroups = result.Assignments.GroupBy(a => a.SiblingGroupId).ToList();
            
            Assert.True(siblingGroups.Count >= 4, "Should have at least 4 sibling groups");
            
            var multiStudentGroups = siblingGroups.Where(g => g.Count() > 1).ToList();
            Assert.True(multiStudentGroups.Count >= 4, "Should have at least 4 sibling families");
        }

        [Fact]
        public async Task TestSchoolCapacityRespected()
        {
            var result = await _mockService.RunDeferredAcceptanceAsync(2024);
            
            var schoolAssignments = result.Assignments
                .GroupBy(a => $"{a.AssignedSchool}_{GetGradeFromStudentName(a.StudentName)}")
                .ToList();
            
            var schools = MockDataService.GetMockSchools();
            
            foreach (var schoolGroup in schoolAssignments)
            {
                var schoolKey = schoolGroup.Key;
                if (schools.ContainsKey(schoolKey))
                {
                    var school = schools[schoolKey];
                    Assert.True(schoolGroup.Count() <= school.AvailableSeats, 
                        $"School {schoolKey} exceeded capacity: {schoolGroup.Count()} > {school.AvailableSeats}");
                }
            }
        }

        private string GetGradeFromStudentName(string studentName)
        {
            var students = MockDataService.GetMockStudents();
            var student = students.FirstOrDefault(s => s.FullName == studentName);
            return student?.Grade ?? "XX";
        }
    }
}