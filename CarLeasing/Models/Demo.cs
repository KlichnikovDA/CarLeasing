using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CarLeasing.Models
{
    public class DemoContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Analitic> Analitics { get; set; }
    }

    public class DemoInitializer : DropCreateDatabaseAlways<DemoContext>
    {
        protected override void Seed(DemoContext db)
        {
            db.Customers.Add(new Customer { CustomerId = 1, Name = "Иванов Иван Иванович", BirthDate = new DateTime(1990, 1, 1), LicenseDate = new DateTime(2008, 1, 1), NotPaid = false, HadAccidents = false, IncomeCategory = 1 });
            db.Cars.Add(new Car { CarId = 1, Model = "Renault Logan", Cost = 150.00, Period = 14, RequiredScore = 600 });
            db.Cars.Add(new Car { CarId = 2, Model = "Toyota Corolla", Cost = 140.00, Period = 17, RequiredScore = 580 });
            db.Cars.Add(new Car { CarId = 3, Model = "Renault Logan", Cost = 105.00, Period = 4, RequiredScore = 540 });
            db.Analitics.Add(new Analitic { AnaliticId = 1, Name = "Петров Петр Петрович" });

            base.Seed(db);
        }
    }

    public class Customer
    {
        // ID
        public int CustomerId { get; set; }
        // ФИО
        public string Name { get; set; }
        // дата рождения
        public DateTime BirthDate { get; set; }
        // дата получения прав
        public DateTime LicenseDate { get; set; }
        // флаг просрочки
        public bool NotPaid { get; set; }
        // флаг аварий
        public bool HadAccidents { get; set; }
        // Категория пользователя по доходам
        public int IncomeCategory { get; set; }
    }

    public class Car
    {
        // ID
        public int CarId { get; set; }
        // модель
        public string Model { get; set; }
        // скоринг
        public int RequiredScore { get; set; }
        // Цена
        public double Cost { get; set; }
        // Срок
        public int Period { get; set; }
    }

    public class Analitic
    {
        // ID
        public int AnaliticId { get; set; }
        // ФИО
        public string Name { get; set; }
    }
    public static class Scoring
    {
        static double AgeModifier = 10;
        static double StageModifier = 15;
        static double TimelyPaymentsModifier = 75;
        static double AccidentsModifier = 200;
        static double IncomeModifier = 50;

        public class Score
        {
            public int AgeScore { get; set; }
            public int StageScore { get; set; }
            public int PaymentsScore { get; set; }
            public int AccidentScore { get; set; }
            public int IncomeScore { get; set; }
            public int TotalScore { get; set; }

            public Score(Customer customer)
            {
                double result = 0;
                // оценка возраста
                int age = (int)Math.Round((DateTime.Today - customer.BirthDate).Days / 365.25);
                if (age < 25)
                {
                    result += 3.5 * AgeModifier;
                }
                else
                {
                    if (age < 45)
                    {
                        result += 6 * AgeModifier;
                    }
                    else
                    {
                        if (age < 60)
                        {
                            result += 7 * AgeModifier;
                        }
                        else
                            result += 3 * AgeModifier;
                    }
                }
                AgeScore = (int)result;
                TotalScore += (int)result;

                result = 0;
                // Оценка стажа
                int stage = (int)Math.Round((DateTime.Today - customer.LicenseDate).Days / 365.25);
                if (stage < 3)
                {
                    result += stage * StageModifier;
                }
                else
                {
                    if (stage < 10)
                    {
                        result += (stage - 2) * StageModifier;
                    }
                    else
                    {
                        result += (stage * 1.05) * StageModifier;
                    }
                }
                StageScore = (int)result;
                TotalScore += (int)result;

                result = 0;
                // Оценка просрочек
                if (customer.NotPaid)
                {
                    result -= TimelyPaymentsModifier;
                }
                else
                {
                    result += TimelyPaymentsModifier;
                }
                PaymentsScore = (int)result;
                TotalScore += (int)result;

                result = 0;
                // Оценка аварий
                if (customer.HadAccidents)
                {
                    result -= AccidentsModifier;
                }
                else
                {
                    result += AccidentsModifier;
                }
                AccidentScore = (int)result;
                TotalScore += (int)result;

                result = 0;
                // Оценка платежеспособности
                result += customer.IncomeCategory * IncomeModifier;
                IncomeScore = (int)result;
                TotalScore += (int)result;
            }
        }        
    }
}