using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SchoolChoice.DeferredAcceptance.Models;

namespace SchoolChoice.DeferredAcceptance.Services
{
    public class MockDeferredAcceptanceService
    {
        private readonly ILogger<MockDeferredAcceptanceService> _logger;

        public MockDeferredAcceptanceService(ILogger<MockDeferredAcceptanceService> logger)
        {
            _logger = logger;
        }

        public async Task<DeferredAcceptanceResult> RunDeferredAcceptanceAsync(int schoolYear, string grade = null)
        {
            var startTime = DateTime.Now;
            _logger.LogInformation($"Starting MOCK Deferred Acceptance Algorithm for school year {schoolYear}");

            try
            {
                var students = MockDataService.GetMockStudents();
                var schools = MockDataService.GetMockSchools();

                if (!string.IsNullOrEmpty(grade))
                {
                    students = students.Where(s => s.Grade == grade).ToList();
                }

                ApplySiblingPreferenceBoosts(students);

                // Ensure all students have some school preferences
                EnsureAllStudentsHavePreferences(students, schools);

                _logger.LogInformation($"Loaded {students.Count} mock students");
                _logger.LogInformation($"Loaded {schools.Count} mock schools");

                // Group siblings
                var siblingGroups = GroupSiblings(students);
                _logger.LogInformation($"Identified {siblingGroups.Count} sibling groups");

                var result = await RunAlgorithmWithSiblingsAsync(siblingGroups, schools);
                result.ProcessingTime = DateTime.Now - startTime;

                _logger.LogInformation($"Mock algorithm completed in {result.TotalRounds} rounds");
                _logger.LogInformation($"Matched {result.Assignments.Count} students");
                _logger.LogInformation($"Unmatched {result.UnmatchedStudents.Count} students");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running mock deferred acceptance algorithm");
                throw;
            }
        }

        private void ApplySiblingPreferenceBoosts(List<Student> students)
        {
            foreach (var student in students.Where(s => s.HasSiblingAtSchool))
            {
                var siblingSchool = student.SiblingLocationCode;

                if (student.SchoolPreferences.Contains(siblingSchool))
                {
                    // Move sibling's school to top of preference list
                    student.SchoolPreferences.Remove(siblingSchool);
                    student.SchoolPreferences.Insert(0, siblingSchool);

                    _logger.LogInformation($"Boosted preference for {student.FullName} to attend {siblingSchool} (sibling's school)");
                }
                else
                {
                    // Add sibling's school as top preference if it wasn't in their list
                    student.SchoolPreferences.Insert(0, siblingSchool);

                    _logger.LogInformation($"Added sibling's school {siblingSchool} as top preference for {student.FullName}");
                }
            }
        }

        private void EnsureAllStudentsHavePreferences(List<Student> students, Dictionary<string, School> schools)
        {
            // Ensure all students have at least some school preferences
            var allSchoolCodes = schools.Values.Select(s => s.LocationCode).Distinct().ToList();
            
            foreach (var student in students)
            {
                if (student.SchoolPreferences == null || student.SchoolPreferences.Count == 0)
                {
                    // If student has no preferences, add all available schools for their grade
                    student.SchoolPreferences = allSchoolCodes
                        .Where(code => schools.ContainsKey($"{code}_{student.Grade}"))
                        .ToList();
                    
                    _logger.LogWarning($"Student {student.FullName} had no preferences - added all available schools");
                }
            }
        }

        private List<List<Student>> GroupSiblings(List<Student> students)
        {
            var siblingGroups = new List<List<Student>>();
            var processed = new HashSet<int>();

            foreach (var student in students)
            {
                if (processed.Contains(student.ApplicationId))
                    continue;

                // Store original sibling group ID
                student.OriginalSiblingGroupId = student.SiblingGroupId;

                var siblingGroup = new List<Student> { student };
                processed.Add(student.ApplicationId);

                // Find siblings using SIB_PURLS_ID
                if (student.HasSibling && !string.IsNullOrEmpty(student.SiblingPurlsId))
                {
                    var siblings = students.Where(s =>
                        s.ApplicationId != student.ApplicationId &&
                        !processed.Contains(s.ApplicationId) &&
                        !string.IsNullOrEmpty(s.SiblingPurlsId) &&
                        s.SiblingPurlsId == student.SiblingPurlsId)
                        .ToList();

                    foreach (var sibling in siblings)
                    {
                        sibling.OriginalSiblingGroupId = sibling.SiblingGroupId;
                        siblingGroup.Add(sibling);
                        processed.Add(sibling.ApplicationId);
                    }
                }

                siblingGroups.Add(siblingGroup);
            }

            return siblingGroups;
        }

