namespace Com2Com;

/// <summary>
/// 
/// </summary>
public class Log {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    public void Debug(string msg) {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    public void Info(string msg) {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    public void Error(string msg) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {msg}");
        Console.ResetColor();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public void Error(Exception e) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {e.ToString()}");
        Console.ResetColor();
    }

}