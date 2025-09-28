using System.IO.Ports;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace Com2Com;

public class Program {
    private static string portName = "COM1";
    private static int baudRate = 9600;
    private static string sendKey = "SEND_KEY";
    private static string rcvKey = "RECEIVE_KEY";
    private static string mqttServer = "broker.emqx.io";
    private static int mqttPort = 0;
    private static string mqttUserName = "";
    private static string mqttPwd = "";

    private static Log Log = new();

    public static void Main(string[] args) {
        Log.Debug(string.Join(" ", args));

        const int paramCont = 5;
        if (args.Length < paramCont) {
            Log.Error($"参数数量必须是{paramCont}个，请查看说明文档。");
            return;
        }

        if (args.Length > 0) {
            foreach (string arg in args) {
                if (arg.StartsWith("portName")) {
                    portName = arg.Replace("portName=", "");
                }
                else if (arg.StartsWith("baudRate")) {
                    baudRate = Convert.ToUInt16(arg.Replace("baudRate=", ""));
                }
                else if (arg.StartsWith("sendKey")) {
                    sendKey = arg.Replace("sendKey=", "");
                }
                else if (arg.StartsWith("rcvKey")) {
                    rcvKey = arg.Replace("rcvKey=", "");
                }
                else if (arg.StartsWith("mqttServer")) {
                    mqttServer = arg.Replace("mqttServer=", "");
                }
                else if (arg.StartsWith("mqttUserName")) {
                    mqttUserName = arg.Replace("mqttUserName=", "");
                }
                else if (arg.StartsWith("mqttPwd")) {
                    mqttPwd = arg.Replace("mqttPwd=", "");
                }
                else if (arg.StartsWith("help")) {
                    string[] strings = [
                        "",
                        "",
                    ];
                    foreach (string s in strings) {
                        Console.WriteLine(s);
                    }
                    return;
                }
            }
        }

        // 创建一个 MqttClient 对象，端口默认1883，这里暂时没有办法指定端口
        MqttClient mqttClient = new(mqttServer);

        // 配置 MQTT 客户端参数
        string clientId = Guid.NewGuid().ToString();
        if (string.IsNullOrWhiteSpace(mqttUserName)) {
            mqttClient.Connect(clientId);
        }
        else {
            mqttClient.Connect(clientId, mqttUserName, mqttPwd);
        }

        // 订阅 MQTT 主题
        // QOS_LEVEL_AT_MOST_ONCE 表示至多一次传输
        // QOS_LEVEL_AT_LEAST_ONCE 表示至少一次传输
        // QOS_LEVEL_EXACTLY_ONCE 表示正好一次传输
        mqttClient.Subscribe(new[] { "com2com/" + rcvKey }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });


        // 创建一个 SerialPort 对象
        SerialPort serialPort = new();

        // 接收 MQTT 消息
        mqttClient.MqttMsgPublishReceived += (sender, e) => {
            // 将消息内容输出到串口
            Log.Debug("接收：" + BitConverter.ToString(e.Message).Replace("-", " "));
            serialPort.Write(e.Message, 0, e.Message.Length);
        };

        // 配置串口参数
        serialPort.PortName = portName; // 串口号
        serialPort.BaudRate = baudRate; // 波特率
        serialPort.Parity = Parity.None; // 校验位
        serialPort.DataBits = 8; // 数据位
        serialPort.StopBits = StopBits.One; // 停止位

        // 打开串口
        try {
            serialPort.Open();
            Log.Info($"{portName} 打开成功");
        }
        catch (Exception ex) {
            Log.Error("Error: " + ex.Message);
            return;
        }

        // 接收串口数据
        while (true) {
            try {
                // 等待串口有数据可读取
                if (serialPort.BytesToRead > 0) {
                    Thread.Sleep(200);
                    //定义一个字节数组buffer，存储读取到的数据
                    byte[] buffer = new byte[serialPort.BytesToRead];

                    //从串口中读取buffer.Length个字节到buffer数组中
                    serialPort.Read(buffer, 0, buffer.Length);

                    // 处理读取到的数据
                    Log.Info("发送：" + BitConverter.ToString(buffer).Replace("-", " "));

                    // 发布一条消息
                    //byte[] data = Encoding.UTF8.GetBytes(msg);
                    mqttClient.Publish("com2com/" + sendKey, buffer, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                }
            }
            catch (Exception ex) {
                Log.Error("Error: " + ex.Message);
                break;
            }
            Thread.Sleep(1000);
        }

        // 关闭串口
        serialPort.Close();
        Console.ReadKey(true);
    }
}

public class Log {

    public void Debug(string msg) {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}");
    }

    public void Info(string msg) {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}");
    }

    public void Error(string msg) {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}");
    }

    public void Error(Exception e) {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {e.ToString()}");
    }

}
