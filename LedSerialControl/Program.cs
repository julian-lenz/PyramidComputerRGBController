using System;
using System.IO.Ports;
using System.Threading;
using LedSerialControl;

class Program
{

    static void Main()
    { 
        const int fadetime = 1; 
        var rgbController = new RGBController("COM4");
        rgbController.SetColorRgbw(0, 0, 0, 0);
        Thread.Sleep(500);
        rgbController.SetFlashingColors(RGBController.ColorNames.Yellow, RGBController.ColorNames.White);
        rgbController.SetFlashingPeriod(30);
        rgbController.SetFlashing(true);
        Thread.Sleep(6000);
        rgbController.SetFlashingPeriod(10);
        Thread.Sleep(3000);
        rgbController.SetFlashing(false);
        for (int i = 0; i < 1; i++)
        {
            // Start with red
            byte r = 255, g = 0, b = 0;
           
            // Fade from red to blue
            for (; b < 255; b++)
            {
                rgbController.SetColorRgbw(r, g, b, 0);
                Thread.Sleep(fadetime);
            }
            for (; r > 0; r--)
            {
                rgbController.SetColorRgbw(r, g, b, 0);
                Thread.Sleep(fadetime);
            }
            // Increase green to 255
            for (; g < 255; g++)
            {
                rgbController.SetColorRgbw(r, g, b, 0);
                Thread.Sleep(fadetime);
            }
            for (; b > 0; b--)
            {
                rgbController.SetColorRgbw(r, g, b, 0);
                Thread.Sleep(fadetime);
            }
            // Increase red to 255
            for (; r < 255; r++)
            {
                rgbController.SetColorRgbw(r, g, b, 0);
                Thread.Sleep(fadetime);
            }
            for (; g > 0; g--)
            {
                rgbController.SetColorRgbw(r, g, b, 0);
                Thread.Sleep(fadetime);
            }
        }

        rgbController.SetColor(RGBController.ColorNames.Off);


    }
}