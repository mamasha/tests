using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace words_count
{
    interface IWordsCounter
    {
        void load(params string[] files);
        void displayStatus();
    }

    class WordsCounter : IWordsCounter
    {
        #region members

        class Counter
        {
            public string Word { get; set; }
            public int Value;
        }

        class Job
        {
            public string Path { get; set; }
            public Dictionary<string, Counter> Cache { get; set; }
            public ManualResetEvent Signal { get; set; }
        }

        private readonly string _rootFolder;
        private readonly IConcurrentMap<string, Counter> _counters;

        #endregion

        #region construction

        public static IWordsCounter New(string rootFolder)
        {
            return
                new WordsCounter(rootFolder);
        }

        private WordsCounter(string rootFolder)
        {
            _rootFolder = rootFolder;
            _counters = ConcurrentMap<string, Counter>.New();
        }

        #endregion

        #region private

        private void process(Job job)
        {
            void count(string word)
            {
                if (!job.Cache.TryGetValue(word, out var counter))      // get from job cache (accessed within a single thread)
                {
                    counter = _counters.putIfAbsent(word,               // get from global map (create new one if not exists)
                        new Counter { Word = word });

                    job.Cache.Add(word, counter);                       // store in job thread cache
                }

                Interlocked.Increment(ref counter.Value);
            }

            try
            {
                var lines = File.ReadLines(job.Path);                   // lazy evaluated line iterator

                foreach (var line in lines)
                {
                    if (line.IsEmpty())
                        continue;

                    var words = line.ToLower().Split(' ');              // split to case insensitive words

                    foreach (var word in words)
                    {
                        if (word == "")
                            continue;

                        count(word);
                    }
                }
            }
            finally
            {
                job.Signal.Set();
            }
        }

        private Job createJob(string file)
        {
            var path = Path.Combine(_rootFolder, file);
            var signal = new ManualResetEvent(false);
            var cache = new Dictionary<string, Counter>();

            var job = new Job {
                Path = path,
                Cache = cache,
                Signal = signal
            };

            return job;
        }

        #endregion

        #region interface

        void IWordsCounter.load(params string[] files)
        {
            var count = files.Length;

            if (count == 0)
                return;

            var jobSignals = new WaitHandle[count];

            for (var indx = 0; indx < count; indx++)
            {
                var file = files[indx];
                var lastFile = indx == count - 1;

                Console.WriteLine($"processing '{file}'");

                var job = createJob(file);

                jobSignals[indx] = job.Signal;

                if (lastFile)
                {
                    process(job);           // process last file in the context of this thread
                }
                else
                {
                    new Thread(() => process(job)).Start();
                }
            }

            WaitHandle.WaitAll(jobSignals);
        }

        void IWordsCounter.displayStatus()
        {
            var words = _counters.getKeys();
            Array.Sort(words);

            var total = 0;

            Console.WriteLine();

            foreach (var word in words)
            {
                var counter = _counters[word];
                var value = counter.Value;

                total += value;

                Console.WriteLine($"{word, -15} {value, 5}");
            }

            Console.WriteLine();
            Console.WriteLine($"{"** total", -15} {total, 5}");
        }

        #endregion
    }

    static class Helpers
    {
        public static bool IsEmpty(this string str)
        {
            return
                string.IsNullOrWhiteSpace(str);
        }
    }
}