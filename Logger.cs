using System;
using System.IO;

enum LogMode {
    Create, Append
}

internal class Logger {
    private StreamWriter writer;

    public Logger(string path, LogMode mode) {

        if (mode == LogMode.Create) {
            writer = new StreamWriter(path, false);
        }
        else
        if (mode == LogMode.Append) {
            writer = new StreamWriter(path, true);
        }
    }

    public void Close() {
        writer.Close();
    }

    public void Write(string msg) {
        DateTime date = DateTime.Now;
        writer.WriteLine($"{date}| {msg}");
        writer.Flush();
    }
}