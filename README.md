# nf.protocol-flow

![flow.png](flow.png)
[flow.puml](flow.puml)

## introduce
this is sample protocol flow. When I making a game(in unity), I don't like to hard-coding for protocol class from message. I'm writing custom class generator, and protocol exporter for that work.

If you want to use [Akka.net](http://getakka.net/), check [Akka.Interfaced](https://github.com/SaladLab/Akka.Interfaced).

## basic Knowledge.
* [Rakefile](https://github.com/ruby/rake)
* [dotnet cli](https://www.microsoft.com/net/core)
* [Protobuf](https://developers.google.com/protocol-buffers/)


# flow

## example
```
$ rake --version
rake, version 10.4.2
$ dotnet --version
2.1.101
$ protoc --version
libprotoc 3.5.1
$ rake unity_project
$ rake test_server
```

## message

``` protocol-buffer

message QHello {
    int32 q1 = 1; /// hello description.
    int32 q2 = 2;
}

message RHello {
    int32 r1 = 1;
    int32 r2 = 2;
}
```


## server

``` python
@post('/')
def hello():
    r = hello_pb2.RHello()
    r.r1 = 11
    r.r2 = 22
    print(r)
    return r.SerializeToString()
```

## client
``` csharp
    // Hello.autogen.cs
    public interface IHello: IMessageSender
    {
        Task<Result<RHello, int>> Hello(QHello q);
        Task<Result<RHello, int>> Hello(System.Int32 Q1, System.Int32 Q2);
    }
```

``` csharp
        private async void Start()
        {
            ProtobufHttpSender sender = new ProtobufHttpSender(new Uri("http://127.0.0.1:8080/"), 3000);
            Result<RHello, int> t = await sender.Hello(1, 2);
            if (t.IsErr)
            {
                Debug.LogError(t.Err);
                return;
            }

            Debug.Log(t.Ok);
        }
```



## etc
* https://atom.io/packages/plantuml-viewer
* https://chrome.google.com/webstore/detail/plantuml-viewer/legbfeljfbjgfifnkmpoajgpgejojooj
