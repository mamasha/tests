### Threading model
- Async/Await

### Software layers

### Software Concerns 
- 

### Error handling

### Protocol
- Transaction per connection

### Coding convension

### Deployment, platform and runtime

### Project structure

### Programming to interfaces

### Security considirations

### Third parties

### Unit tests

### Highlevel design

### Classlevel design

interface IAuthenticator {
    void Authenticate(Request request);
}

interface ILogger {
    void Log(object msg);
}


interface IServer {
    void AddUser(string userName, string password, string folder);
    void RemoveUser(string userName);
    void UpdatePassword(string userName, string password);
}

interface IClient {
    string[] ListFiles(SrvRequest request);
    void Upload(SrvRequest request, string srcPath);
    void Download(SrvRequest request, string fileName, string dstFolder);
    void Delete(SrvRequest request, string fileName);
}

class TcpProxy : IClient
{
    private readonly IClient _server;
    
    public static IClient MakeClientSide(string address);
    
    public static void StartServerSide(IClient client)
    {
        
    }
    
    private void ProcessEvent()
    {
        var tcp = ListenForConnections();
        var request = tcp.ReadRequest();
        switch (request.OpCode)
        {
            case "ListFiles": 
                SrvListFiles(); 
                return;
        }
    }
    
    private void SrvListFiles(Tcp tcp)
    {
        var list = _server.ListFiles(request);
        tcp.WriteStringArray(list);
    }
    
    public ListFiles()
    {
        var tcp = makeTcpConnection();
        tcp.WriteRequest(request);
        var list = tcp.ReadStringArray();
        return list;
    }
}

class Server : IClient, IServer
{
    
}