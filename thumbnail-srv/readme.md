## Thumbnail demo service

Start with [unit tests page]()
Then go to [Log endpoint]()


### Compilation and publish

Visual Studio 2017 --> Publish to Dropbox folder --> Azure deploy from Dropbox


### Sources - points of interest

Resize implementation is in [ImageUtilities.cs](./ImageUtilities.cs)
Scaling can be seen through [ThumbnailOp.cs](./ThumbnailOp.cs) `runStateMachine()` function
Routing and endpoints in [AnyHandler.cs](./AnyHandler.cs) `startRequest()` function

### Request flow

[AnyHandler.cs](./AnyHandler.cs)  ==>
[Api.cs](./Api.cs)  ==>
[ThumbnailOp.cs](./ThumbnailOp.cs)  ==>
[ImageUtilities.cs](./ImageUtilities.cs)

### Thoughts on implementation of thumbnail operation (state machine)

- Look if a needed thumbnail is in cache
- If so retrieve from cache and return
- Am I first to get the thumbnail? ==> schedule a download
- Wait for download of a remote image to be completed
- Am I first to resize the downloaded image? ==> schedule a resize
- Wait for resize to be completed
- Put a ready thumbnail to cache

As a result of this state machine each remote image is downloaded only once. As well a resize operation is also done only once. In a case of concurrent request for a same image (burst of CNN) all requests are waiting until the first one downloads the required image. The same is happening for resize operation.

No blocking waits. An Image re-sampling is done by background threads.

### Infrastructure classes

Logging - Logger.cs and TopicLogger.cs
Caching - LocalCache.cs
Async - AsyncFlow.cs
Asp.Net - AnyHandler.cs


### Concerns encapsulated

- AnyHandler.cs and SrvRequest.cs encapsulate Asp.Net interaction
- Api.cs encapsulates API parameters and routing 
- ThumbnailOp.cs encapsulates flow of a single thumbnail API
- LocalCache.cs encapsulates caching of the service
- AsyncFlow.cs encapsulates asynchronous programing model


### Techniques used in solution

#### Programing to interface

Programming to interface has implementation benefits on its own. But its real glory is expressing the building blocks of a problem domain in a short and concise way. With interfaces it is a matter of seconds to see if a class represents a decoupled concern and stands for the SOLID principles.

#### Separation of concerns

The major cause of spaghetti code, IMHO, not the lack of technical skills of a programmer, but wrong decomposition of a problem domain into properly decoupled concerns which then may become lego blocks to form any solution within that problem domain.






