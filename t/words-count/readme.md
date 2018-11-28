### Words counter

[ConcurrentMap.cs](./ConcurrentMap.cs) - simulates java `ConcurrentMap` class.
It has only add members since we only add words. A decorator pattern is used
to factor out the synchronization concern. The `New()` method wraps an implementation
with an `AtomicCuncurrenMap` instance that synchronizes all method calls. Since the constructor
of `ConcurrentMap` class is private, there is no way to get an unwrapped (unsafe) implementation.

[WordsCounter.cs](./WordsCounter.cs) implements the logic of word counting. 
Since the assignment couples a `WordsCounter` interface to the file system and to the console, 
it is a little bit harder to decouple separate concerns and write proper unit tests for each one.

Here are the concerns that `WordsCounter` class implements:
- Iterating lines of a file
- Reporting results to console
- Threads and job-end events orchestration
- Caching counters in a per-job cache

#### Synchronization model

Each word is being represented by a `Counter` [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object) class. A single `ConcurrentMap` ensures that each word has one counter only across all threads. Each counter has a `Value` field
that is being incremented with the [Interlocked.Increment()](docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.increment) method. Thus all threads can hold
a same instance of a counter. Furthermore threads may cache counters in local cache.
Thus a number of synchronized accesses to a single `ConcurrentMap` is minimized.
