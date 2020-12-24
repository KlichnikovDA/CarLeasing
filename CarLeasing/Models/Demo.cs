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
            db.Customers.Add(new Customer
            {
                CustomerId = 1,
                Name = "Иванов Иван Иванович",
                BirthDate = new DateTime(1990, 1, 1),
                LicenseDate = new DateTime(2008, 1, 1),
                Married = false,
                HasChildren = false,
                CHistory = Customer.CreditHistory.Positive,
                Employment = false,
                IncomeCategory = 1,
                Verified = false
            });
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
        // Семейное положение
        public bool Married { get; set; }
        // Дети
        public bool HasChildren { get; set; }
        // Кредитная история
        public enum CreditHistory
        {
            Negative = 0,
            None = 1,
            Positive = 2,
        }
        public CreditHistory CHistory {get; set; }
        // Официальное трудоустройство
        public bool Employment { get; set; }
        // Дата устройства на работу
        public DateTime EmploymentDate { get; set; }
        // Категория пользователя по доходам
        public int IncomeCategory { get; set; }
        // Паспортные данные подтверждены
        public bool Verified { get; set; }
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
        static double AgeModifier = 1;
        static double MarriageModifier = 1;
        static double ChildrenModifier = 1;
        static double CreditHistoryModifier = 1;
        static double EmploymentModifier = 1;
        static double StageModifier = 1;
        static double IncomeModifier = 1;
        static double VerificationModifier = 1;
        static double DrivingStageModifier = 1;
        //static double AccidentsModifier = 1;

        public class Score
        {
            public Dictionary<string, int> ScoreCategories;
            public int TotalScore { get; set; }

            public Score(Customer customer)
            {
                ScoreCategories = new Dictionary<string, int>();
                int result = 0;
                // оценка возраста
                int age = (int)Math.Round((DateTime.Today - customer.BirthDate).Days / 365.25);
                if (age < 25)
                {
                    result += (int)(50 * AgeModifier);
                }
                else
                {
                    if (age <= 50)
                    {
                        result += (int)(150 * AgeModifier);
                    }
                    else
                    {
                        result += (int)(100 * AgeModifier);
                    }
                }
                ScoreCategories.Add("Возраст", result);
                TotalScore += result;

                result = 0;
                // Оценка семейного положения
                if (customer.Married)
                    result += (int)(100 * MarriageModifier);
                else
                    result += (int)(50 * MarriageModifier);
                ScoreCategories.Add("Семейное положение", result);
                TotalScore += result;

                result = 0;
                // Оценка наличия детей
                if (customer.HasChildren)
                    result += (int)(100 * ChildrenModifier);
                else
                    result += (int)(50 * ChildrenModifier);
                ScoreCategories.Add("Дети", result);
                TotalScore += result;

                result = 0;
                // Оценка кредитной истории
                if ((int)customer.CHistory == 0)
                    result += (int)(-100 * CreditHistoryModifier);
                else
                    if ((int)customer.CHistory == 1)
                        result += (int)(50 * CreditHistoryModifier);
                    else
                        result += (int)(100 * CreditHistoryModifier);
                ScoreCategories.Add("Кредитная история", result);
                TotalScore += result;

                result = 0;
                // оценка трудоустройства
                int stage;
                if (customer.Employment)
                {
                    result += (int)(100 * EmploymentModifier);
                    TotalScore += result;
                    ScoreCategories.Add("Трудоустройство", result);
                    stage = (int)Math.Round((DateTime.Today - customer.EmploymentDate).Days / 365.25);
                    if (stage < 5)
                        result = (int)(50 * StageModifier);
                    else
                        result = (int)(100 * StageModifier);
                    ScoreCategories.Add("Трудовой стаж", result);
                    TotalScore += result;
                }
                else
                {
                    result += (int)(-50 * EmploymentModifier);
                    ScoreCategories.Add("Трудоустройство", result);
                    TotalScore += result;
                }

                result = 0;
                // Оценка уровня доходов
                if (customer.IncomeCategory == 0)
                    result += (int)(-50 * IncomeModifier);
                else
                    if (customer.IncomeCategory == 1)
                        result += (int)(50 * IncomeModifier);
                    else
                        result += (int)(100 * IncomeModifier);
                ScoreCategories.Add("Уровень дохода", result);
                TotalScore += result;

                result = 0;
                // Оценка верифицированности
                if (customer.Verified)
                    result += (int)(100 * VerificationModifier);
                else
                    result += (int)(-50 * VerificationModifier);
                ScoreCategories.Add("Верифицированный пользователь", result);
                TotalScore += result;

                result = 0;
                // Оценка водительского стажа
                stage = (int)Math.Round((DateTime.Today - customer.LicenseDate).Days / 365.25);
                if (stage < 1)
                {
                    result += (int)(50 * DrivingStageModifier);
                }
                else
                {
                    result += (int)(100 * DrivingStageModifier);
                }
                ScoreCategories.Add("Водительский стаж", result);
                TotalScore += result;

                //result = 0;
                //// Оценка аварий
                //if (customer.HadAccidents)
                //{
                //    result -= AccidentsModifier;
                //}
                //else
                //{
                //    result += AccidentsModifier;
                //}
                //AccidentScore = (int)result;
                //TotalScore += (int)result;
            }
        }        
    }
}