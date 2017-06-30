# HERO-Autonomous-Selector-Example
An autonomous selector for FRC using the HERO and a display module.

# Setup
FRC teams often struggle with selecting autonomous modes before a match. If a multiswitch fails or there's not enough time to hardcode a selection, there's no indicator of what autonomous program will run on the robot. Use this autonomous selector program to set up a HERO Development Board to select an autonomous and verify that the robot is recieving the information via the CAN bus.

To begin, simply download and extract the project, and open it in Visual Studio. If you haven't done so already, you'll need to install the development tools for the HERO from the HERO main page: http://www.ctr-electronics.com/hro.html#product_tabs_technical_resources

Next, wire the HERO into the CAN bus like any other CAN device usch as the Talon. Plug a Gadgeteer Display Module into the HERO on Port 8 (AOSX). 
![Wiring picture](Images/Auton_Selector_Wiring.jpg)
*Correct wiring for the Autonomous Selector.*

Use a mini-usb cable to connect the HERO to your computer, and run the program from Visual Studio. Make sure that you've followed the correct steps to upoad a program to the HERO, including selecting it in the .NET Framework tab's dropdown menu. If you have trouble, refer to the HERO documentation.
You should see a screen like the one in the image above, with a yellow arrow, a list of names, and a check mark. 

Note that you can change the names of the 10 selectable autonomous modes by editing the Program.cs fle in the project. See the highlighted text below:
![Chaging names](Images/Changing_Auton_Names.png)
You can have up to 10 unique modes. 

# Usage
Using the autonomous selector is simple. Just press the button onboard the HERO to move the yellow selector arrow down the list. The HERO will send a number to the RoboRIO over CAN (0-9) representing the selected mode; 0 is at the top o the list, and 9 is at the bottom. This range will be reduced if fewer than 10 modes are used. The RoboRIO must be programmed to send the same number back to the HERO, which will update the green check mark. 
![Correct](Images/correct.jpg)

This allows the user to see both what the Hero is sending the RoboRIO, and what the RoboRIO sends back as an acknowledgement. If the check mark and arrow point to different autonomous modes, the RoboRIO probably does not have the correct autonomous selected and the code should be checked.
![Incorrect](Images/incorrect.jpg)


The HERO sends a single byte of data representing the selected mode every 100ms. It sends using the Arbitration ID 0x1E040000. THe RoboRIO should send the same number back in a single byte on the Arbitration ID 0x1E040001. Note that this is **not** the same as a Talon ID.

