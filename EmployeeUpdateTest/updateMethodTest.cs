using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTest
{
    [TestClass]
    public class updateMethodTest
    {
        [TestMethod]
        public void IsTranformFromStringToDictCorrect()
        {
            // arrange
            string updateCase = "Id:1 Firstname:Jack Lastname:Sparrow salARY:100.50"; //Example of command with update info
            Dictionary<string, string> actual = new Dictionary<string, string>();
            // act
            actual = updateCase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) //Split string into Dictionary
                        .Select(part => part.Split(':'))
                        .ToDictionary(split => split[0], split => split[1]);
            actual = actual.ToDictionary(k => k.Key.ToLower(), k => k.Value); //Bring keys of dict to lowercase
            Dictionary<string, string> expected = new Dictionary<string, string>();
            expected.Add("id", "1");
            expected.Add("firstname", "Jack");
            expected.Add("lastname", "Sparrow");
            expected.Add("salary", "100.50");  //write dict with expected results
            string actualString = "";
            foreach (KeyValuePair<string, string> keyValues in actual)
            {
                actualString += keyValues.Key + " : " + keyValues.Value + ", "; //transform actual dict in string
            }
            actualString.TrimEnd(',', ' ');
            string expectedString = "";
            foreach (KeyValuePair<string, string> keyValues in expected)
            {
                expectedString += keyValues.Key + " : " + keyValues.Value + ", "; //transform expected dict in string
            }
            expectedString.TrimEnd(',', ' ');
            // assert
            Assert.AreEqual(expectedString, actualString); //Compare strings
        }

        [TestMethod]
        public void IsPasteFromDictToClassCorrect()
        {
            //arrange
            List<Employee> currentEmployees = new List<Employee>(); //Initiate list of objects
            Employee newEmployee = new Employee(1, "John", "Doe", 100.50m); //Initiate start object
            currentEmployees.Add(newEmployee); //add start object to list
            Dictionary<string, string> values = new Dictionary<string, string>(); //Initiate dict
            values.Add("id", "1"); //Add id to dict
            values.Add("firstname", "Jack"); //Add firstname to dict
            values.Add("lastname", "Sparrow"); //Add lastname to dict
            values.Add("salary", "900.90"); //Add salary to dict
            int id = int.Parse(values["id"]); //Parse id from string to int
            Employee actual = currentEmployees.FirstOrDefault(x => x.Id == id); // Initiate actual object as object from list with same id's.
            if (actual != null) //check for existing object
            {
                foreach (String key in values.Keys) //cycle for every key in dict
                {
                    if (key != null) //check for info in dict
                    {
                        switch (key) 
                        {
                            case "firstname":
                                {
                                    actual.FirstName = values[key]; //Assign FirstName from dict value with key "firstname"
                                    break;
                                }
                            case "lastname":
                                {
                                    actual.LastName = values[key]; //Assign LastName from dict value with key "lastname"
                                    break;
                                }
                            case "salary":
                                {
                                    actual.SalaryPerHour = decimal.Parse(values[key]); //Assign SalaryPerHour from dict value with key "salary"
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
            Employee expected = new Employee(1, "Jack", "Sparrow", 900.90m); //Initialize expected object
            Assert.IsTrue(expected.Id == actual.Id && expected.FirstName == actual.FirstName && expected.LastName == actual.LastName 
                && expected.SalaryPerHour == actual.SalaryPerHour); //Compare each parameters within expected and actual
        }
    }
    class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal SalaryPerHour { get; set; }

        public Employee(int id, string name, string surname, decimal salary)
        {
            Id = id;
            FirstName = name;
            LastName = surname;
            SalaryPerHour = salary;

        }
        public void SetId(int id)
        {
            Id = id;
        }
        public override string ToString()
        {
            return $"{Id} {FirstName} {LastName} {SalaryPerHour}";
        }
    }
}