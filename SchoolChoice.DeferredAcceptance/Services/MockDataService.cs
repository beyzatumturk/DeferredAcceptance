using System;
using System.Collections.Generic;
using System.Linq;
using SchoolChoice.DeferredAcceptance.Models;

namespace SchoolChoice.DeferredAcceptance.Services
{
    public static class MockDataService
    {
        public static List<Student> GetMockStudents()
        {
            return new List<Student>
            {
                new Student
                {
                    ApplicationId = 1001,
                    DistrictId = "SMITH001",
                    StudentFirstName = "John",
                    StudentLastName = "Smith",
                    PriorityPoints = 85,
                    SubPriorityPoints = 85001,
                    Grade = "03",
                    SiblingDistrictId = "SMITH_FAMILY",
                    SiblingPurlsId = "SMITH_FAMILY",
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM003", "ELEM002" }
                },
                new Student
                {
                    ApplicationId = 1002,
                    DistrictId = "SMITH002",
                    StudentFirstName = "Jane",
                    StudentLastName = "Smith",
                    PriorityPoints = 85,
                    SubPriorityPoints = 85002,
                    Grade = "06",
                    SiblingDistrictId = "SMITH_FAMILY",
                    SiblingPurlsId = "SMITH_FAMILY",
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM003", "ELEM002" }
                },
                
                new Student
                {
                    ApplicationId = 2001,
                    DistrictId = "JOHNSON001",
                    StudentFirstName = "Mike",
                    StudentLastName = "Johnson",
                    PriorityPoints = 90,
                    SubPriorityPoints = 90001,
                    Grade = "09",
                    SiblingDistrictId = "JOHNSON_FAMILY",
                    SiblingPurlsId = "JOHNSON_FAMILY",
                    SchoolPreferences = new List<string> { "HIGH001", "ELEM003", "HIGH002" }
                },
                new Student
                {
                    ApplicationId = 2002,
                    DistrictId = "JOHNSON002",
                    StudentFirstName = "Sarah",
                    StudentLastName = "Johnson",
                    PriorityPoints = 90,
                    SubPriorityPoints = 90002,
                    Grade = "11",
                    SiblingDistrictId = "JOHNSON_FAMILY",
                    SiblingPurlsId = "JOHNSON_FAMILY",
                    SchoolPreferences = new List<string> { "HIGH001", "ELEM003", "HIGH002" }
                },
                
                new Student
                {
                    ApplicationId = 3001,
                    DistrictId = "GARCIA001",
                    StudentFirstName = "Carlos",
                    StudentLastName = "Garcia",
                    PriorityPoints = 88,
                    SubPriorityPoints = 88001,
                    Grade = "05",
                    SiblingDistrictId = "GARCIA_FAMILY",
                    SiblingPurlsId = "GARCIA_FAMILY",
                    SchoolPreferences = new List<string> { "ELEM002", "ELEM001", "ELEM003" }
                },
                new Student
                {
                    ApplicationId = 3002,
                    DistrictId = "GARCIA002",
                    StudentFirstName = "Maria",
                    StudentLastName = "Garcia",
                    PriorityPoints = 88,
                    SubPriorityPoints = 88002,
                    Grade = "09",
                    SiblingDistrictId = "GARCIA_FAMILY",
                    SiblingPurlsId = "GARCIA_FAMILY",
                    SchoolPreferences = new List<string> { "ELEM002", "ELEM001", "ELEM003" }
                },
                
                new Student
                {
                    ApplicationId = 4001,
                    DistrictId = "RODRIGUEZ001",
                    StudentFirstName = "Luis",
                    StudentLastName = "Rodriguez",
                    PriorityPoints = 82,
                    SubPriorityPoints = 82001,
                    Grade = "04",
                    SiblingDistrictId = "RODRIGUEZ_FAMILY",
                    SiblingPurlsId = "RODRIGUEZ_FAMILY",
                    SiblingLocationCode = "ELEM003", 
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM002", "ELEM003" }
                },
                new Student
                {
                    ApplicationId = 4002,
                    DistrictId = "RODRIGUEZ002",
                    StudentFirstName = "Ana",
                    StudentLastName = "Rodriguez",
                    PriorityPoints = 82,
                    SubPriorityPoints = 82002,
                    Grade = "07",
                    SiblingDistrictId = "RODRIGUEZ_FAMILY",
                    SiblingPurlsId = "RODRIGUEZ_FAMILY",
                    SiblingLocationCode = "ELEM003", 
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM002", "ELEM003" }
                },
                
                new Student
                {
                    ApplicationId = 5001,
                    DistrictId = "TORRES001",
                    StudentFirstName = "Diego",
                    StudentLastName = "Torres",
                    PriorityPoints = 78,
                    SubPriorityPoints = 78001,
                    Grade = "08",
                    SiblingLocationCode = "HIGH002", 
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM003" }
                },
                
                new Student
                {
                    ApplicationId = 6001,
                    DistrictId = "WILLIAMS001",
                    StudentFirstName = "Alex",
                    StudentLastName = "Williams",
                    PriorityPoints = 80,
                    SubPriorityPoints = 80001,
                    Grade = "07",
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM003", "ELEM002" }
                },
                new Student
                {
                    ApplicationId = 7001,
                    DistrictId = "BROWN001",
                    StudentFirstName = "Emma",
                    StudentLastName = "Brown",
                    PriorityPoints = 75,
                    SubPriorityPoints = 75001,
                    Grade = "05",
                    SchoolPreferences = new List<string> { "ELEM002", "ELEM001", "ELEM003" }
                },
                new Student
                {
                    ApplicationId = 8001,
                    DistrictId = "DAVIS001",
                    StudentFirstName = "Ryan",
                    StudentLastName = "Davis",
                    PriorityPoints = 92,
                    SubPriorityPoints = 92001,
                    Grade = "10",
                    SchoolPreferences = new List<string> { "HIGH001", "ELEM003", "HIGH002" }
                },
                new Student
                {
                    ApplicationId = 9001,
                    DistrictId = "MILLER001",
                    StudentFirstName = "Sophia",
                    StudentLastName = "Miller",
                    PriorityPoints = 77,
                    SubPriorityPoints = 77001,
                    Grade = "01",
                    SchoolPreferences = new List<string> { "ELEM001", "ELEM002", "ELEM003" }
                }
            };
        }

