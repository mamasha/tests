## CsvParser for Sela

Start with [unit tests](./_nunit/UsingCsvParserTests.cs)  
Than go to [CsvParser](./CsvParser.cs)

### Thoughts on design

Obviously the suggested interface has two different parts. Sequential access and random access to a file.
Random access can lead to performance issues since the parser needs to read all lines before a desired point.
It can be optimized by looking at the file as a stream of bytes and making a byte-offset to a line number map.
But since the main scenario of the parser probably will be a sequential access (going through all the lines), 
there is no need in optimization of that kind. Except one scenario, when random access is used
for actually traversing all the lines of a file. In this use case both random and sequential access should give
the same performance.

A suggested List<CsvRows> as a result value of filtering process is not convenient. Since it requires all the 
matching results to be accumulated before they are returned. So a more flexible IEnumarable<CsvRow> collection will be 
implemented.

All methods are thread safe except random access implementation.

After implementing the random access logic I would argue hard against using it since

- Traversing through a file is not thread safe
- It is very easy to kill performance by using random access
- Empty lines can not be eliminated, since external viewers count them


### Thoughts on implementation

I read line by line withoug chunking since the underlaying File.ReadLines() should do it by itself (At least I guess it does so).

- I use programing to interface technique through the code
- A dedicated exception CsvParsingException would be defined for real use
- There is a dependency on nunit framework for unit tests (_nunit folder)
