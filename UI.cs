using System;

internal class UI {

    private const string IncorrectValue = "\nПолучено некорректное значение. Повторите ввод.\n";
    private const string IncorrectCommand = "\nКоманда не распознана\n";

    public static int askInteger(string prompt, int a, int b) {

        bool good = false;
        int result = 0;
        string input;
        while (!good) {
            Console.Write($"{prompt}");
            input = Console.ReadLine();

            good = int.TryParse(input, out result);

            if ( !good || result < a || result >= b ) {
                Console.WriteLine(IncorrectValue);
                good = false;
            }
        }
        return result;
    }

    public static string askString(string prompt) {

        Console.Write($"{prompt}");
        return Console.ReadLine();
    }

    public static bool askYesNo(string prompt) {

        bool good = false;
        bool result = false;
        string input;
        while (!good) {
            Console.Write($"{prompt}");
            input = Console.ReadLine();

            if (input == "y" || input == "Y" || input == "д" || input == "Д") {
                result = true;
                good = true;
            }
            else
            if (input == "n" || input == "N" || input == "н" || input == "Н") {
                result = false;
                good = true;
            }
            else
            {
                Console.WriteLine(IncorrectValue);
                good = false;
            }
        }
        return result;
    }

    private static Field UserEdit(Field old) {

        Field result = old;
        bool editing = true;
        string code;
        Console.WriteLine($"\nВыбрана запись:\n{old}");

        while (editing) {
            
            Console.WriteLine("Какое поле необходимо изменить?");
            Console.WriteLine("1: название\n2: автор\n3: год\n4: жанр\nq: закончить изменения");
            code = askString("\nКоманда: ");
            if (code == "1") {
                result.title = askString($"Новое значение поля [{old.title}]: ");
                Console.WriteLine($"Изменения внесены:\n{result}");
            }
            else
            if (code == "2") {
                result.author = askString($"Новое значение поля [{old.author}]: ");
                Console.WriteLine($"Изменения внесены:\n{result}");
            }
            else
            if (code == "3") {
                result.year = askInteger($"Новое значение поля [{old.year}]: ", 0, 2022);
                Console.WriteLine($"Изменения внесены:\n{result}");
            }
            else
            if (code == "4") {
                Console.WriteLine("Допустимые жанры: ");
                for (int i = 0; i < (int)Genre.Total; i++) Console.WriteLine($"{i}: {(Genre)i}");
                result.genre = (Genre)askInteger($"Новое значение поля [{(int)old.genre}]: ", 0, (int)Genre.Total);
                Console.WriteLine($"Изменения внесены:\n{result}");
            }
            else
            if (code == "q") {
                editing = false;
            }
            else
            {
                Console.WriteLine(IncorrectCommand);
            }
        }
    
        return result;
    }

    private static void SelectRecords(DataBase A)
    {
        string code;
        
        Console.WriteLine("\nРежим выбора:\n");
        Console.WriteLine("e: все записи");
        Console.WriteLine("k: выборка по ключу");
        Console.WriteLine("t: выборка по названию");
        Console.WriteLine("a: выборка по автору");
        Console.WriteLine("y: выборка по году");
        Console.WriteLine("g: выборка по жанру");

        code = askString("\nУкажите способ выделения: ");

        switch (code)
        {
            case "e":
                A.Select(x => true);
                break;
            case "k":
                int k = askInteger("Ключ: ", 0, int.MaxValue);
                A.Select(x => x.key == k);
                break;
            case "t":
                string t = askString("Название: ");
                A.Select(x => x.title == t);
                break;
            case "a":
                string a = askString("Автор: ");
                A.Select(x => x.author == a);
                break;
            case "y":
                int y = askInteger("Год: ", 0, int.MaxValue);
                A.Select(x => x.year == y);
                break;
            case "g":
                Console.WriteLine("\nКоды жанров:\n");
                for (int i = 0; i < (int)Genre.Total; i++)
                    Console.WriteLine($"{i}: {(Genre)i}");
                Genre g = (Genre)askInteger("\nЖанр: ", 0, (int)Genre.Total);
                A.Select(x => x.genre == g);
                break;
            default:
                Console.WriteLine(IncorrectCommand);
                break;

        }
    }

