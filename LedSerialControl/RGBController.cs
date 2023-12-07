/// This Class is used to control the RGB Light Controller from Pyramid Computers.
/// Author: Julian Lenzenweger
/// Date: 07.12.2023
///

using System.IO.Ports;


namespace LedSerialControl;

public class RGBController
{
    /// <summary>
    /// Enum representing the different colors that the LED strip can be set to.
    /// </summary>
    public enum ColorNames
    {
        Red,
        Green,
        Blue,
        White,
        Yellow,
        Orange,
        Cyan,
        Magenta,
        Off
    }

    /// <summary>
    /// Dictionary mapping ColorNames to their corresponding byte arrays.
    /// </summary>
    private static readonly Dictionary<ColorNames, byte[]> colorDictionaryRGBW = new Dictionary<ColorNames, byte[]>
    {
        { ColorNames.Red, new byte[] { 255, 0, 0, 0 } } ,
        { ColorNames.Green, new byte[] { 0, 255, 0, 0 } },
        { ColorNames.Blue, new byte[] { 0, 0, 255, 0 } },
        { ColorNames.White, new byte[] { 0, 0, 0, 255 } },
        { ColorNames.Yellow, new byte[] { 255, 255, 0, 0 } },
        { ColorNames.Orange, new byte[] { 255, 165, 0, 0 } },
        { ColorNames.Cyan, new byte[] { 0, 255, 255, 0 } },
        { ColorNames.Magenta, new byte[] { 255, 0, 255, 0 } },
        { ColorNames.Off, new byte[] { 0, 0, 0, 0 } }
    };
    
    private static int Baudrate { get; set; }
    private const int Databits = 8;
    private SerialPort serialPort;
    
    private byte[] _startbits = StringToByteArray("5AFF");
    private byte[] _endbits = StringToByteArray("A5");
    private byte _SetColorB0 = Convert.ToByte("CA", 16);
    private byte _changeFlashingColoursB0 = Convert.ToByte("D3", 16);
    private byte _modeB0 = Convert.ToByte("D6", 16);
    private byte _flashingPeriod = Convert.ToByte("E5", 16);
    private const byte _off = 0;
    private const byte _on = 1;
    
    public bool Flashing;
    
    public void StartFlashing()
    {
        if (!Flashing)
            Flashing = true;
        SendCommand(_modeB0, _on);
    }
    
    public void StopFlashing()
    {
        if (Flashing)
            Flashing = false;
        SendCommand(_modeB0, _off);
    }
    /// <summary>
    /// Sets the flashing period of the RGB LED strip.
    /// Each Step is about 27ms
    /// </summary>
    /// <param name="period">The period to set the LED strip to. This is a byte value.</param>
    public void SetFlashingPeriod(byte period)
    {
        SendCommand(_flashingPeriod, period);
    }
    
    /// <summary>
    /// Sets the color of the RGB LED strip.
    /// </summary>
    /// <param name="color">The color to set the LED strip to. This is an enum value.</param>
    public void SetColor(ColorNames color)
    {
        byte[] values = colorDictionaryRGBW[color];
        var argument = ConcatArrays(values, new byte[] { 0, 0, 0, 0 });
        SendCommand(_SetColorB0, argument);
    }
    
    public void SetColorRgbw( byte r = 0, byte g = 0, byte b = 0, byte w = 0)
    {
        byte[] values = { r, g, b, w, 0, 0, 0, 0 };
        SendCommand(_SetColorB0, values);
    }
    
    /// <summary>
    /// Sets the color of the RGBW Module in Percent
    /// </summary>
    /// <param name="r">Value of Red</param>
    /// <param name="g">Value of Green </param>
    /// <param name="b">Value of Blue</param>
    /// <param name="w">Value of White</param>
    public void SetColorRgbwPercent( short r = 0, short g = 0, short b = 0, short w = 0)
    {
        if (r > 100 || g > 100 || b > 100 || w > 100 || r < 0 || g < 0 || b < 0 || w < 0)
            throw new ArgumentException("Values must be between 0 and 100");
        byte red = (byte)(r*255/100);
        byte green =  (byte)(g*255/100);
        byte blue =  (byte)(b*255/100);
        byte white =  (byte)(w*255/100);
        byte[] values = { red, green, blue, white, 0, 0, 0, 0 };
        SendCommand(_SetColorB0, values);
    }
    
    public void SetFlashingColors(ColorNames color1, ColorNames color2)
    {
        byte[] values = ConcatArrays(colorDictionaryRGBW[color1], colorDictionaryRGBW[color2]);
        SendCommand(_changeFlashingColoursB0, values);
    }
    /// <summary>
    /// Constructor for RGBController
    /// </summary>
    /// <param name="portname">Commonly COMx, eg. COM4</param>
    public RGBController(string portname)
    {
        Baudrate = 9600;
        serialPort = new SerialPort(portname, Baudrate);
        serialPort.DataBits = Databits; // 8 data bits
        serialPort.StopBits = StopBits.One;
        serialPort.Parity = Parity.None;
        serialPort.Open();
    }

    ~RGBController()
    {
        serialPort.Close();
    }


    /// <summary>
    /// Sends a command to the RGB Controller. Adds start and end bits.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    private void SendCommand(byte command, byte[] arguments)
    {
        byte[] message = ConcatArrays(_startbits, new byte[] { command }, arguments, _endbits);
        serialPort.Write(message, 0, message.Length);
        Console.Write($"Sent Message: {ByteArrayToHexString(message)}\n");
    }
    private void SendCommand(byte command, byte argument)
    {
        SendCommand(command, new byte[] { argument });
    }
    
    /// <summary>
    /// Help function to concat multiple byte arrays
    /// </summary>
    /// <param name="arrays"></param>
    /// <returns></returns>
    private static byte[] ConcatArrays(params byte[][] arrays)
    {
        return arrays.SelectMany(arr => arr).ToArray();
    }
    
    /// <summary>
    /// Help function to convert a byte array to a hex string
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static string ByteArrayToHexString(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "");
    }
    
    /// <summary>
    /// Converts a Hex String to Array of Bytes
    /// </summary>
    /// <param name="hex"></param>
    /// <returns>Eschowissn</returns>
    private static byte[] StringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
}

