# unity_client
unity client for [gserver](https://github.com/fish-tennis/gserver)


## 演示功能
- 网络协议的消息号自动生成
- 消息回调的自动注册
- 客户端支持TCP和WebSocket
- 配置数据管理模块
- 采用Entity-Component设计,模块解耦
- 通用且扩展性强的条件接口
- 任务模块,演示了如何实现一个通用且扩展性强的任务系统[设计文档](/Design_Quest.md)
- 兑换模块,演示了如何实现一个通用且扩展性强的兑换功能
- 活动模块,演示了如何设计一个通用且支持扩展的活动模块
- [开发中]背包模块,演示了如何设计一个通用且支持扩展的容器模块

## 测试命令(GM命令)
控制台输入字符串以@开头 表示向服务器发送测试命令,如
```
@AddExp 100
```
gserver目前支持的测试命令: https://github.com/fish-tennis/gserver/blob/main/game/test_cmd.go

- 加经验: @AddExp 经验值
- 加物品: @AddItem 物品配置id 物品数量[可选] 限时秒数[可选]
- 完成所有任务: @FinishQuest all
- 完成某个任务: @FinishQuest 任务配置id
- 兑换请求: @Exchange 兑换配置id 数量
- 添加所有活动: @AddActivity all
- 添加某个活动: @AddActivity 活动配置id
- 模拟一个战斗事件: @Fight 是否是Pvp[可选] 是否获胜[可选]
- 分发一个事件: @FireEvent 事件名称 事件字段名 事件字段值

## 配置表和proto
https://github.com/fish-tennis/gserver
- 配置表及导表工具: gserver/excel
- proto及生成工具: gserver/proto

## unity版本
演示代码用的Unity2022.3.6

## 其他版本的客户端
- c#控制台客户端[cshap_client](https://github.com/fish-tennis/cshap_client)
- golang测试客户端[gtestclient](https://github.com/fish-tennis/gtestclient)