        public static Dictionary<string, School> GetMockSchools()
        {
            return new Dictionary<string, School>
            {
                ["ELEM001_01"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "01",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM001_03"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "03",
                    TotalSeats = 30,
                    AvailableSeats = 30,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM001_04"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "04",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM001_05"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "05",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM001_06"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "06",
                    TotalSeats = 28,
                    AvailableSeats = 28,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM001_07"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "07",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM001_08"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "08",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },

                ["ELEM002_01"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "01",
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05" }
                },
                ["ELEM002_03"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "03",
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05" }
                },
                ["ELEM002_04"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "04",
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05" }
                },
                ["ELEM002_05"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "05",
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05" }
                },

                ["HIGH001_09"] = new School
                {
                    LocationCode = "HIGH001",
                    SchoolName = "Roosevelt High School",
                    Grade = "09",
                    TotalSeats = 40,
                    AvailableSeats = 40,
                    GradesServed = new List<string> { "09", "10", "11", "12" }
                },
                ["HIGH001_10"] = new School
                {
                    LocationCode = "HIGH001",
                    SchoolName = "Roosevelt High School",
                    Grade = "10",
                    TotalSeats = 35,
                    AvailableSeats = 35,
                    GradesServed = new List<string> { "09", "10", "11", "12" }
                },
                ["HIGH001_11"] = new School
                {
                    LocationCode = "HIGH001",
                    SchoolName = "Roosevelt High School",
                    Grade = "11",
                    TotalSeats = 35,
                    AvailableSeats = 35,
                    GradesServed = new List<string> { "09", "10", "11", "12" }
                },

                ["ELEM003_01"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "01",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_03"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "03",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_04"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "04",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_05"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "05",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_06"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "06",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_07"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "07",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_08"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "08",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_09"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "09",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_10"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "10",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM003_11"] = new School
                {
                    LocationCode = "ELEM003",
                    SchoolName = "Jefferson Academy",
                    Grade = "11",
                    TotalSeats = 15,
                    AvailableSeats = 15,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },

                ["HIGH002_08"] = new School
                {
                    LocationCode = "HIGH002",
                    SchoolName = "Kennedy High School",
                    Grade = "08",
                    TotalSeats = 30,
                    AvailableSeats = 30,
                    GradesServed = new List<string> { "08", "09", "10", "11", "12" }
                },
                ["HIGH002_09"] = new School
                {
                    LocationCode = "HIGH002",
                    SchoolName = "Kennedy High School",
                    Grade = "09",
                    TotalSeats = 30,
                    AvailableSeats = 30,
                    GradesServed = new List<string> { "08", "09", "10", "11", "12" }
                },
                ["HIGH002_10"] = new School
                {
                    LocationCode = "HIGH002",
                    SchoolName = "Kennedy High School",
                    Grade = "10",
                    TotalSeats = 30,
                    AvailableSeats = 30,
                    GradesServed = new List<string> { "08", "09", "10", "11", "12" }
                },
                ["ELEM001_09"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary",
                    Grade = "09",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM001_10"] = new School
                {
                    LocationCode = "ELEM001", 
                    SchoolName = "Washington Elementary",
                    Grade = "10",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["ELEM001_11"] = new School
                {
                    LocationCode = "ELEM001",
                    SchoolName = "Washington Elementary", 
                    Grade = "11",
                    TotalSeats = 25,
                    AvailableSeats = 25,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },

                ["ELEM002_06"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "06", 
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM002_07"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "07",
                    TotalSeats = 20,
                    AvailableSeats = 20, 
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM002_08"] = new School
                {
                    LocationCode = "ELEM002",
                    SchoolName = "Lincoln Elementary",
                    Grade = "08",
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08" }
                },
                ["ELEM002_09"] = new School
                {
                    LocationCode = "ELEM002", 
                    SchoolName = "Lincoln Elementary",
                    Grade = "09",
                    TotalSeats = 20,
                    AvailableSeats = 20,
                    GradesServed = new List<string> { "KG", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" }
                },
                ["HIGH002_11"] = new School
                {
                    LocationCode = "HIGH002",
                    SchoolName = "Kennedy High School",
                    Grade = "11",
                    TotalSeats = 30,
                    AvailableSeats = 30,
                    GradesServed = new List<string> { "08", "09", "10", "11", "12" }
                }
            };
        }
    }
}