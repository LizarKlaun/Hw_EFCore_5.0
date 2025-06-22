using Hw_EFCore_5._0.Entities;

namespace Hw_EFCore_5._0
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Меню Притулку для Собак ---");
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
                    case "1":
                        AddDog();
                        break;
                    case "2":
                        UpdateDog();
                        break;
                    case "3":
                        ViewAllDogs();
                        break;
                    case "4":
                        ViewDogsInShelter();
                        break;
                    case "5":
                        ViewAdoptedDogs();
                        break;
                    case "6":
                        ShowSearchMenu();
                        break;
                    case "0":
                        Console.WriteLine("Дякуємо, що завітали!");
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
                WaitForEnter();
            }
        }

        static void AddDog()
        {
            Console.Clear();
            Console.WriteLine("--- Додавання нової собаки ---");
            try
            {
                Console.Write("Введіть кличку: ");
                string name = Console.ReadLine();

                Console.Write("Введіть вік (повних років): ");
                int age = int.Parse(Console.ReadLine());

                Console.Write("Введіть породу: ");
                string breed = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(breed))
                {
                    Console.WriteLine("Кличка та порода не можуть бути пустими.");
                    return;
                }

                var newDog = new Dog { Name = name, Age = age, Breed = breed };

                using (var context = new ShelterContext())
                {
                    context.Dogs.Add(newDog);
                    context.SaveChanges();
                }
                Console.WriteLine("\nСобаку успішно додано!");
            }
            catch (FormatException)
            {
                Console.WriteLine("\nПомилка: Вік має бути числом.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nСталася помилка: {ex.Message}");
            }
        }

        static void UpdateDog()
        {
            Console.Clear();
            Console.WriteLine("--- Оновлення даних собаки ---");
            Console.Write("Введіть ID собаки для оновлення: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            using (var context = new ShelterContext())
            {
                var dog = context.Dogs.Find(id);
                if (dog == null)
                {
                    Console.WriteLine("Собаку з таким ID не знайдено.");
                    return;
                }

                Console.WriteLine($"Поточні дані: {dog}");
                try
                {
                    Console.Write($"Нова кличка (поточна: {dog.Name}): ");
                    dog.Name = Console.ReadLine();

                    Console.Write($"Новий вік (поточний: {dog.Age}): ");
                    dog.Age = int.Parse(Console.ReadLine());

                    Console.Write($"Нова порода (поточна: {dog.Breed}): ");
                    dog.Breed = Console.ReadLine();

                    Console.Write($"Собаку прилаштовано? (y/n, поточний: {(dog.IsAdopted ? 'y' : 'n')}): ");
                    string adoptedChoice = Console.ReadLine().ToLower();
                    if (adoptedChoice == "y") dog.IsAdopted = true;
                    else if (adoptedChoice == "n") dog.IsAdopted = false;

                    context.SaveChanges();
                    Console.WriteLine("\nДані успішно оновлено!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nПомилка: Вік має бути числом.");
                }
            }
        }

        static void ViewAllDogs()
        {
            using var context = new ShelterContext();
            PrintDogList("--- Всі собаки в базі ---", context.Dogs.ToList());
        }

        static void ViewDogsInShelter()
        {
            using var context = new ShelterContext();
            PrintDogList("--- Собаки, що чекають на сім'ю ---", context.Dogs.Where(d => !d.IsAdopted).ToList());
        }

        static void ViewAdoptedDogs()
        {
            using var context = new ShelterContext();
            PrintDogList("--- Собаки, що знайшли дім ---", context.Dogs.Where(d => d.IsAdopted).ToList());
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

            using var context = new ShelterContext();
            List<Dog> results = new List<Dog>();

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        var dog = context.Dogs.Find(id);
                        if (dog != null) results.Add(dog);
                    }
                    break;
                case "2":
                    Console.Write("Введіть кличку (або її частину): ");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        results = context.Dogs.Where(d => d.Name.Contains(name)).ToList();
                    }
                    break;
                case "3":
                    Console.Write("Введіть породу: ");
                    string breed = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(breed))
                    {
                        results = context.Dogs.Where(d => d.Breed.ToLower() == breed.ToLower()).ToList();
                    }
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
                foreach (var dog in dogs)
                {
                    Console.WriteLine(dog);
                }
            }
        }

        static void WaitForEnter()
        {
            Console.WriteLine("\nНатисніть Enter, щоб повернутися до меню...");
            Console.ReadLine();
        }
    }

}
