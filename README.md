# com2com

开发工具 vs2022  
开发环境 .NET 8.0  
支持跨平台

[Gitee](https://gitee.com/bonn_admin/com2com)
[Github](https://github.com/bonn-admin/com2com)

[下载最新版本](https://gitee.com/bonn_admin/com2com/releases/latest)


## 系统结构图
![](doc/系统结构图.png '系统结构图')

## 运行效果图
![](doc/运行界面2.png '系统结构图')


# 开发初衷

有一台串口设备在客户那边，客户接在他本地的工控机上，我这边远程开发调试。开始是模拟数据包开发的，但感觉这样仍然不太方便。所以就想找一个工具能够将设备的数据转发到我这里，方便开发。
暂时没有找到合适的工具，所以就自己临时开发一个，方便自己使用，目前基本满足使用，后期再慢慢完善。

# 使用方法

1. 将 com2com.exe 复制到设备连接的电脑主机上，和本地开发机，各一份。
2. 设备连接的电脑（称为 A 主机），能够用串口工具正常通讯。
3. 开发主机（称为 B 主机），需要使用虚拟串口工具，模拟出一对串口。比如 com1 和 com101。
4. 在 A 主机上运行 com2com.exe com1 9600 a b，COM1 是和设备通讯的串口号，9200 是波特率。后面的 a,b 为密钥对。前面的 a 表示自己，后面 b 表示要发给谁。
5. 在 B 主机上运行 com2com.exe com101 9600 b a，此时主机打开 com1 串口，就可以正常通讯了。主机可以认为 com1 接的就是远程的设备。

# 原理介绍

利用 mqtt 服务器做中转，设备侧 A 主机收到的数据发送到 mqtt 服务器上。B 主机通过订阅主题，可以收到相应的数据，再通过虚拟串口发给 B 主机的串口上。

公网免费开放的 mqtt 服务器 
```bash
broker.mqttdashboard.com  
broker.emqx.io
```
# 使用示例

分别在开发机和设备主机执行下面的指令

开发机
```bash
com2com.exe portName=com101 baudRate=9600 sendKey=aaaaaaa rcvKey=bbbbbb mqttServer=broker.emqx.io
```

设备主机  
```bash
com2com.exe portName=com1 baudRate=9600 sendKey=bbbbbb rcvKey=aaaaaaa mqttServer=broker.emqx.io
```

# 参数说明

com2com.exe 是主程序  
com1 为本地端口号，在设备侧对应的是连接设备的串口号，在主机侧对应的是虚拟串口。  
9600 为波特率  
a b 这是两个密钥对，必须成对出现。在设备侧是 a b，在开发机侧就是 b a。字符串随意起。但是最好是唯一的，如果其他人也使用同样的密钥对，就也能接收到你的数据了。
