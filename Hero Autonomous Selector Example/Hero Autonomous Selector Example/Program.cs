using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.HERO.Module;


namespace Hero_Autonomous_Selector_Example
{
    public class Program
    {
        static string[] autonList = { "Autonomous 0", "Autonomous 1", "Autonomous 2", "Autonomous 3", "Autonomous 4",
            "Autonomous 5", "Autonomous 6", "Autonomous 7", "Autonomous 8", "Autonomous 9" }; //up to 15 characters per auton mode, change this to change headings! Can be up to 10 strings long.

        static Font SMALLFONT = Properties.Resources.GetFont(Properties.Resources.FontResources.small); //defines the font to be used
        static DisplayModule.OrientationType PORTRAIT = DisplayModule.OrientationType.Portrait; //sets the orientation of the display

        static DisplayModule.Color WHITE = DisplayModule.Color.White; //defines white

        static InputPort HeroButton = new InputPort((Cpu.Pin)0x42, false, Port.ResistorMode.Disabled); //Hero button port
        static DisplayModule displayModule = new DisplayModule(CTRE.HERO.IO.Port8, PORTRAIT); //starts the LCD
        static DisplayModule.ResourceImageSprite check; //yellow arrow to RIO
        static DisplayModule.ResourceImageSprite retcheck; //green check from RIO

        static uint arbid = 0x1E040000; //CAN id of the auton selector Hero, big-endian
        static uint RIOid = 0x1E040001; //CAN id of the RIO sending messages, big-endian

        private static void sendAuton(uint data, uint len, uint freq) //sends data to RoboRIO
        {
            CTRE.Native.CAN.Send(arbid, data, len, freq); //send the the first "len" bytes of data to CAN device 0x1E04xxxx every 100ms
        }

        private static ulong loadAuton(uint len) //gets data from RoboRIO
        {
            ulong ret = 0;
            CTRE.Native.CAN.Receive(RIOid, ref ret, ref len); //receive the the first "len" bytes of data from CAN device 0x1E04xxxx
            return ret; //return data from RoboRIO
        }

        private static void disp(DisplayModule.LabelSprite[] text, uint selectedAuton, uint lastSelect, uint rioAuton) //updates the display according to selected and read autons
        {
            check.SetPosition(5, 29 + 13 * (int)selectedAuton); //sets position of the yellow arrow to correct auton
            retcheck.SetPosition(110, 29 + 13 * (int)rioAuton); //sets position of the green check to correct auton
        }


        /* main functions */
        private static void runForever() //runs forever
        {
            uint selectedAuton = 0; //inialize the selected auton to 0
            uint lastSelect = 0; //last selected auton
            bool pressed = false; //button debounce stuff
            bool lastPress = false; //button debounce stuff

            DisplayModule.LabelSprite header = displayModule.AddLabelSprite(SMALLFONT, WHITE, 5, 5, 118, 16); //Makes text object for headers
            header.SetText("Selected              Actual"); //spaces for spacing
            DisplayModule.LabelSprite[] list = new DisplayModule.LabelSprite[autonList.Length]; //array of text objects
            for (int i = 0; i < autonList.Length; i++)
            {
                list[i] = displayModule.AddLabelSprite(SMALLFONT, WHITE, 18, 26 + 13 * i, 91, 13); //Makes text object for list of autons
                list[i].SetText(autonList[i]); //sets text to the string on top
            }

            check = displayModule.AddResourceImageSprite(Properties.Resources.ResourceManager,
                Properties.Resources.BinaryResources._checked,
                Bitmap.BitmapImageType.Jpeg, 5, 29 + 13 * (int)selectedAuton); //generates a cursor arrow
            retcheck = displayModule.AddResourceImageSprite(Properties.Resources.ResourceManager,
                Properties.Resources.BinaryResources.green_check,
                Bitmap.BitmapImageType.Jpeg, 110, 29 + 13 * (int)selectedAuton); //generates a green check from RIO

            while (true) //loop forever
            {
                pressed = HeroButton.Read(); //checks for button press
                if (pressed != lastPress && pressed == true)
                {
                    selectedAuton++; //move to next auton
                    selectedAuton = selectedAuton % (uint)list.Length; //wraps the cursor around
                }
                lastPress = pressed; //debounce

                sendAuton(selectedAuton, 1, 100); //send 1 byte every 100ms, 0-255 auton mode
                uint rioAuton = (uint)loadAuton(1); //get 1 byte for RIO auton return

                disp(list, selectedAuton, lastSelect, rioAuton);
                lastSelect = selectedAuton;
            }
        }
        public static void Main() //main function
        {
            runForever(); //runs forever
        }
    }
}