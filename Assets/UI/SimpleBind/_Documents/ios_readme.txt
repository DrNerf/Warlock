Our goal is to design Simple Bind in such a way that it will be compatible will all platforms that Unity supports.
  
Currently, Simple Bind has been tested on Windows, OS X, Android and iOS.

However, iOS has some specific requirements.  

When compiling for iOS you must set the following in the Player Settings:

 - Configuration: Scripting Backend -> Mono2x
 - Optimization: Api Compatability Level -> .NET 2.0 Subset
 
Without these two setting on iOS Simple Bind may compile and run but it will not function properly.

We are continuing to investigate this issue and hope to find ways to support but IL2CPP and the full .Net 2.0 API in the future.
