﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using Code.cfg;
using Code.network;
using gnet_csharp;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Code.game
{
    // 单件模式的客户端对象,放一些全局数据
    internal class Client
    {
        private static Client _instance = null;
        // 是否使用网关模式,使用网关模式时,客户端只需要连接网关服务器,由网关服务器转发消息给后端服务器
        // 使用网关模式时,连接登录服务器,可以不使用http
        // 暂时强制使用网关模式
        public static bool m_IsUseGateMode = true;

        public ConnectionConfig m_ConnectionConfig;
        public ICodec m_Codec;
        public ClientConnection m_Connection;
        public bool IsRunning = false;
        private ConcurrentQueue<string> m_InputCmds = new ConcurrentQueue<string>();

        // 本机玩家
        public Player Player { get; set; }

        private Client() { }
        public static Client Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Client();
                }
                return _instance;
            }
        }

        public void Init(string dataPath)
        {
            // 消息号映射
            PacketCommandMapping.InitCommandMappingFromFile(dataPath + "/cfgdata/message_command_mapping.json");
            // 注册消息回调
            HandlerRegister.RegisterMethodsForClass(typeof(Login), "");
            HandlerRegister.RegisterMethodsForPlayer();
            // 加载配置数据
            DataMgr.Load(dataPath + "/cfgdata/");
            Helper.AfterLoad(); // 预处理配置数据

            // 网络参数配置
            m_ConnectionConfig = new ConnectionConfig
            {
                RecvBufferSize = 1024 * 100,
                RecvTimeout = 3000,
                WriteTimeout = 3000
            };
        }

        // 连接服务器
        // 如果address是127.0.0.1:10001,表示使用tcp
        // 如果address是ws://127.0.0.1:10001/ws或wss://127.0.0.1:10001/wss,则表示使用Websocket
        public bool Connect(string address)
        {
            // 根据服务器地址来自动检查是否使用websocket
            var connectionMode = "";
            if (address.StartsWith("ws://"))
            {
                connectionMode = "ws";
            }
            else if (address.StartsWith("wss://"))
            {
                connectionMode = "wss";
            }
            IConnection conn = null;
            // 默认使用TcpConnection
            if (string.IsNullOrEmpty(connectionMode) || connectionMode == "tcp")
            {
                m_Codec = new ProtoCodec();
                m_ConnectionConfig.Codec = m_Codec;
                conn = new TcpConnection(m_ConnectionConfig, 1);
            }
            else
            {
                // websocket ws/wss
                if (connectionMode == "wss")
                {
                    m_ConnectionConfig.InsecureSkipVerify = true;
                }
                m_Codec = new SimpleProtoCodec();
                m_ConnectionConfig.Codec = m_Codec;
                conn = new WsConnection(m_ConnectionConfig, 1);
            }
            PacketCommandMapping.RegisterCodec(m_Codec); // 自动注册所有消息
            m_Connection = new ClientConnection(conn);
            IsRunning = true;
            return m_Connection.m_Connection.Connect(address);
        }

        public void Update()
        {
            m_Connection?.ProcessPackets();
            m_Connection?.AutoPing();
        }

        public void Run()
        {
            while (IsRunning)
            {
                if (m_InputCmds.TryDequeue(out string cmd))
                {
                    OnCommand(cmd);
                }
                Update();
                Thread.Sleep(50);
            }
        }

        public void Shutdown()
        {
            m_Connection?.m_Connection.Close();
        }

        // 从其他线程收到cmd
        public void RecvCommand(string cmd)
        {
            m_InputCmds.Enqueue(cmd);
        }

        // 测试命令
        public void OnCommand(string cmd)
        {
            Console.WriteLine("OnCommand:" + cmd);
            var cmdArgs = cmd.Split(' ');
            if (cmdArgs.Length == 0)
            {
                return;
            }
            // @开头表示是gm命令 如@AddExp 100
            if (cmdArgs[0].StartsWith("@"))
            {
                m_Connection.Send(new Gserver.TestCmd
                {
                    Cmd = cmd.Substring(1),
                });
            }
            else
            {
                // 通用的protobuf消息,进行动态的组装
                // 格式: messageName fieldName fieldValue fieldName fieldValue ...
                var messageName = cmdArgs[0];
                var messageDescriptor = PacketCommandMapping.GetMessageDescriptor(messageName);
                if (messageDescriptor == null)
                {
                    Console.WriteLine("not find message:" + messageName);
                    return;
                }
                // 创建一个新消息,并对字段进行赋值
                var message = Activator.CreateInstance(messageDescriptor.ClrType) as IMessage;
                for (int i = 1; i < cmdArgs.Length && i+1 < cmdArgs.Length; i+=2)
                {
                    var fieldName = cmdArgs[i];
                    var fieldValue = cmdArgs[i+1];
                    var fieldDescriptor = messageDescriptor.FindFieldByName(fieldName);
                    if (fieldDescriptor == null )
                    {
                        Console.WriteLine("not find fieldName:" + fieldName);
                        continue;
                    }
                    // 暂不支持repeated和map类型的字段的动态赋值
                    if(fieldDescriptor.IsRepeated || fieldDescriptor.IsMap)
                    {
                        Console.WriteLine("not support repeated or map field:" + fieldName);
                        continue;
                    }
                    switch(fieldDescriptor.FieldType)
                    {
                        case FieldType.String:
                            fieldDescriptor.Accessor.SetValue(message, fieldValue);
                            break;
                        case FieldType.Int32:
                            var i32 = Int32.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, i32);
                            break;
                        case FieldType.UInt32:
                            var u32 = UInt32.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, u32);
                            break;
                        case FieldType.Int64:
                            var i64 = Int64.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, i64);
                            break;
                        case FieldType.UInt64:
                            var u64 = UInt64.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, u64);
                            break;
                        case FieldType.Bool:
                            var lowerValue = fieldValue.ToLower();
                            var b = (lowerValue == "1" || lowerValue == "true");
                            fieldDescriptor.Accessor.SetValue(message, b);
                            break;
                        case FieldType.Float:
                            var f = float.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, f);
                            break;
                        case FieldType.Double:
                            var d = double.Parse(fieldValue);
                            fieldDescriptor.Accessor.SetValue(message, d);
                            break;
                        default:
                            Console.WriteLine("unsupported FieldType:" + fieldDescriptor.FieldType);
                            break;
                    }
                }
                if (!m_Connection.Send(message))
                {
                    Console.WriteLine("send err:" + messageName);
                }
            }
        }

        public static bool Send(IMessage message)
        {
            if (Instance.m_Connection == null)
            {
                return false;
            }
            return Instance.m_Connection.Send(message);
        }
    }
}
