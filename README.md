# com2com
开发工具 vs2022  
开发环境 .NET 8.0  
支持跨平台

# 开发初衷
有一台串口设备在客户那边，客户接在他本地的工控机上，我这边远程开发调试。开始是模拟数据包开发的，但感觉这样仍然不太方便。所以就想找一个工具能够将设备的数据转发到我这里，方便开发。
暂时没有找到合适的工具，所以就自己临时开发一个，方便自己使用，目前基本满足使用，后期再慢慢完善。


# 使用方法
1. 将com2com.exe复制到设备连接的电脑主机上，和本地开发机，各一份。  
2. 设备连接的电脑（称为A主机），能够用串口工具正常通讯。  
3. 开发主机（称为B主机），需要使用虚拟串口工具，模拟出一对串口。比如com1和com101。  
4. 在A主机上运行com2com.exe com1 9600 a b，COM1是和设备通讯的串口号，9200是波特率。后面的a,b为密钥对。前面的a表示自己，后面b表示要发给谁。
5. 在B主机上运行com2com.exe com101 9600 b a，此时主机打开com1串口，就可以正常通讯了。主机可以认为com1接的就是远程的设备。


# 原理介绍
利用mqtt服务器做中转，设备侧A主机收到的数据发送到mqtt服务器上。B主机通过订阅主题，可以收到相应的数据，再通过虚拟串口发给B主机的串口上。

公网免费开放的mqtt服务器
broker.mqttdashboard.com
broker.emqx.io


# 使用示例
分别在开发机和设备主机执行下面的指令

开发机  
com2com.exe portName=com101 baudRate=9600 sendKey=aaaaaaa rcvKey=bbbbbb mqttServer=broker.emqx.io

设备主机  
com2com.exe portName=com1 baudRate=9600 sendKey=bbbbbb rcvKey=aaaaaaa mqttServer=broker.emqx.io

# 参数说明

com2com.exe是主程序  
com1为本地端口号，在设备侧对应的是连接设备的串口号，在主机侧对应的是虚拟串口。  
9600为波特率  
a b这是两个密钥对，必须成对出现。在设备侧是a b，在开发机侧就是b a。字符串随意起。但是最好是唯一的，如果其他人也使用同样的密钥对，就也能接收到你的数据了。  




