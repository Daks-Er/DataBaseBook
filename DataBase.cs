using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
enum Genre {
    Comedy, Drama, Tragedy, Ode, Poem, Novel, FairyTale, Story, Total
}

[Serializable]
struct Field {
    public int key;
    public string title;
    public string author;
    public int year;
    public Genre genre;

    public override string ToString() {
        return $"{key, 4} {title, 30} {author, 20} {year, 4} {genre, 10}";
    }

    public static string Header {
        get {
            return $"Ключ {"Название", 30} {"Автор", 20} {"Год", 4} {"Жанр", 10}";
        }
    }
}

internal class DataBase {
    private FileStream mainBuffer, backBuffer;
    private BinaryFormatter bf;
    private Dictionary<int, bool> selection;
    private Logger log;
    private string file;

    public DataBase(string path, LogMode logMode) {

        mainBuffer = new FileStream(path, FileMode.Open);
        backBuffer = new FileStream(path + ".buf", FileMode.Create);
        bf = new BinaryFormatter();
        selection = new Dictionary<int, bool>();
        log = new Logger(path + ".log", logMode);
        file = path;
        log.Write("Getting started.");
    }

    public void Open() {

        log.Write("Trying to open session.");

        if (File.Exists(file + ".lock")) {
            throw new InvalidOperationException();
        }

        Field tmp;
        while (mainBuffer.Position < mainBuffer.Length) {
            tmp = Next();
            selection.Add(tmp.key, false);
        }

        FileStream f = File.Create(file + ".lock");
        f.Close();

        log.Write("Session opened.");
    }

    public void Close() {
        mainBuffer.Close();
        backBuffer.Close();
        selection.Clear();
        File.Delete(file + ".buf");
        File.Delete(file + ".lock");
        log.Write("Session closed.");
        log.Close();
    }

    public static void Create(string name, int count) {

        FileStream f = new FileStream(name, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        Field book;
        string[] names = {"Автор 1", "Автор 2", "Автор 3", "Автор 4", "Автор 5", "Автор 6", "Автор 7", "Автор 8", "Автор 9" };
        string[] title = {"Книга 1", "Книга 2", "Книга 3", "Книга 4", "Книга 5", "Книга 6", "Книга 7", "Книга 8", "Книга 9" };

        for (int i = 0; i < count; i++) {
            book.key = i;
            book.author = names[Rand.rnd.Next(8)];
            book.title = title[Rand.rnd.Next(8)];
            book.year = Rand.rnd.Next(868, 2021);
            book.genre = (Genre)Rand.rnd.Next((int)Genre.Total);
            bf.Serialize(f, book);
        }
        f.Close();
    }

    private void Reset() {
        log.Write("RESET");
        backBuffer.SetLength(0);
        mainBuffer.Position = 0;
    }

    private void Commit() {
        log.Write("COMMIT");
        mainBuffer.SetLength(0);
        backBuffer.Position = 0;
        backBuffer.CopyTo(mainBuffer);
    }

    private Field Next() {
        return (Field)bf.Deserialize(mainBuffer);
    }

    private void Bufferize(Field cargo) {
        bf.Serialize(backBuffer, cargo);
    }

    public int FreeKey {
        get {
            bool found = false;
            int key = 0;
            while (key < int.MaxValue && !found) {
                if (selection.ContainsKey(key)) {
                    key++;
                } else {
                    found = true;
                }
            }
            return key;
        }
    }

    public void DisplayAll() {

        mainBuffer.Position = 0;
        Field tmp;

        Console.WriteLine($"\n* {Field.Header}");

        while (mainBuffer.Position < mainBuffer.Length) {
            tmp = Next();
            if (selection[tmp.key]) {
                Console.WriteLine($"+ {tmp}");
            } else {
                Console.WriteLine($"  {tmp}");
            }
        }

        Console.WriteLine($"Всего {selection.Count} записей");
        log.Write($"Displayed {selection.Count} records");
    }

    public void DisplaySelected() {

        mainBuffer.Position = 0;
        Field tmp;
        int i = 0;

        Console.WriteLine($"\n{Field.Header}");

        while (mainBuffer.Position < mainBuffer.Length) {
            tmp = Next();
            if (selection[tmp.key]) {
                Console.WriteLine(tmp);
                i++;
            }
        }

        Console.WriteLine($"Всего {i} записей");
        log.Write($"Displayed {i} records");
    }

    public void Select(Predicate<Field> condition) {

        mainBuffer.Position = 0;
        Field tmp;

        while (mainBuffer.Position < mainBuffer.Length) {
            tmp = Next();
            if (condition(tmp) && !selection[tmp.key]) 
            {
                selection[tmp.key] = true;
                log.Write($"Selected record {tmp.key}");
            }
        }
    }

    public void Deselect()
    {

        mainBuffer.Position = 0;
        Field tmp;

        while (mainBuffer.Position < mainBuffer.Length)
        {
            tmp = Next();
            if (selection[tmp.key])
            {
                selection[tmp.key] = false;
                log.Write($"Deselected record {tmp.key}");
            }
        }
    }

    public void Remove() {

        Field tmp;
        Reset();

        while (mainBuffer.Position < mainBuffer.Length) {
            tmp = Next();
            if (selection[tmp.key]) {
                selection.Remove(tmp.key);
                log.Write($"Removed record \"{tmp}\"");
            } else {
                Bufferize(tmp);
            }
        }
        Commit();
    }

    public void Insert(Field neo, int pos) {

        int i = 0;
        Reset();

        while (mainBuffer.Position < mainBuffer.Length) {
            if (i == pos) {
                Bufferize(neo);
                selection.Add(neo.key, false);
                log.Write($"Inserted record \"{neo}\"");
            } else {
                Bufferize(Next());
            }
            i++;
        }
        if (i <= pos) {
            Bufferize(neo);
            selection.Add(neo.key, false);
            log.Write($"Inserted record \"{neo}\"");
        }
        Commit();
    }

    public void Append(Field neo) {

        Reset();

        mainBuffer.CopyTo(backBuffer);
        Bufferize(neo);
        selection.Add(neo.key, false);
        log.Write($"Inserted record \"{neo}\"");

        Commit();
    }

    public void Edit(Func<Field, Field> filter) {

        Field tmp, neo;
        Reset();

        while (mainBuffer.Position < mainBuffer.Length) {
            tmp = Next();
            if (selection[tmp.key]) {

                log.Write($"Started editing \"{tmp}\"");
                neo = filter(tmp);
                log.Write($"Ended editing   \"{neo}\"");

                if (neo.key != tmp.key) {
                    log.Write("Key change detected! Preserving old record");
                    Bufferize(tmp);
                } else {
                    Bufferize(neo);
                    selection[tmp.key] = false;
                }
            } else {
                Bufferize(tmp);
            }
        }
        Commit();
    }
}