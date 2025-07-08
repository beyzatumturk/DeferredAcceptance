using System;
using System.Collections.Generic;

namespace SchoolChoice.DeferredAcceptance.Models
{
    public class Student
    {
        public int ApplicationId { get; set; }
        public string DistrictId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int PriorityPoints { get; set; }
        public long SubPriorityPoints { get; set; }
        public string Grade { get; set; }
        public List<string> SchoolPreferences { get; set; } = new List<string>();
        public string AssignedSchool { get; set; }
        public bool IsMatched { get; set; }
        public int CurrentPreferenceIndex { get; set; }
        public string SiblingDistrictId { get; set; }
        public string SiblingPurlsId { get; set; }
        public string SiblingLocationCode { get; set; }
        public bool HasSibling => !string.IsNullOrEmpty(SiblingPurlsId);
        public bool HasSiblingAtSchool => !string.IsNullOrEmpty(SiblingLocationCode);
        public string SiblingGroupId => HasSibling ? SiblingPurlsId : ApplicationId.ToString();
        public string FullName => $"{StudentFirstName} {StudentLastName}";
        
        public bool HasFallenBackToIndividual { get; set; } = false;
        public string OriginalSiblingGroupId { get; set; }
    }

    public class School
    {
        public string LocationCode { get; set; }
        public string SchoolName { get; set; }
        public string Grade { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<Student> TentativeAssignments { get; set; } = new List<Student>();
        public List<Student> RejectedStudents { get; set; } = new List<Student>();
        public List<string> GradesServed { get; set; } = new List<string>();
    }

    public class DeferredAcceptanceResult
    {
        public List<StudentAssignment> Assignments { get; set; } = new List<StudentAssignment>();
        public List<Student> UnmatchedStudents { get; set; } = new List<Student>();
        public int TotalRounds { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public List<string> ProcessingLog { get; set; } = new List<string>();
    }

    public class StudentAssignment
    {
        public int ApplicationId { get; set; }
        public string DistrictId { get; set; }
        public string StudentName { get; set; }
        public string AssignedSchool { get; set; }
        public string SchoolName { get; set; }
        public int PreferenceRank { get; set; }
        public int PriorityPoints { get; set; }
        public bool HasSibling { get; set; }
        public string SiblingGroupId { get; set; }
        public string SiblingLocationCode { get; set; }
        public bool IsAtSiblingSchool { get; set; }
    }
}