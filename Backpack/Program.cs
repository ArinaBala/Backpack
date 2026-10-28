using System;

namespace Backpack
{
    public class BagItem
    {
        public string Title {
            get; 
            
            set; 
        }
        public double Capacity 
        {
            get;
            set; 
        }
        public double Weight 
        {
            get;
            set;
        }

        public BagItem(string title, double capacity, double weight)
        {
            Title = title;
            Capacity = capacity;
            Weight = weight;
        }

        public override string ToString()
        {
            return $"{Title} (Об'єм: {Capacity} л, Вага: {Weight} кг)";
        }
    }

    public class Laptop : BagItem
    {
        public double Diagonal 
        {
        get;
        set; }

        public Laptop(string title, double capacity, double weight, double diagonal)
            : base(title, capacity, weight)
        {
            Diagonal = diagonal;
        }

        public override string ToString()
        {
            return base.ToString() + $", Діагональ: {Diagonal}\"";
        }
    }

    public class WaterBottle : BagItem
    {
        public string MaterialType 
        {
        get;
        set;
        }

        public WaterBottle(string title, double capacity, double weight, string materialType)
            : base(title, capacity, weight)
        {
            MaterialType = materialType;
        }

        public override string ToString()
        {
            return base.ToString() + $", Матеріал: {MaterialType}";
        }
    }

    public class Book : BagItem
    {
        public int PageCount 
        {
        get;
        set;
        }

        public Book(string title, double capacity, double weight, int pageCount)
            : base(title, capacity, weight)
        {
            PageCount = pageCount;
        }

        public override string ToString()
        {
            return base.ToString() + $", Сторінок: {PageCount}";
        }
    }

    public class Clothing : BagItem
    {
        public string Size 
        {
        get; 
        set;
        }

        public Clothing(string title, double capacity, double weight, string size)
            : base(title, capacity, weight)
        {
            Size = size;
        }

        public override string ToString()
        {
            return base.ToString() + $", Розмір: {Size}";
        }
    }

    public delegate void BagItemHandler(object source, BagItem element);

    public class Rucksack
    {
        private BagItem[] items = new BagItem[0];

        public string Hue 
        {
            get;
            set;
        }
        public string Producer 
        {
            get;
            set;
        }
        public string Material 
        {
            get;
            set;
        }
        public double Mass 
        {
            get;
            set;
        }
        public double LimitVolume 
        {
            get;
            set;
        }

        public BagItem[] Items => items;

        public event BagItemHandler ItemInserted;

        public Rucksack()
        {
        }

        public Rucksack(string hue, string producer, string material, double mass, double limitVolume)
        {
            Init(hue, producer, material, mass, limitVolume);
        }

        public void Init(string hue, string producer, string material, double mass, double limitVolume)
        {
            Hue = hue;
            Producer = producer;
            Material = material;
            Mass = mass;
            LimitVolume = limitVolume;
        }

        public double UsedVolume => items.Sum(x => x.Capacity);

        public double RestVolume => LimitVolume - UsedVolume;

        public double TotalWeight => Mass + items.Sum(x => x.Weight);

        public void Insert(BagItem element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Об'єкт не може бути null.");
            }

            if (UsedVolume + element.Capacity > LimitVolume)
            {
                throw new InvalidOperationException(
                    $"Перевищено об'єм рюкзака! Спроба додати '{element.Title}' ({element.Capacity} л). Доступно: {RestVolume} л."
                );
            }

            Array.Resize(ref items, items.Length + 1);
            items[items.Length - 1] = element;

            ItemInserted?.Invoke(this, element);
        }

        public void PrintDetails()
        {
            Console.WriteLine("=== ХАРАКТЕРИСТИКИ РЮКЗАКА ===");
            Console.WriteLine($"Марка та виробник: {Producer}");
            Console.WriteLine($"Колір: {Hue}");
            Console.WriteLine($"Тканина: {Material}");
            Console.WriteLine($"Власна вага рюкзака: {Mass} кг");
            Console.WriteLine($"Загальна вага (з вмістом): {TotalWeight} кг");
            Console.WriteLine($"Максимальний об'єм: {LimitVolume} л");
            Console.WriteLine($"Зайнято об'єму: {UsedVolume} / {LimitVolume} л (Вільне місце: {RestVolume} л)");
            Console.WriteLine("Вміст:");
            if (items.Length == 0)
            {
                Console.WriteLine("  (порожньо)");
            }
            else
            {
                foreach (var x in items)
                {
                    Console.WriteLine($"  - {x}");
                }
            }
            Console.WriteLine(new string('-', 40));
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Rucksack bag = new Rucksack();
            bag.Init(
                hue: "Чорний",
                producer: "Deuter (Німеччина)",
                material: "Cordura Nylon",
                mass: 1.2,
                limitVolume: 15.0
            );

            bag.ItemInserted += delegate (object sender, BagItem target)
            {
                Console.WriteLine($"[ПОДІЯ] Додано '{target.Title}' (об'єм: {target.Capacity} л, вага: {target.Weight} кг)");
            };

            bag.PrintDetails();

            try
            {
                Console.WriteLine("Додаємо предмети різних класів:");
                bag.Insert(new Laptop("Ноутбук", 4.0, 2.3, 15.6));
                bag.Insert(new WaterBottle("Пляшка води", 1.5, 1.5, "Пластик"));
                bag.Insert(new Book("Книга C#", 2.0, 0.8, 450));
                bag.Insert(new Clothing("Куртка", 5.0, 1.1, "L"));

                Console.WriteLine("\nСпроба додати предмет, який перевищує місткість:");
                bag.Insert(new Clothing("Спальний мішок", 5.0, 1.8, "XL"));
            }
            catch (Exception err)
            {
                Console.WriteLine($"\n{err.Message}");
            }

            Console.WriteLine("\nПідсумковий стан рюкзака:");
            bag.PrintDetails();
        }
    }
}