        private async Task<DeferredAcceptanceResult> RunAlgorithmWithSiblingsAsync(List<List<Student>> siblingGroups, Dictionary<string, School> schools)
        {
            var result = new DeferredAcceptanceResult();
            var round = 0;
            var hasChanges = true;

            _logger.LogInformation("Starting deferred acceptance rounds with sibling constraints");

            while (hasChanges && round < 1000) 
            {
                round++;
                hasChanges = false;
                var unmatchedGroups = siblingGroups.Count(g => g.Any(s => string.IsNullOrEmpty(s.AssignedSchool)));
                _logger.LogDebug($"Round {round}: Processing {unmatchedGroups} unmatched sibling groups");

                // Phase 1: Each unmatched sibling group applies to next preferred school
                foreach (var siblingGroup in siblingGroups.Where(g => g.Any(s => string.IsNullOrEmpty(s.AssignedSchool))))
                {
                    var applied = ProcessSiblingGroupApplication(siblingGroup, schools);
                    if (applied)
                    {
                        hasChanges = true;
                    }
                }

                // Phase 2: Each school processes applications and makes tentative assignments
                foreach (var school in schools.Values)
                {
                    ProcessSchoolAssignments(school);
                }

                // Check if we've reached a stable state
                var allStudents = siblingGroups.SelectMany(g => g).ToList();
                var unmatchedStudents = allStudents.Where(s => string.IsNullOrEmpty(s.AssignedSchool)).ToList();
                
                if (unmatchedStudents.Count == 0)
                {
                    _logger.LogInformation($"All students matched after {round} rounds");
                    hasChanges = false;
                    break;
                }

                // Check if any unmatched students have remaining preferences
                var studentsWithRemainingPreferences = unmatchedStudents.Where(s => 
                    s.CurrentPreferenceIndex < s.SchoolPreferences.Count).ToList();
                    
                if (studentsWithRemainingPreferences.Count == 0)
                {
                    _logger.LogInformation($"Algorithm converged after {round} rounds - no more preferences to try");
                    _logger.LogInformation($"Final result: {unmatchedStudents.Count} students remain unmatched due to capacity constraints");
                    hasChanges = false;
                    break;
                }
            }

            result.TotalRounds = round;

            foreach (var school in schools.Values)
            {
                foreach (var student in school.TentativeAssignments)
                {
                    result.Assignments.Add(new StudentAssignment
                    {
                        ApplicationId = student.ApplicationId,
                        DistrictId = student.DistrictId,
                        StudentName = student.FullName,
                        AssignedSchool = student.AssignedSchool,
                        SchoolName = school.SchoolName,
                        PreferenceRank = student.SchoolPreferences.IndexOf(student.AssignedSchool) + 1,
                        PriorityPoints = student.PriorityPoints,
                        HasSibling = student.HasSibling,
                        SiblingGroupId = student.SiblingGroupId,
                        SiblingLocationCode = student.SiblingLocationCode,
                        IsAtSiblingSchool = student.HasSiblingAtSchool && student.AssignedSchool == student.SiblingLocationCode
                    });
                }
            }

            // Handle unmatched students by assigning them to randomly available schools
            var allStudents2 = siblingGroups.SelectMany(g => g).ToList();
            var remainingUnmatchedStudents = allStudents2.Where(s => string.IsNullOrEmpty(s.AssignedSchool)).ToList();
            
            if (remainingUnmatchedStudents.Count > 0)
            {
                _logger.LogInformation($"Assigning {remainingUnmatchedStudents.Count} unmatched students to randomly available schools");
                AssignUnmatchedStudentsToAvailableSchools(remainingUnmatchedStudents, schools, result);
            }

            result.UnmatchedStudents = allStudents2.Where(s => string.IsNullOrEmpty(s.AssignedSchool)).ToList();

            // Log final statistics
            _logger.LogInformation($"Final matching results:");
            _logger.LogInformation($"- Total students: {allStudents2.Count}");
            _logger.LogInformation($"- Successfully matched: {result.Assignments.Count}");
            _logger.LogInformation($"- Unmatched: {result.UnmatchedStudents.Count}");
            
            if (result.UnmatchedStudents.Count > 0)
            {
                _logger.LogWarning($"Students that could not be matched to any school:");
                foreach (var student in result.UnmatchedStudents)
                {
                    _logger.LogWarning($"- {student.FullName} (ID: {student.ApplicationId}) - no schools available for grade {student.Grade}");
                }
            }

            return result;
        }

