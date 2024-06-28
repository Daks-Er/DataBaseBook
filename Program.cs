using System;
using System.IO;
using System.Runtime.Serialization;

internal class Program {
    static void Main(string[] args) {

        string path = UI.askString("Введите путь к файлу БД: ");

        if ( path == "" ) {

            path = "sample.db";
            Console.WriteLine("Будет создана случайная БД по адресу " + path);
            int n = UI.askInteger("Введите требуемое кол-во записей: ", 0, int.MaxValue);
            DataBase.Create(path, n);
        }
        else
        if (!File.Exists(path)) {

            Console.WriteLine("Файл не существует. Будет создана новая БД");
            FileStream f = File.Create(path);
            f.Close();
        }

        LogMode mode = UI.askYesNo("Перезаписывать журнал? Y/N: ") 
            ? LogMode.Create 
            : LogMode.Append;

        DataBase A = new DataBase(path, mode);
        bool ok = true;

        try {
            A.Open();
        }
        catch (InvalidOperationException) {
            Console.WriteLine("Не удаётся открыть сеанс: файл заблокирован");
            ok = false;
        }
        catch (SerializationException) {
            Console.WriteLine("Не удаётся открыть сеанс: файл имеет недопустимый формат");
            ok = false;
        }
        catch (ArgumentException) {
            Console.WriteLine("Не удаётся открыть сеанс: база данных содержит повторы ключей");
            ok = false;
        }

        if (ok) UI.Session(A); 

        A.Close();
    }
}