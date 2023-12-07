using System;
using System.IO.Ports;
using System.Threading;
using LedSerialControl;

class Program
{

    static void Main()
    {
       var rgbController = new LedSerialControl.RGBController("COM4");
       rgbController.StartFlashing();
    }
}