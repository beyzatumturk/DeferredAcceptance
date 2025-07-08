using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolChoice.DeferredAcceptance.Services;

namespace SchoolChoice.DeferredAcceptance
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => 
                builder.AddConsole()
                       .SetMinimumLevel(LogLevel.Information));
            services.AddScoped<MockDeferredAcceptanceService>();
            
            var serviceProvider = services.BuildServiceProvider();
            var mockService = serviceProvider.GetService<MockDeferredAcceptanceService>();

            try
            {
                var result = await mockService.RunDeferredAcceptanceAsync(2024);

                
                var siblingGroups = result.Assignments.GroupBy(a => a.SiblingGroupId).ToList();
                
                foreach (var group in siblingGroups.OrderByDescending(g => g.Max(s => s.PriorityPoints)))
                {
                    var assignments = group.ToList();
                    if (assignments.Count > 1)
                    {
                        Console.WriteLine($"SIBLING GROUP ({assignments.First().SiblingGroupId}):");
                        foreach (var assignment in assignments)
                        {
                            Console.WriteLine($"  {assignment.StudentName} (Grade {GetGradeFromStudent(assignment)}) -> {assignment.SchoolName} (Choice #{assignment.PreferenceRank}) [Priority: {assignment.PriorityPoints}]");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        var assignment = assignments.First();
                        Console.WriteLine($"{assignment.StudentName} (Grade {GetGradeFromStudent(assignment)}) -> {assignment.SchoolName} (Choice #{assignment.PreferenceRank}) [Priority: {assignment.PriorityPoints}]");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("UNMATCHED STUDENTS");
                if (result.UnmatchedStudents.Any())
                {
                    foreach (var student in result.UnmatchedStudents)
                    {
                        Console.WriteLine($"{student.FullName} (Grade {student.Grade}) - No assignment");
                    }
                }
                else
                {
                    Console.WriteLine("All students were successfully matched!");
                }

                Console.WriteLine();
                Console.WriteLine($"Total Rounds: {result.TotalRounds}");
                Console.WriteLine($"Processing Time: {result.ProcessingTime.TotalMilliseconds:F1} ms");
                Console.WriteLine($"Matched Students: {result.Assignments.Count}");
                Console.WriteLine($"Unmatched Students: {result.UnmatchedStudents.Count}");
                Console.WriteLine($"Sibling Groups: {siblingGroups.Count}");
                Console.WriteLine($"Sibling Groups Kept Together: {siblingGroups.Count(g => g.Count() > 1 && g.Select(s => s.AssignedSchool).Distinct().Count() == 1)}");
                Console.WriteLine($"Students with Sibling School Preference: {result.Assignments.Count(a => a.IsAtSiblingSchool)}");
                
                Console.WriteLine();
                Console.WriteLine("SIBLING SCHOOL PLACEMENTS:");
                var siblingsWithSchools = result.Assignments.Where(a => !string.IsNullOrEmpty(a.SiblingLocationCode)).ToList();
                foreach (var assignment in siblingsWithSchools)
                {
                    Console.WriteLine($"{assignment.StudentName}: Sibling at {assignment.SiblingLocationCode}, Assigned to {assignment.AssignedSchool} {(assignment.IsAtSiblingSchool ? "(SAME SCHOOL)" : "(DIFFERENT SCHOOL)")}");
                }
                
                Console.WriteLine();
                
                var siblingGroupsData = siblingGroups.Where(g => g.Count() > 1).ToList();
                Console.WriteLine($"Total Sibling Groups: {siblingGroupsData.Count}");
                
                foreach (var group in siblingGroupsData)
                {
                    var assignments = group.ToList();
                    var schools = assignments.Select(a => a.AssignedSchool).Distinct().ToList();
                    var kept_together = schools.Count == 1;
                    
                    Console.WriteLine($"  {assignments.First().SiblingGroupId}: {assignments.Count} siblings, {(kept_together ? "KEPT TOGETHER" : "SEPARATED")} at {string.Join(", ", schools)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("\n================================================================");
            Console.ReadKey();
        }

        private static string GetGradeFromStudent(Models.StudentAssignment assignment)
        {
            // Get grade from mock data by matching student name
            var students = MockDataService.GetMockStudents();
            var student = students.FirstOrDefault(s => s.FullName == assignment.StudentName);
            return student?.Grade ?? "XX";
        }
    }
}