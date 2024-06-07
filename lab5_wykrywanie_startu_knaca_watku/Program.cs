using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        int n = 5; // number of threads
        List<Thread> threads = new List<Thread>();
        CountdownEvent countdown = new CountdownEvent(n);

        for (int i = 0; i < n; i++)
        {
            var thread = new Thread(() =>
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} started.");
                countdown.Signal();
                while (true)
                {
                    if (Volatile.Read(ref stop))
                    {
                        break;
                    }
                }
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} stopping.");
            });
            threads.Add(thread);
            thread.Start();
        }

        countdown.Wait();
        Console.WriteLine("All threads have started.");

        Volatile.Write(ref stop, true);

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("\nWidzimy, ze w roznych kolejnosciach zaczynaly i konczyly.\nAll threads have been stopped. Main thread exiting.");
    }

    static volatile bool stop = false;
}
