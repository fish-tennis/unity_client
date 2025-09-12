using cshap_client.game;
using gnet_csharp;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace cshap_client.network
{
    public class MethodData
    {
        public MethodInfo Method;
        public bool IsPlayer; // 玩家的消息回调
        public string ComponentName; // 玩家组件名,为空表示是Player类

        public MethodData Next; // 链表,同一个消息可以注册多个回调
    }
    
    // 消息回调注册
    internal class HandlerRegister
    {
        // 规范: 消息回调函数名格式: OnXyz(Xyz res) 或者 OnXyz(Xyz res, int errCode)
        public const string HandlerMethodPrefix = "On";

        // 注册的消息回调
        private static Dictionary<Type, MethodData> m_Handlers = new Dictionary<Type, MethodData>();

        // 扫描一个类的所有函数,自动注册消息回调
        // 消息回调函数名格式:
        //   OnXyz(Xyz res)
        //   OnXyz(Xyz res, int errCode)
        // 支持静态函数,玩家模块的成员函数
        public static void RegisterMethodsForClass(Type type, string componentName)
        {
            // 获取所有方法
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                if (!method.Name.StartsWith(HandlerMethodPrefix))
                {
                    continue;
                }
                var messageName = method.Name.Substring(HandlerMethodPrefix.Length);
                //var descriptor = PacketCommandMapping.GetMessageDescriptor(messageName);
                //if (descriptor == null)
                //{
                //    continue;
                //}
                if (method.GetParameters().Length < 1 || method.GetParameters().Length > 2)
                {
                    continue;
                }
                var messageParamInfo = method.GetParameters()[0];
                if (!typeof(IMessage).IsAssignableFrom(messageParamInfo.ParameterType))
                {
                    continue;
                }
                if (messageParamInfo.ParameterType.Name != messageName)
                {
                    Console.WriteLine("messageName not matched:" + method.Name + " messageName:" + messageName + " paramName:" + messageParamInfo.ParameterType.Name);
                    continue;
                }
                if (method.GetParameters().Length > 1)
                {
                    var errCodeParamType = method.GetParameters()[1].ParameterType;
                    if (errCodeParamType != typeof(int))
                    {
                        Console.WriteLine("errorCode not matched:" + method.Name + " messageName:" + messageName + " paramType:" + errCodeParamType);
                        continue;
                    }
                }
                var methodData = new MethodData
                {
                    Method = method,
                    ComponentName = componentName,
                };
                if(type == typeof(Player))
                {
                    methodData.IsPlayer = true;
                }
                if (!string.IsNullOrEmpty(componentName))
                {
                    methodData.IsPlayer = true;
                }
                if (m_Handlers.TryGetValue(messageParamInfo.ParameterType, out var existMethodData))
                {
                    existMethodData.Next = methodData;
                }
                else
                {
                    m_Handlers.Add(messageParamInfo.ParameterType, methodData);
                }
                Debug.Log("RegisterHandler:" + method.Name + " message:" + messageName + " type:" + type.Name);
            }
        }

        // 扫描玩家类及其组件的所有函数,自动注册消息回调
        public static void RegisterMethodsForPlayer()
        {
            Player player = new Player(0);
            player.InitComponents();
            // 扫描玩家类上的消息回调
            RegisterMethodsForClass(player.GetType(), "");
            // 扫描玩家组件上的消息回调,组件必须在player.InitComponents()里添加上了
            player.RangePlayerComponents((BasePlayerComponent component) =>
            {
                RegisterMethodsForClass(component.GetType(), component.GetName());
            });
        }
        
        // 检查是否继承于某个泛型类
        public static bool IsSubclassOfRawGeneric(Type type, Type genericBase)
        {
            if (type == null || genericBase == null)
                return false;
            // 遍历类型继承链
            while (type != null && type != typeof(object))
            {
                // 检查当前类型是否为泛型类型并与目标泛型定义匹配
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericBase == cur)
                    return true;
                // 继续检查基类
                type = type.BaseType;
            }
            return false;
        }

        // 收到消息,调用注册的回调
        public static bool OnRecvPacket(IPacket packet, Player player)
        {
            var message = packet.Message();
            if (!m_Handlers.ContainsKey(message.GetType()))
            {
                return false;
            }
            var methodData = m_Handlers[message.GetType()];
            if (methodData == null)
            {
                Console.WriteLine("not find method, message" + message.GetType().Name);
                return false;
            }
            while (methodData != null)
            {
                try
                {
                    object[] parameters = new object[] { message };
                    var method = methodData.Method;
                    if (method.GetParameters().Length == 2)
                    {
                        parameters = new object[] { message, (int)packet.ErrorCode() };
                    }
                    if (method.IsStatic)
                    {
                        method.Invoke(null, parameters); // 静态函数
                    }
                    else
                    {
                        // 非静态函数,是玩家类或玩家组件或ViewModel上的成员函数
                        // 玩家或玩家组件上的成员函数
                        if (methodData.IsPlayer)
                        {
                            if (string.IsNullOrEmpty(methodData.ComponentName))
                            {
                                method.Invoke(player, parameters); // 玩家类上的成员函数
                            }
                            else
                            {
                                var componet = player.GetComponentByName(methodData.ComponentName);
                                if (componet == null)
                                {
                                    Console.WriteLine("not find component, message" + message.GetType().Name + " component:" + methodData.ComponentName);
                                    return false;
                                }
                                method.Invoke(componet, parameters); // 玩家组件上的成员函数
                            } 
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("OnRecvPacketErr: message" + message.GetType().Name + " e:" + e.Message);
                }
                methodData = methodData.Next;
            }
            return true;
        }
    }
}
