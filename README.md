# 02 DataPipesTransfer

c#/Pips/NamedPipeClientStream/NamedPipeServerStream

**实现功能：**
- 通过NamedPipe传输数据
- pipeSecurity.SetAccessRule
- 服务器端多线程
- 客户端即时生成client实例访问服务器端
- 实现check update、data list、get data完整三步

**性能**
- 序列化待传输的对象时，大对象有一次性序列化开销。
- 反序列化同样。
- 序列化还涉及装箱拆箱问题？
- 32/64位操作系统传输时指定数据长度对应int32/int64。

**使用方法**
- 解决方案下DataSrv是服务端，应先运行；
- 运行DataSrv后，点击Form上的start server按钮；
- 再运行解决方案下AnalyzeServ，此为客户端；
- AnalyzeServ的Form上三个按钮对应check update、data list、get data；
- 服务端接受请求后传输的是假数据或空对象；
- 点击按钮没有任何可见反应，需自行设置断点检查数据传输效果。