

class Program
{
    static bool running = true;

    static void MonitorDirectory(string path)
    {
        // "wzorzec" observer w realnym zastosowaniu
        using (var watcher = new FileSystemWatcher(path))
        {
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Created += (s, e) => Console.WriteLine($"File added: {e.Name}");
            watcher.Deleted += (s, e) => Console.WriteLine($"File removed: {e.Name}");
            watcher.EnableRaisingEvents = true;

            while (running) { Thread.Sleep(100); }
        }
    }

    static void Main(string[] args)
    {
        string directoryPath = @"/home/mswiatek/RiderProjects/lab5/lab5_monitoring/testowe";
        Thread monitorThread = new Thread(() => MonitorDirectory(directoryPath));
        monitorThread.Start();

        Console.WriteLine("Press 'q' to quit.");
        while (Console.ReadKey().KeyChar != 'q') { }

        running = false;
        monitorThread.Join();
    }
}