    public static void Session(DataBase A) {

        bool running = true;
        string code;

        Console.WriteLine("\nИспользуйте h, чтобы увидеть список команд");

        while (running) {
            code = askString("\nКоманда: ");

            switch(code) {
                case "h":
                    Console.Clear();
                    Console.WriteLine("\nДопустимые команды:\n");
                    Console.WriteLine("h: отобразить этот список и очистить консоль");
                    Console.WriteLine("l: отобразить базу данных");
                    Console.WriteLine("p: отобразить определенные записи");
                    Console.WriteLine("r: удалить выбранные записи");
                    Console.WriteLine("i: вставить запись на место"); 
                    Console.WriteLine("a: вставить запись в конец");
                    Console.WriteLine("e: корректировать выбранные записи");
                    Console.WriteLine("q: завершить сеанс");
                    Console.WriteLine("\nЗапросы: \n");
                    Console.WriteLine("0: найти кол-во книг, написанных в XXI веке");
                    Console.WriteLine("1: найти записи о поэмах Автора 4 и заменить их жанр на оду");
                    Console.WriteLine("2: найти кол-во романов XIX века");
                    Console.WriteLine("3: найти кол-во сказок X века или раньше, название которых начинается на А");
                    Console.WriteLine("4: найти кол-во книг Автора X, где X - ключ соответствующей записи");
                    Console.WriteLine();
                    break;
                case "l":
                    A.DisplayAll();
                    break;
                case "p":
                    SelectRecords(A);
                    A.DisplaySelected();
                    A.Deselect();
                    break;
                case "r":
                    SelectRecords(A);
                    A.Remove();
                    break;
                case "e":
                    SelectRecords(A);
                    A.Edit(UI.UserEdit);
                    break;
                case "q":
                    running = false;
                    break;
                case "a":
                case "i":
                    Field field;
                    field.key = A.FreeKey;
                    field.title = askString("Название: ");
                    field.author = askString("Автор: ");
                    field.year = askInteger("Год: ", 0, int.MaxValue);
                    Console.WriteLine("\nКоды жанров:\n");
                    for (int i = 0; i < (int)Genre.Total; i++)
                        Console.WriteLine($"{i}: {(Genre)i}");
                    field.genre = (Genre)askInteger("\nЖанр: ", 0, (int)Genre.Total);

                    if (code == "i")
                    {
                        int pos = askInteger("Позиция для вставки: ", 0, int.MaxValue);
                        A.Insert(field, pos);
                    }
                    else A.Append(field);
                    break;
                case "0":
                    Console.WriteLine("Запрос 0: найти кол-во книг, написанных в XXI веке");

                    if (askYesNo("Выполнить? "))
                    {
                        A.Select(x => x.year > 2000);
                        A.DisplaySelected();
                        A.Deselect();
                    }
                    break;
                case "1":
                    Console.WriteLine("Запрос 1: найти записи о поэмах Автора 4 и заменить их жанр на оду");
                    
                    if (askYesNo("Выполнить? "))
                    {
                        A.Select(x => (x.author == "Автор 4") && (x.genre == Genre.Poem));
                        A.DisplaySelected();
                        A.Edit(x => { x.genre = Genre.Ode; return x; });
                    }
                    break;
                case "2":
                    Console.WriteLine("Запрос 2: найти кол-во романов XIX века");

                    if (askYesNo("Выполнить? "))
                    {
                        A.Select(x => (x.year > 1800) && (x.year < 1901) && (x.genre == Genre.Novel));
                        A.DisplaySelected();
                        A.Deselect();
                    }
                    break;
                case "3":
                    Console.WriteLine("Запрос 3: найти кол-во сказок X века или раньше, название которых начинается на А");

                    if (askYesNo("Выполнить? "))
                    {
                        A.Select(x => (x.genre == Genre.FairyTale) && (x.year < 1001) && (x.title.StartsWith("А")));
                        A.DisplaySelected();
                        A.Deselect();
                    }
                    break;
                case "4":
                    Console.WriteLine("Запрос 4: найти кол-во книг Автора X, где X - ключ соответствующей записи");

                    if (askYesNo("Выполнить? "))
                    {
                        A.Select(x => x.author.EndsWith(x.key.ToString()));
                        A.DisplaySelected();
                        A.Deselect();
                    }
                    break;
                default:
                    Console.WriteLine(IncorrectCommand);
                    break;
            }

        }
        Console.WriteLine("\nGoodbye!\n");
    }
}