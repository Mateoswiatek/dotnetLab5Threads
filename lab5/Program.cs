using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static bool running = true;
    static ConcurrentQueue<Data> dataQueue = new(); // usedTarget-type new XD 
    static Dictionary<int, int> consumptionCount = new(); // new Dictionary<int, int>();
    static object lockObj = new();

    class Data
    {
        public int ProducerId { get; }

        public Data(int producerId)
        {
            ProducerId = producerId;
        }
    }

    class Producer
    {
        private int id;
        private Random rand;
        private int delay;

        public Producer(int id, int delay)
        {
            this.id = id;
            this.delay = delay;
            this.rand = new Random();
        }

        public void Produce()
        {
            while (running)
            {
                Thread.Sleep(rand.Next(delay));
                var data = new Data(id);
                dataQueue.Enqueue(data);
                Console.WriteLine($"Producer {id} produced data.");
            }
        }
    }

    class Consumer
    {
        private int id;
        private int delay;
        private Random rand;
        private Dictionary<int, int> localConsumptionCount;

        public Consumer(int id, int delay)
        {
            this.id = id;
            this.delay = delay;
            rand = new Random();
            localConsumptionCount = new Dictionary<int, int>();
        }

        public void Consume()
        {
            while (running)
            {
                Thread.Sleep(rand.Next(delay));
                if (dataQueue.TryDequeue(out Data data))
                {
                    lock (lockObj)
                    {
                        if (!localConsumptionCount.ContainsKey(data.ProducerId))
                            localConsumptionCount[data.ProducerId] = 0;
                        localConsumptionCount[data.ProducerId]++;
                    }
                    Console.WriteLine($"Consumer {id} consumed data from Producer {data.ProducerId}.");
                }
            }
            lock (lockObj)
            {
                foreach (var kvp in localConsumptionCount)
                {
                    if (!consumptionCount.ContainsKey(kvp.Key))
                        consumptionCount[kvp.Key] = 0;
                    consumptionCount[kvp.Key] += kvp.Value;
                }
            }
        }
    }

    static void Main(string[] args)
    {
        int n = 3; // number of producers
        int m = 2; // number of consumers
        int producerDelay = 1000;
        int consumerDelay = 1500;

        List<Thread> threads = new List<Thread>();

        for (int i = 0; i < n; i++)
        {
            var producer = new Producer(i, producerDelay);
            var thread = new Thread(producer.Produce);
            threads.Add(thread);
            thread.Start();
        }

        for (int i = 0; i < m; i++)
        {
            var consumer = new Consumer(i, consumerDelay);
            var thread = new Thread(consumer.Consume);
            threads.Add(thread);
            thread.Start();
        }

        Console.WriteLine("Press 'q' to quit.");
        while (Console.ReadKey().KeyChar != 'q')
        {
            // Tutaj jest nasz głowny program, bo wszystko sie robi w wątkach...
        }

        running = false;

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("Consumption Summary:");
        foreach (var kvp in consumptionCount)
        {
            Console.WriteLine($"Producer {kvp.Key} - {kvp.Value}");
        }
    }
}
