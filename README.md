# .Net Class for RGBController by Pyramid Computer
---
#### Helperclass for controlling the RGB light controller from Pyramid Computer.

Uses System.IO.Ports, therefore **only running on Windows**.

This is part of a group project for the bachelor1 thesis where we experiment with using UPOS (Pos for .NET 1.14) for controlling the hardware of a *[self checkout terminal](https://pyramid-computer.com/polytouch/flex/sco-self-checkout-kiosk/)*.

The status light on top of the terminal, which is used for displaying the status of the terminal (occupied, free, help), uses a custom controlling unit by Pyramid Computer.  
The controller is internally accessed via serial communication. This project provides a class which eases the use of the lightcontroller.

Further steps are to wrap the controller class in a Pos for .Net Service Object and create a simple interface for demonstration and choosing the right COM port.
