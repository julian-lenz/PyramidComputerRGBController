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
    private static readonly Dictionary<ColorNames, byte[]> colorDictionary = new Dictionary<ColorNames, byte[]>
    {
        { ColorNames.Red, new byte[] { 255, 0, 0 } } ,
        { ColorNames.Green, new byte[] { 0, 255, 0 } },
        { ColorNames.Blue, new byte[] { 0, 0, 255} },
        { ColorNames.White, new byte[] { 255, 255, 255 } },
        { ColorNames.Yellow, new byte[] { 255, 255, 0 } },
        { ColorNames.Orange, new byte[] { 255, 165, 0 } },
        { ColorNames.Cyan, new byte[] { 0, 255, 255 } },
        { ColorNames.Magenta, new byte[] { 255, 0, 255 } },
        { ColorNames.Off, new byte[] { 0, 0, 0 } }
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
    
    public void SetFlashingPeriod(byte period)
    {
        SendCommand(_flashingPeriod, period);
    }
    
    /// <summary>
    /// Sets the color of the RGB LED strip.
    /// </summary>
    /// <param name="color">The color to set the LED strip to. This is an enum value.</param>
    public void SetColour(ColorNames color)
    {
        byte[] values = colorDictionary[color];
        for(var i = 0; i<4; i++)
            values[4+i] = 0; // fill with 0, see Documentation
        SendCommand(_SetColorB0, values);
    }
    
    public void SetColorRgbw(byte r = 0, byte g = 0, byte b = 0, byte w = 0)
    {
        byte red = r;
        byte green = g;
        byte blue = b;
        byte white = w;
        byte[] values = { red, green, blue, white, 0, 0, 0, 0 };
        SendCommand(_SetColorB0, values);
    }
    
    public void SetFlashingColors(ColorNames color1, ColorNames color2)
    {
        byte[] values = ConcatArraysRef(colorDictionary[color1], colorDictionary[color2]);
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
        byte[] message = ConcatArraysRef(_startbits, new byte[] { command }, arguments, _endbits);
        serialPort.Write(message, 0, message.Length);
        Console.Write("Sent Message: " + ByteArrayToHexString(message));
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
    private static byte[] ConcatArraysRef(params byte[][] arrays)
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

