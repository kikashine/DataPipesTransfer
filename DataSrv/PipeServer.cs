using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using StockToolKit.Common;
using System.Runtime.Serialization.Formatters.Binary;

namespace StockToolKit.DataSrv
{
    public class PipeServer
    {
        private string opration = "";

        public void Start()
        {
            int threadcount = 4;

            for (int i = 0; i < threadcount; i++)
            {

                Thread jobThread = new Thread(new ParameterizedThreadStart(SendData));
                jobThread.Name = "SendData" + i.ToString();
                jobThread.Start(i);
            }
        }

        private void SendData(object threadNum)
        {
            //bool inuse = false;
            while (true)
            {
                //using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut, 10))
                // Prepare the security attributes (the pipeSecurity parameter in  
                // the constructor of NamedPipeServerStream) for the pipe. This  
                // is optional. If pipeSecurity of NamedPipeServerStream is null,  
                // the named pipe gets a default security descriptor.and the  
                // handle cannot be inherited. The ACLs in the default security  
                // descriptor of a pipe grant full control to the LocalSystem  
                // account, (elevated) administrators, and the creator owner.  
                // They also give only read access to members of the Everyone  
                // group and the anonymous account. However, if you want to  
                // customize the security permission of the pipe, (e.g. to allow  
                // Authenticated Users to read from and write to the pipe), you  
                // need to create a PipeSecurity object. 
                PipeSecurity pipeSecurity = null;
                pipeSecurity = CreateSystemIOPipeSecurity();

                using (NamedPipeServerStream pipeServer =
                    new NamedPipeServerStream("testpipe",
                    PipeDirection.InOut,
                    -1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.None,
                    4096,
                    4096,
                    pipeSecurity,
                    HandleInheritability.None
                    )
                    )
                {
                    pipeServer.WaitForConnection();
                    //inuse = true;
                    Hashtable stock = null;
                    //Console.WriteLine("Client connected.");
                    try
                    {
                        // Read the request from the client. Once the client has
                        // written to the pipe its security token will be available.
                        using (BinaryWriter dw = new BinaryWriter(pipeServer))
                        using (BinaryReader dr = new BinaryReader(pipeServer))
                        {
                            //循环检查接收到的指令，并且做出对应处理
                            while (pipeServer.IsConnected)
                            {
                                string[] str = dr.ReadString().Split('_');
                                switch (str[0])
                                {
                                    //客户端将发送日期进行更新数据检查
                                    case "checkUpdate":
                                        dw.Write(Utility.getDataUpdate(0));
                                        pipeServer.Close();
                                        break;
                                    case "getData":
                                        stock = new Hashtable();
                                        stock.Add("StockCode", str[1]);
                                        stock.Add("MarketCode", str[2]);
                                        PipeDBReader dbr = new PipeDBReader(pipeServer, stock, 0);
                                        pipeServer.RunAsClient(dbr.GetData);
                                        pipeServer.Close();
                                        break;
                                    case "stockList":
                                        byte[] kd = ToBytes(Utility.GetStockList_industry2());
                                        dw.Write((long)kd.Length);
                                        dw.Flush();
                                        dw.Write(kd);
                                        dw.Flush();
                                        pipeServer.Close();
                                        break;
                                }
                                break;
                               
                            }

                        }
                    }
                    // Catch the IOException that is raised if the pipe is broken
                    // or disconnected.
                    catch (IOException e)
                    {
                        //Console.WriteLine("ERROR: {0}", e.Message);
                    }
                    if (pipeServer.IsConnected)
                    {
                        pipeServer.Close();
                    }
                }
                //inuse = false;
            }
        }

        /// <summary> 
        /// The CreateSystemIOPipeSecurity function creates a new PipeSecurity  
        /// object to allow Authenticated Users read and write access to a pipe,  
        /// and to allow the Administrators group full access to the pipe. 
        /// </summary> 
        /// <returns> 
        /// A PipeSecurity object that allows Authenticated Users read and write  
        /// access to a pipe, and allows the Administrators group full access to  
        /// the pipe. 
        /// </returns> 
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa365600(VS.85).aspx"/> 
        static PipeSecurity CreateSystemIOPipeSecurity()
        {
            PipeSecurity pipeSecurity = new PipeSecurity();

            // Allow Everyone read and write access to the pipe. 
            pipeSecurity.SetAccessRule(new PipeAccessRule("Authenticated Users",
                PipeAccessRights.FullControl, AccessControlType.Allow));

            // Allow the Administrators group full access to the pipe. 
            pipeSecurity.SetAccessRule(new PipeAccessRule("Administrators",
                PipeAccessRights.FullControl, AccessControlType.Allow));

            return pipeSecurity;
        }

        private byte[] ToBytes(Dictionary<string, StockInfo> sds)
        {
            byte[] leafMyClassByteTemp;
            MemoryStream leafMemoryStreamTemp = new MemoryStream();
            BinaryFormatter leafBinaryFormatterTemp = new BinaryFormatter();
            leafBinaryFormatterTemp.Serialize(leafMemoryStreamTemp, sds);
            leafMyClassByteTemp = leafMemoryStreamTemp.GetBuffer();
            return leafMyClassByteTemp;
        }
    }
}
