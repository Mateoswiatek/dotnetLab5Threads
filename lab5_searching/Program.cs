
using System.Collections.Concurrent;

class Program
{
    static void SearchFiles(string directory, string searchPattern, Action<string> onFileFound, ConcurrentQueue<string> fileQueue)
    {
        // Dla plików sprawdzamy
        foreach (var file in Directory.GetFiles(directory))
        {
            if (Path.GetFileName(file).Contains(searchPattern))
            {
                onFileFound(file); // wersja z wypisywaniem "odrazu"
                fileQueue.Enqueue(file); // wersja z kolejka
            }
        }
        //Dla podkatalogów odpalamy szkanie w nich.
        foreach (var subDirectory in Directory.GetDirectories(directory))
        {
            SearchFiles(subDirectory, searchPattern, onFileFound, fileQueue);
        }
    }

    static void Main(string[] args)
    {
        string directoryPath = @"/home/mswiatek/RiderProjects/lab5/lab5_searching/testowe";
        string searchPattern = "szuk";
        
        ConcurrentQueue<string> fileQueue = new ConcurrentQueue<string>();
        
        //Za pomocą action jest prosciej, ew kolejke dodać.
        Thread searchThread = new Thread(() => SearchFiles(directoryPath, searchPattern, Console.WriteLine, fileQueue));
        searchThread.Start();
        searchThread.Join();
        
        Console.WriteLine("\n\nZnalezione pliki:");
        while (fileQueue.TryDequeue(out string foundFile))
        {
            Console.WriteLine(foundFile);
        }
    }
}