        private bool ProcessSiblingGroupApplication(List<Student> siblingGroup, Dictionary<string, School> schools)
        {
            var unmatchedSiblings = siblingGroup.Where(s => string.IsNullOrEmpty(s.AssignedSchool)).ToList();

            if (unmatchedSiblings.Count == 0)
                return false;

            // Check if siblings need to be kept together
            var shouldStayTogether = ShouldSiblingsStayTogether(unmatchedSiblings);

            if (shouldStayTogether && unmatchedSiblings.Count > 1)
            {
                return ProcessSiblingGroupTogether(unmatchedSiblings, schools);
            }
            else
            {
                // Handle each sibling individually
                var hasApplications = false;
                foreach (var student in unmatchedSiblings)
                {
                    if (ProcessIndividualStudent(student, schools))
                    {
                        hasApplications = true;
                    }
                }
                return hasApplications;
            }
        }

        private bool ShouldSiblingsStayTogether(List<Student> siblings)
        {
            // Keep siblings together if they have the same SiblingPurlsId and are both applying
            return siblings.Count > 1 && siblings.All(s => !string.IsNullOrEmpty(s.SiblingPurlsId));
        }

        private bool ProcessSiblingGroupTogether(List<Student> siblings, Dictionary<string, School> schools)
        {
            // Find the next common preference among siblings
            var minPreferenceIndex = siblings.Min(s => s.CurrentPreferenceIndex);
            
            // Check if any sibling has exhausted their preferences
            if (siblings.Any(s => s.CurrentPreferenceIndex >= s.SchoolPreferences.Count))
            {
                // Fall back to individual processing
                _logger.LogWarning($"Sibling group {siblings.First().SiblingGroupId} cannot stay together - processing individually");
                foreach (var sibling in siblings)
                {
                    sibling.SiblingPurlsId = null; // Remove sibling constraint
                }
                return false;
            }

            // Try to find a school that can accommodate all siblings
            while (minPreferenceIndex < siblings.Min(s => s.SchoolPreferences.Count))
            {
                var canAccommodateAll = true;
                var schoolsForSiblings = new List<School>();
                var preferredSchool = siblings.First().SchoolPreferences[minPreferenceIndex];

                foreach (var sibling in siblings)
                {
                    if (sibling.CurrentPreferenceIndex >= sibling.SchoolPreferences.Count ||
                        sibling.SchoolPreferences[sibling.CurrentPreferenceIndex] != preferredSchool)
                    {
                        canAccommodateAll = false;
                        break;
                    }

                    var schoolKey = $"{preferredSchool}_{sibling.Grade}";
                    if (schools.ContainsKey(schoolKey))
                    {
                        schoolsForSiblings.Add(schools[schoolKey]);
                    }
                    else
                    {
                        canAccommodateAll = false;
                        break;
                    }
                }

                if (canAccommodateAll)
                {
                    // Apply all siblings to their respective grade schools
                    for (int i = 0; i < siblings.Count; i++)
                    {
                        var sibling = siblings[i];
                        var school = schoolsForSiblings[i];
                    
                        if (!string.IsNullOrEmpty(sibling.AssignedSchool))
                        {
                            var currentSchool = schools.Values.FirstOrDefault(s => s.LocationCode == sibling.AssignedSchool);
                            currentSchool?.TentativeAssignments.Remove(sibling);
                        }
                        
                        school.TentativeAssignments.Add(sibling);
                        _logger.LogDebug($"Sibling {sibling.ApplicationId} applies to {preferredSchool} with sibling constraint");
                    }

                    // Move to next preference for all siblings
                    foreach (var sibling in siblings)
                    {
                        sibling.CurrentPreferenceIndex++;
                    }
                    
                    return true;
                }
                else
                {
                    // Move to next preference and try again
                    minPreferenceIndex++;
                    foreach (var sibling in siblings)
                    {
                        if (sibling.CurrentPreferenceIndex < sibling.SchoolPreferences.Count)
                        {
                            sibling.CurrentPreferenceIndex++;
                        }
                    }
                }
            }

            return false;
        }

