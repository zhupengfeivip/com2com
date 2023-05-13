using System;
using System.IO.Ports;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace Com2Com
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string portName = args[0];
            int baudRate = int.Parse(args[1]);
            string sendKey = args[2];
            string rcvKey = args[3];

            Console.WriteLine(string.Join(" ",args));

            // 创建一个 MqttClient 对象
            MqttClient mqttClient = new MqttClient("broker.mqttdashboard.com");

            // 配置 MQTT 客户端参数
            string clientId = Guid.NewGuid().ToString();
            mqttClient.Connect(clientId);

            // 订阅 MQTT 主题
            mqttClient.Subscribe(new[] { "com2com/" + rcvKey }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });


            // 创建一个 SerialPort 对象
            SerialPort serialPort = new SerialPort();

            // 接收 MQTT 消息
            mqttClient.MqttMsgPublishReceived += (object sender, MqttMsgPublishEventArgs e) =>
            {
                // 将消息内容输出到串口
                Console.WriteLine("接收：" + BitConverter.ToString(e.Message).Replace("-", " "));
                serialPort.Write(e.Message, 0, e.Message.Length);
            };

            // 配置串口参数
            serialPort.PortName = portName; // 串口号
            serialPort.BaudRate = baudRate; // 波特率
            serialPort.Parity = Parity.None; // 校验位
            serialPort.DataBits = 8; // 数据位
            serialPort.StopBits = StopBits.One; // 停止位

            // 打开串口
            try
            {
                serialPort.Open();
                Console.WriteLine(portName + "打开成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return;
            }

            // 接收串口数据
            while (true)
            {
                try
                {
                    // 等待串口有数据可读取
                    if (serialPort.BytesToRead > 0)
                    {
                        Thread.Sleep(200);
                        //定义一个字节数组buffer，存储读取到的数据
                        byte[] buffer = new byte[serialPort.BytesToRead];

                        //从串口中读取buffer.Length个字节到buffer数组中
                        serialPort.Read(buffer, 0, buffer.Length);

                        // 处理读取到的数据
                        Console.WriteLine("发送：" + BitConverter.ToString(buffer).Replace("-", " "));

                        // 发布一条消息
                        //byte[] data = Encoding.UTF8.GetBytes(msg);
                        mqttClient.Publish("com2com/" + sendKey, buffer, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
                Thread.Sleep(1000);
            }

            // 关闭串口
            serialPort.Close();
            Console.ReadKey(true);
        }
    }
}