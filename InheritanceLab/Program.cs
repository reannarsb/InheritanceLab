using System;

namespace EmployeePaymentSystem
{
    class Employee
    {
        public string EmployeeID;
        public string Name;
        public string SIN;

        public virtual double CalculateWeeklyPay()
        {
            return 0.0;
        }
    }

    class Salaried : Employee
    {
        public double WeeklySalary;

        public override double CalculateWeeklyPay()
        {
            return WeeklySalary;
        }
    }

    class Wage : Employee
    {
        public double HourlyRate;
        public double WorkHours;

        public override double CalculateWeeklyPay()
        {
            double regularPay = HourlyRate * Math.Min(WorkHours, 40);
            double overtimePay = HourlyRate * 1.5 * Math.Max(WorkHours - 40, 0);
            return regularPay + overtimePay;
        }
    }

    class PartTime : Employee
    {
        public double HourlyRate;
        public double WorkHours;

        public override double CalculateWeeklyPay()
        {
            return HourlyRate * WorkHours;
        }
    }

    class Program
    {
        static List<Employee> ReadEmployeesFromFile(string filePath)
        {
            List<Employee> employees = new List<Employee>();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                string employeeID = data[0].Trim();
                string name = data[1].Trim();
                string sin = data[2].Trim();
                double salaryOrHourlyRate = double.Parse(data[3].Trim());
                double workHours = double.Parse(data[4].Trim());

                Employee employee;
                if (employeeID[0] >= '0' && employeeID[0] <= '4')
                {
                    employee = new Salaried { EmployeeID = employeeID, Name = name, SIN = sin, WeeklySalary = salaryOrHourlyRate };
                }
                else if (employeeID[0] >= '5' && employeeID[0] <= '7')
                {
                    employee = new Wage { EmployeeID = employeeID, Name = name, SIN = sin, HourlyRate = salaryOrHourlyRate, WorkHours = workHours };
                }
                else if (employeeID[0] >= '8' && employeeID[0] <= '9')
                {
                    employee = new PartTime { EmployeeID = employeeID, Name = name, SIN = sin, HourlyRate = salaryOrHourlyRate, WorkHours = workHours };
                }
                else
                {
                    throw new ArgumentException("Invalid employee ID.");
                }

                employees.Add(employee);
            }
            return employees;
        }

        static void SaveEmployeesToFile(List<Employee> employees, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var employee in employees)
                {
                    writer.WriteLine($"{employee.EmployeeID},{employee.Name},{employee.SIN}");
                }
            }
        }

        static double CalculateAverageWeeklyPay(List<Employee> employees)
        {
            double totalPay = 0.0;
            foreach (var employee in employees)
            {
                totalPay += employee.CalculateWeeklyPay();
            }
            return totalPay / employees.Count;
        }

        static Tuple<double, string> CalculateHighestWeeklyPay(List<Employee> employees)
        {
            double maxPay = 0.0;
            string employeeName = "";
            foreach (var employee in employees.OfType<Wage>())
            {
                double weeklyPay = employee.CalculateWeeklyPay();
                if (weeklyPay > maxPay)
                {
                    maxPay = weeklyPay;
                    employeeName = employee.Name;
                }
            }
            return new Tuple<double, string>(maxPay, employeeName);
        }

        static void Main(string[] args)
        {
            string filePath = @"C:\Users\reann\OneDrive\Desktop\Object Oriented 2\InheritanceLab\TextFile1.txt";
            List<Employee> employees = ReadEmployeesFromFile(filePath);

            double averageWeeklyPay = CalculateAverageWeeklyPay(employees);
            Console.WriteLine("Average weekly pay for all employees: " + averageWeeklyPay.ToString("0.00"));

            Tuple<double, string> highestWeeklyPayInfo = CalculateHighestWeeklyPay(employees);
            Console.WriteLine("Highest weekly pay for a wage employee: " + highestWeeklyPayInfo.Item1.ToString("0.00") + " (Employee: " + highestWeeklyPayInfo.Item2 + ")");

            int totalEmployees = employees.Count;
            int salariedCount = employees.OfType<Salaried>().Count();
            int wageCount = employees.OfType<Wage>().Count();
            int partTimeCount = employees.OfType<PartTime>().Count();

            Console.WriteLine("Percentage of salaried employees: " + ((double)salariedCount / totalEmployees).ToString("P"));
            Console.WriteLine("Percentage of wage employees: " + ((double)wageCount / totalEmployees).ToString("P"));
            Console.WriteLine("Percentage of part-time employees: " + ((double)partTimeCount / totalEmployees).ToString("P"));

            SaveEmployeesToFile(employees, filePath);
        }
    }
}