        private bool TryAssignIndividualStudentByPreferences(Student student, Dictionary<string, School> schools, DeferredAcceptanceResult result)
        {
            while (student.CurrentPreferenceIndex < student.SchoolPreferences.Count)
            {
                var preferredSchool = student.SchoolPreferences[student.CurrentPreferenceIndex];
                var schoolKey = $"{preferredSchool}_{student.Grade}";
                
                if (schools.ContainsKey(schoolKey))
                {
                    var school = schools[schoolKey];
                    if (school.AvailableSeats > school.TentativeAssignments.Count)
                    {
                        school.TentativeAssignments.Add(student);
                        student.AssignedSchool = school.LocationCode;
                        
                        result.Assignments.Add(new StudentAssignment
                        {
                            ApplicationId = student.ApplicationId,
                            DistrictId = student.DistrictId,
                            StudentName = student.FullName,
                            AssignedSchool = student.AssignedSchool,
                            SchoolName = school.SchoolName,
                            PreferenceRank = student.SchoolPreferences.IndexOf(student.AssignedSchool) + 1,
                            PriorityPoints = student.PriorityPoints,
                            HasSibling = student.HasSibling,
                            SiblingGroupId = student.SiblingGroupId,
                            SiblingLocationCode = student.SiblingLocationCode,
                            IsAtSiblingSchool = student.HasSiblingAtSchool && student.AssignedSchool == student.SiblingLocationCode
                        });
                        
                        _logger.LogInformation($"Assigned individual student {student.FullName} to preferred school {preferredSchool}");
                        return true;
                    }
                }
                
                student.CurrentPreferenceIndex++;
            }
            
            return false;
        }

        private bool ProcessIndividualStudent(Student student, Dictionary<string, School> schools)
        {
            if (student.CurrentPreferenceIndex < student.SchoolPreferences.Count)
            {
                var preferredSchool = student.SchoolPreferences[student.CurrentPreferenceIndex];
                var schoolKey = $"{preferredSchool}_{student.Grade}";

                if (schools.ContainsKey(schoolKey))
                {
                    var school = schools[schoolKey];
                    
                    // Remove from current assignment if any
                    if (!string.IsNullOrEmpty(student.AssignedSchool))
                    {
                        var currentSchool = schools.Values.FirstOrDefault(s => s.LocationCode == student.AssignedSchool);
                        currentSchool?.TentativeAssignments.Remove(student);
                    }
                    
                    school.TentativeAssignments.Add(student);
                    _logger.LogDebug($"Student {student.ApplicationId} applies to {preferredSchool}");
                    
                    student.CurrentPreferenceIndex++;
                    return true;
                }
                else
                {
                    // School doesn't exist for this grade, move to next preference
                    _logger.LogDebug($"School {preferredSchool} doesn't support grade {student.Grade}");
                    student.CurrentPreferenceIndex++;
                    return ProcessIndividualStudent(student, schools); // Try next preference immediately
                }
            }

            return false;
        }

