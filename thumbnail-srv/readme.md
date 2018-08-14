## Thumbnail demo service

Start with a [unit tests page](https://thumbnail-srv.azurewebsites.net/unit/test.html)  
Then go to a [log endpoint](https://thumbnail-srv.azurewebsites.net/log)  


### Compilation and publish

Visual Studio 2017 --> Publish to Dropbox folder --> Azure deploy from Dropbox


### Sources - points of interest

- Resize implementation is in [ImageUtilities.cs](./ImageUtilities.cs)
- Service scaling in production can be seen through [ThumbnailOp.cs](./ThumbnailOp.cs) `runStateMachine()` function
- Routing and endpoints are in [AnyHandler.cs](./AnyHandler.cs) `startRequest()` function

### Request flow

[AnyHandler.cs](./AnyHandler.cs)  -->
[Api.cs](./Api.cs)  -->
[ThumbnailOp.cs](./ThumbnailOp.cs)  -->
[ImageUtilities.cs](./ImageUtilities.cs)

### Thoughts on implementation

**The state machine logic**

- Look if a needed thumbnail is in cache
- If so retrieve from cache
- Am I first to get the thumbnail? 
    --> schedule a download
- Wait for download to be completed
- Am I first to resize the thumbnail? 
    --> schedule a resize
- Wait for resize to be completed
- Put a ready thumbnail to cache

As a result of such a state machine each remote image is downloaded only once. A resize operation is done only once as well. Concurrent requests for a same image (burst of CNN) wait for the first single download to be completed. The same is happening for a resize operation.

No blocking waits. An image re-sampling is done by a background thread.

The real gem of this project is the AsyncFlow class that facilitates implementing a concise state machine inside the ThumbnailOp class. I have been thinking about various asynchronous programming models many years. I have always felt like a switch statement is a very promising idea for encapsulating an asynchronous state machine within a single function. That's what AsyncFlow class is all about.

### Concerns encapsulation

- AnyHandler.cs and SrvRequest.cs encapsulate Asp.Net interaction
- Api.cs encapsulates API parameters and routing 
- ThumbnailOp.cs encapsulates flow of a single thumbnail API
- LocalCache.cs encapsulates caching of the service
- AsyncFlow.cs encapsulates asynchronous programing model


### Infrastructure classes

- Caching - LocalCache.cs
- Asynchronous programming - AsyncFlow.cs
- Asp.Net - AnyHandler.cs and SrvRequest.cs
- Logging - Logger.cs and TopicLogger.cs


### Techniques used in solution

#### Programing to interface

Programming to interface has implementation benefits on its own. But its real glory is in expressing of building blocks of a problem domain in a short and concise way. With interfaces it is a matter of seconds to see how a class represents a decoupled concern and stands for the SOLID principles.

#### Separation of concerns

The major cause of spaghetti code, IMHO, not the lack of technical skills of a programmer, but wrong decomposition of a problem domain into properly decoupled concerns which then may become lego blocks to form any solution within that problem domain.






