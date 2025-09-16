using System;
using Code.game;
using gnet_csharp;
using Google.Protobuf;

namespace Code.network
{
    internal class ClientConnection
    {
        private long m_LastPingTimestamp; // 上一次ping的时间戳
        public const int PingInterval = 5; // 几秒钟ping一次

        public IConnection m_Connection;

        public ClientConnection(IConnection connection)
        {
            m_Connection = connection;
            m_Connection.Tag = this;
            m_Connection.OnConnected = onConnected;
            m_Connection.OnClose = onClose;
        }

        // NOTE: 在网络线程中执行,不要调用unity的接口!!!
        private void onConnected(IConnection connection, bool success)
        {
            // Console.WriteLine("onConnected success:" + success);
            if(Client.Instance.Player == null && !string.IsNullOrEmpty(Login.s_AccountName))
            {
                // 自动账号登录
                Login.AutoLogin();
            }
        }

        // NOTE: 在网络线程中执行,不要调用unity的接口!!!
        private void onClose(IConnection connection)
        {
            // Console.WriteLine("onClose");
        }

        // 发protobuf消息,会自动查找对应的消息号
        public bool Send(IMessage message)
        {
            ushort cmdId = PacketCommandMapping.GetCommandByProto(message);
            if (cmdId == 0)
            {
                return false;
            }
            return m_Connection.Send(cmdId, message);
        }

        public void AutoPing()
        {
            if (m_Connection == null || !m_Connection.IsConnected())
            {
                return;
            }
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - m_LastPingTimestamp >= PingInterval)
            {
                m_LastPingTimestamp = now;
                Send(new Gserver.HeartBeatReq
                {
                    Timestamp = now,
                });
            }
        }
        
        private IPacket PopPacket()
        {
            switch (m_Connection)
            {
                case TcpConnection tcpConnection:
                    return tcpConnection.PopPacket();
                case WsConnection wsConnection:
                    return wsConnection.PopPacket();
                default:
                    return null;
            }
        }

        public void ProcessPackets()
        {
            while (Client.Instance.IsRunning)
            {
                var packet = PopPacket();
                if (packet == null)
                {
                    return;
                }
                if (packet.Message() == null)
                {
                    Console.WriteLine("recv null message, err:" + packet.ErrorCode() + " cmd:" + packet.Command());
                    continue;
                }
                // 消息回调
                if (!HandlerRegister.OnRecvPacket(packet, Client.Instance.Player))
                {
                    var descriptor = PacketCommandMapping.GetMessageDescriptorByCommand(packet.Command());
                    if (descriptor is { Name: "HeartBeatRes" })
                    {
                        continue; // 心跳包不打印
                    }
                    // 没注册的消息
                    if (packet.ErrorCode() > 0)
                    {
                        Console.WriteLine("unhandled err:" + packet.ErrorCode() + " name:" + descriptor.Name + " msg:" + packet.Message());
                    }
                    else
                    {
                        Console.WriteLine("unhandled cmd:" + packet.Command() + " name:" + descriptor.Name + " msg:" + packet.Message());
                    }
                }
            }
        }
    }
}