        private void ProcessSchoolAssignments(School school)
        {
            // Clear rejected students from previous round
            school.RejectedStudents.Clear();

            // If no applications, nothing to process
            if (school.TentativeAssignments.Count == 0)
                return;

            // Group by sibling groups
            var siblingGroups = school.TentativeAssignments
                .GroupBy(s => s.SiblingGroupId ?? s.ApplicationId.ToString())
                .ToList();

            // Sort sibling groups by highest priority member
            var sortedGroups = siblingGroups
                .OrderByDescending(g => g.Max(s => s.PriorityPoints))
                .ThenByDescending(g => g.Max(s => s.SubPriorityPoints))
                .ThenBy(g => g.Min(s => s.ApplicationId)) // Tie-breaker for consistency
                .ToList();

            var acceptedStudents = new List<Student>();
            var rejectedStudents = new List<Student>();

            foreach (var group in sortedGroups)
            {
                var groupStudents = group.ToList();
                
                // Check if we can fit this entire group
                if (acceptedStudents.Count + groupStudents.Count <= school.AvailableSeats)
                {
                    // Accept entire group
                    acceptedStudents.AddRange(groupStudents);
                    foreach (var student in groupStudents)
                    {
                        student.AssignedSchool = school.LocationCode;
                    }
                }
                else
                {
                    // Reject entire group to keep siblings together
                    rejectedStudents.AddRange(groupStudents);
                    foreach (var student in groupStudents)
                    {
                        student.AssignedSchool = null;
                    }
                }
            }

            // Update school's tentative assignments to only accepted students
            school.TentativeAssignments.Clear();
            school.TentativeAssignments.AddRange(acceptedStudents);
            
            // Set rejected students
            school.RejectedStudents.AddRange(rejectedStudents);

            _logger.LogDebug($"School {school.LocationCode} (Grade {school.Grade}) accepted {acceptedStudents.Count}/{school.AvailableSeats} students, rejected {rejectedStudents.Count} students");
        }

        private void AssignUnmatchedStudentsToAvailableSchools(List<Student> unmatchedStudents, Dictionary<string, School> schools, DeferredAcceptanceResult result)
        {
            var random = new Random();
            
            // Group unmatched students by sibling groups to maintain family unity
            var siblingGroups = unmatchedStudents
                .GroupBy(s => s.SiblingGroupId ?? s.ApplicationId.ToString())
                .ToList();

            foreach (var group in siblingGroups)
            {
                var students = group.ToList();
                
                if (students.Count > 1 && ShouldSiblingsStayTogether(students))
                {
                    // Try to find a school location that can accommodate all siblings
                    AssignSiblingGroupToAvailableSchool(students, schools, result, random);
                }
                else
                {
                    // For individual students, try their remaining preferences first
                    foreach (var student in students)
                    {
                        bool studentMatched = TryAssignIndividualStudentByPreferences(student, schools, result);
                        
                        if (!studentMatched)
                        {
                            // If still unmatched after all preferences, assign randomly
                            AssignIndividualStudentToAvailableSchool(student, schools, result, random);
                        }
                    }
                }
            }
        }

