using Dapper;
using Hw_EFCore_5._0.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace Hw_EFCore_5._0
{
    internal class Program
    {
        private static readonly string connectionString = "Data Source=dogshelter.db";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Притулок для Собак (Dapper & SQLite) ---");
                Console.WriteLine("1. Додати собаку");
                Console.WriteLine("2. Оновити дані собаки за ID");
                Console.WriteLine("3. Переглянути всіх собак");
                Console.WriteLine("4. Переглянути собак, що ще в притулку");
                Console.WriteLine("5. Переглянути прилаштованих собак");
                Console.WriteLine("6. Пошук собаки");
                Console.WriteLine("0. Вихід");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddDog(); break;
                    case "2": UpdateDog(); break;
                    case "3": ViewAllDogs(); break;
                    case "4": ViewDogsInShelter(); break;
                    case "5": ViewAdoptedDogs(); break;
                    case "6": ShowSearchMenu(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір. Спробуйте ще раз."); break;
                }
                WaitForEnter();
            }
        }


        static void InitializeDatabase()
        {
            using var connection = new SqliteConnection(connectionString);
            var sql = @"
            CREATE TABLE IF NOT EXISTS Dogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Age INTEGER NOT NULL,
                Breed TEXT NOT NULL,
                IsAdopted BOOLEAN NOT NULL DEFAULT 0
            )";
            connection.Execute(sql);
        }

        static void AddDog()
        {
            Console.Clear();
            Console.WriteLine("--- Додавання нової собаки ---");
            Console.Write("Введіть кличку: "); string name = Console.ReadLine();
            Console.Write("Введіть вік: "); int age = int.Parse(Console.ReadLine());
            Console.Write("Введіть породу: "); string breed = Console.ReadLine();

            var sql = "INSERT INTO Dogs (Name, Age, Breed) VALUES (@Name, @Age, @Breed);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Execute(sql, new { Name = name, Age = age, Breed = breed });
            }

            Console.WriteLine("\nСобаку успішно додано!");
        }

        static void UpdateDog()
        {
            Console.Write("Введіть ID собаки для оновлення: ");
            int id = int.Parse(Console.ReadLine());

            using (var connection = new SqliteConnection(connectionString))
            {
                var dog = connection.QueryFirstOrDefault<Dog>("SELECT * FROM Dogs WHERE Id = @Id;", new { Id = id });

                if (dog == null)
                {
                    Console.WriteLine("Собаку не знайдено.");
                    return;
                }

                Console.WriteLine($"Поточні дані: {dog}");
                Console.Write($"Нова кличка (поточна: {dog.Name}): "); dog.Name = Console.ReadLine();
                Console.Write($"Новий вік (поточний: {dog.Age}): "); dog.Age = int.Parse(Console.ReadLine());
                Console.Write("Собаку прилаштовано? (y/n): ");
                dog.IsAdopted = Console.ReadLine().ToLower() == "y";

                var sql = "UPDATE Dogs SET Name = @Name, Age = @Age, Breed = @Breed, IsAdopted = @IsAdopted WHERE Id = @Id;";
                connection.Execute(sql, dog);

                Console.WriteLine("\nДані оновлено!");
            }
        }

        static void ViewAllDogs()
        {
            using var connection = new SqliteConnection(connectionString);
            var dogs = connection.Query<Dog>("SELECT * FROM Dogs;").ToList();
            PrintDogList("--- Всі собаки в базі ---", dogs);
        }

        static void ViewDogsInShelter()
        {
            using var connection = new SqliteConnection(connectionString);
            var dogs = connection.Query<Dog>("SELECT * FROM Dogs WHERE IsAdopted = 0;").ToList();
            PrintDogList("--- Собаки, що чекають на сім'ю ---", dogs);
        }

        static void ViewAdoptedDogs()
        {
            using var connection = new SqliteConnection(connectionString);
            var dogs = connection.Query<Dog>("SELECT * FROM Dogs WHERE IsAdopted = 1;").ToList();
            PrintDogList("--- Собаки, що знайшли дім ---", dogs);
        }

        static void ShowSearchMenu()
        {
            Console.Clear();
            Console.WriteLine("--- Пошук собаки ---");
            Console.WriteLine("1. За ID");
            Console.WriteLine("2. За кличкою");
            Console.WriteLine("3. За породою");
            Console.Write("\nВаш вибір: ");
            string choice = Console.ReadLine();

            List<Dog> results = new List<Dog>();
            using var connection = new SqliteConnection(connectionString);

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        var dog = connection.QueryFirstOrDefault<Dog>("SELECT * FROM Dogs WHERE Id = @Id;", new { Id = id });
                        if (dog != null) results.Add(dog);
                    }
                    break;
                case "2":
                    Console.Write("Введіть кличку (або її частину): ");
                    string name = Console.ReadLine();
                    results = connection.Query<Dog>("SELECT * FROM Dogs WHERE Name LIKE @SearchTerm;", new { SearchTerm = $"%{name}%" }).ToList();
                    break;
                case "3":
                    Console.Write("Введіть породу: ");
                    string breed = Console.ReadLine();
                    results = connection.Query<Dog>("SELECT * FROM Dogs WHERE Breed = @Breed;", new { Breed = breed }).ToList();
                    break;
                default:
                    Console.WriteLine("Невірний вибір.");
                    return;
            }

            PrintDogList("--- Результати пошуку ---", results);
        }

        static void PrintDogList(string title, List<Dog> dogs)
        {
            Console.Clear();
            Console.WriteLine(title);
            Console.WriteLine(new string('-', title.Length));
            if (!dogs.Any())
            {
                Console.WriteLine("Список порожній.");
            }
            else
            {
                dogs.ForEach(Console.WriteLine);
            }
        }

        static void WaitForEnter()
        {
            Console.WriteLine("\nНатисніть Enter, щоб повернутися до меню...");
            Console.ReadLine();
        }
    }
}