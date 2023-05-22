using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ZT
{
    class Program
    {
        static string filePath { get; set; }
        static int getLastId()
        {
            List<Employee> employees = getAll();
            int lastId = employees.Count == 0 ? 0 : employees.Last().Id;
            return lastId;
        }
        static void saveToJson(Employee employee)
        {
            List<Employee> employees = getAll();
            int lastId = employees.Count == 0 ? 0 : employees.Last().Id;
            employee.Id = lastId+1;
            employees.Add(employee);
            string serialize = JsonConvert.SerializeObject(employees);
            File.WriteAllText(filePath, serialize);
        }
        static void saveToJson(List<Employee> employees)
        {
            string serialize = JsonConvert.SerializeObject(employees);
            File.WriteAllText(filePath, serialize);
        }
        static void updateById(Dictionary<string,string> values) 
        {
            List<Employee> currentEmployees = getAll();
            int id = int.Parse(values["id"]);
            Employee forUpdate = currentEmployees.FirstOrDefault(x => x.Id == id);
            if (forUpdate != null)
            {
                Console.WriteLine("Employee to update:\n" + forUpdate + "\n");
                foreach (String key in values.Keys)
                {
                    if (key != null)
                    {
                        switch (key) 
                        { 
                            case "firstname":
                                {
                                    forUpdate.FirstName = values[key];
                                    break;
                                }
                            case "lastname":
                                {
                                    forUpdate.LastName = values[key];
                                    break;
                                }
                            case "salary":
                                {
                                    forUpdate.SalaryPerHour = decimal.Parse(values[key]);
                                    break; 
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
                Console.WriteLine("Updated employee:\n" + forUpdate + "\n");
                saveToJson(currentEmployees);
            }
        }
        static void getById(int id) 
        {
            List<Employee> currentEmployees = getAll();
            Employee forGet = currentEmployees.FirstOrDefault(x => x.Id == id);
            string getEmployee = $"Id = {forGet.Id}, FirstName = {forGet.FirstName}, LastName = {forGet.LastName}, SalaryPerHour = {forGet.SalaryPerHour}";
            Console.WriteLine(getEmployee);
        }
        static void deleteById(int id)
        {
            List<Employee> currentEmployees = getAll();
            Employee forDelete = currentEmployees.FirstOrDefault(x => x.Id == id);
            if (forDelete != null)
            {
                Console.WriteLine("This employee will be removed:\n" + forDelete + "\n Confirm deletion(y/n)?");
                string confirm = Console.ReadLine();
                if (confirm == "y" || confirm == "Y")
                {
                    Console.WriteLine("Deletion confirmed.");
                    currentEmployees.Remove(forDelete);
                    saveToJson(currentEmployees);
                }
                else
                {
                    Console.WriteLine("Deletion canceled");
                }
            }
        }
        static List<Employee> getAll()
        {
            string json = File.ReadAllText(filePath);
            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(json);
            return employees ?? new List<Employee>();
        }
        static void deleteAll()
        {
            File.WriteAllText(filePath, String.Empty);
            Console.WriteLine("File Contents deleted");

        }
        static int GetIntFromString(string inputStr)
        {
            int input = 0;
            try
            {
                input = int.Parse(inputStr);
            }
            catch (FormatException)
            {
            }
            return input;
        }
        
        static void Main(string[] args)
        {
            bool working = true;
            string fileName = "employees.json";
            string fileFolderPath = Path.GetTempPath();
            filePath = fileFolderPath + fileName;
            if (File.Exists(filePath) == false) 
            {
                var file = File.Create(filePath);
                file.Close();
            }
            string allCommands = "\n '-getall' - Show all \n '-add' - Add new \n '-delete' - Delete by Id \n '-update' - Update by Id \n '-clear' - Clear file \n '-end' - End program \n";
            Console.WriteLine("Welcome to console application that processes a text file containing a list of employees in JSON format\nFor list of commands enter '-help'.");
            Console.WriteLine("File used: " + filePath);
            while (working)
            {
                Console.WriteLine("\nPlease enter command:\n");
                string input = Console.ReadLine();             
                var spaceIndex = input.IndexOf(" ");
                string inputCommand;
                string inputValue;
                Dictionary<string, string> commandValues = new Dictionary<string, string>();
                try
                {
                    inputCommand = input.Substring(0, spaceIndex).ToLower();
                    inputValue = input.Substring(spaceIndex + 1);
                    commandValues = inputValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(part => part.Split(':'))
                        .ToDictionary(split => split[0], split => split[1]);
                    commandValues = commandValues.ToDictionary(k => k.Key.ToLower(), k => k.Value);

                }
                catch (Exception)
                {
                    inputCommand = input.ToLower();
                    inputValue = "";
                }
                switch (inputCommand)
                {
                    case "-help":
                        {
                            Console.WriteLine(allCommands);
                            break;
                        }
                    case "-getall":
                        {
                            var employees = getAll();
                            if (employees.Count == 0)
                            {
                                Console.WriteLine("Empty for now...\n");
                            }
                            Console.WriteLine("Current employees:\n_______");
                            foreach (Employee employee in employees)
                            {
                                Console.WriteLine($"{employee}");
                            }
                            Console.WriteLine("_______");
                            break;
                        }
                    case "-add":
                        {
                            try
                            {
                                if (commandValues["firstname"] == null || commandValues["lastname"] == null || commandValues["salary"] == null)
                                {
                                    Console.WriteLine("Wrong or insufficient data for -add command.");
                                }
                                else
                                {
                                    Employee newEmployee = new Employee(commandValues["firstname"], commandValues["lastname"], Convert.ToDecimal(commandValues["salary"]));
                                    saveToJson(newEmployee);
                                    Console.WriteLine("\nNew employee added.\n");
                                }
                            }
                            catch (Exception) 
                                {
                                Console.WriteLine("Wrong command.");
                                }
                            break;
                        }
                    case "-delete":
                        {
                            if (commandValues["id"] != null)
                            {
                                int id;
                                int lastId = getLastId();
                                try
                                {
                                    id = int.Parse(commandValues["id"]);
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Correct ID needed for this command.");
                                    break;
                                }
                                if (id == 0 || id > lastId)
                                {
                                    Console.WriteLine("Wrong Id.");
                                }
                                else
                                {
                                    deleteById(id);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Correct ID needed for this command.");
                            }
                            break;
                        }
                    case "-get":
                        {
                            if (commandValues["id"] != null)
                            {
                                int id = int.Parse(commandValues["id"]);
                                int lastId = getLastId();
                                if (id == 0 || id > lastId)
                                {
                                    Console.WriteLine("Wrong Id.");
                                }
                                else
                                {
                                    getById(id);
                                }
                            }
                            else
                            {
                                Console.WriteLine("ID needed for this command.");
                            }                                              
                            break;
                        }
                    case "-update":
                        {
                            int lastId = getLastId();
                            if (commandValues["id"] != null && int.Parse(commandValues["id"]) <= lastId)
                            {
                                string newId = commandValues["id"];
                                int id = GetIntFromString(newId);
                                updateById(commandValues);
                            }
                            else
                            {
                                Console.WriteLine("Correct ID needed for this command.");
                            }
                            break;
                        }
                    case "-clear":
                        {
                            deleteAll();
                            break;
                        }
                    case "-end":
                        {
                            working = false;
                            Console.WriteLine("\nProgram ended. Thanks for your time.\nCreated by Savchuk Denis.");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Wrong command");
                            break;
                        }
                }
            }
        }
    }

    class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal SalaryPerHour { get; set; }

        public Employee(string name, string surname, decimal salary)
        {
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