        private void AssignSiblingGroupToAvailableSchool(List<Student> siblings, Dictionary<string, School> schools, DeferredAcceptanceResult result, Random random)
        {
            // Find school locations that have available capacity for all siblings
            var availableSchoolLocations = schools.Values
                .GroupBy(s => s.LocationCode)
                .Where(locationGroup => siblings.All(sibling => 
                    locationGroup.Any(school => 
                        school.Grade == sibling.Grade && 
                        school.AvailableSeats > school.TentativeAssignments.Count)))
                .Select(g => g.Key)
                .ToList();

            if (availableSchoolLocations.Count == 0)
            {
                _logger.LogWarning($"No school location can accommodate all siblings in group {siblings.First().SiblingGroupId}. Assigning individually.");
                // Fall back to individual assignment
                foreach (var student in siblings)
                {
                    AssignIndividualStudentToAvailableSchool(student, schools, result, random);
                }
                return;
            }

            // Randomly select a school location
            var selectedLocation = availableSchoolLocations[random.Next(availableSchoolLocations.Count)];
            
            // Assign each sibling to the appropriate grade school at this location
            var allAssigned = true;
            var assignedStudents = new List<Student>();

            foreach (var sibling in siblings)
            {
                var schoolKey = $"{selectedLocation}_{sibling.Grade}";
                if (schools.ContainsKey(schoolKey))
                {
                    var school = schools[schoolKey];
                    if (school.AvailableSeats > school.TentativeAssignments.Count)
                    {
                        school.TentativeAssignments.Add(sibling);
                        sibling.AssignedSchool = selectedLocation;
                        assignedStudents.Add(sibling);
                        
                        result.Assignments.Add(new StudentAssignment
                        {
                            ApplicationId = sibling.ApplicationId,
                            DistrictId = sibling.DistrictId,
                            StudentName = sibling.FullName,
                            AssignedSchool = sibling.AssignedSchool,
                            SchoolName = school.SchoolName,
                            PreferenceRank = -1, // Not from preference list
                            PriorityPoints = sibling.PriorityPoints,
                            HasSibling = sibling.HasSibling,
                            SiblingGroupId = sibling.SiblingGroupId,
                            SiblingLocationCode = sibling.SiblingLocationCode,
                            IsAtSiblingSchool = sibling.HasSiblingAtSchool && sibling.AssignedSchool == sibling.SiblingLocationCode
                        });
                        
                        _logger.LogInformation($"Assigned sibling {sibling.FullName} to random available school {selectedLocation}");
                    }
                    else
                    {
                        allAssigned = false;
                        break;
                    }
                }
                else
                {
                    allAssigned = false;
                    break;
                }
            }

            if (!allAssigned)
            {
                // Rollback assignments if we couldn't assign all siblings
                foreach (var student in assignedStudents)
                {
                    var schoolKey = $"{selectedLocation}_{student.Grade}";
                    if (schools.ContainsKey(schoolKey))
                    {
                        schools[schoolKey].TentativeAssignments.Remove(student);
                        student.AssignedSchool = null;
                    }
                    // Remove from results
                    result.Assignments.RemoveAll(a => a.ApplicationId == student.ApplicationId);
                }
                
                // Try individual assignment
                foreach (var student in siblings)
                {
                    AssignIndividualStudentToAvailableSchool(student, schools, result, random);
                }
            }
        }

        private void AssignIndividualStudentToAvailableSchool(Student student, Dictionary<string, School> schools, DeferredAcceptanceResult result, Random random)
        {
            // Find all schools that serve this student's grade and have available seats
            var availableSchools = schools.Values
                .Where(s => s.Grade == student.Grade && s.AvailableSeats > s.TentativeAssignments.Count)
                .ToList();

            if (availableSchools.Count == 0)
            {
                _logger.LogWarning($"No available schools found for student {student.FullName} in grade {student.Grade}");
                return;
            }

            // Randomly select a school
            var selectedSchool = availableSchools[random.Next(availableSchools.Count)];
            
            // Assign the student
            selectedSchool.TentativeAssignments.Add(student);
            student.AssignedSchool = selectedSchool.LocationCode;
            
            result.Assignments.Add(new StudentAssignment
            {
                ApplicationId = student.ApplicationId,
                DistrictId = student.DistrictId,
                StudentName = student.FullName,
                AssignedSchool = student.AssignedSchool,
                SchoolName = selectedSchool.SchoolName,
                PreferenceRank = -1, // Not from preference list
                PriorityPoints = student.PriorityPoints,
                HasSibling = student.HasSibling,
                SiblingGroupId = student.SiblingGroupId,
                SiblingLocationCode = student.SiblingLocationCode,
                IsAtSiblingSchool = student.HasSiblingAtSchool && student.AssignedSchool == student.SiblingLocationCode
            });
            
            _logger.LogInformation($"Assigned student {student.FullName} to random available school {selectedSchool.LocationCode} ({selectedSchool.SchoolName})");
        }
    }
}