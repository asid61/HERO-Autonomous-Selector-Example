using System;
using Microsoft.SPOT;

namespace Hero_Autonomous_Selector_Example
{
    public class Routines {
        private static CTRE.GamepadValues _gamepadValues = new CTRE.GamepadValues();

        public static int AppendToByteArray(float value, byte[] dest, int destIndex)
        {
            int raw = (int)(value * 1000 + 0.5f); // save integral 

            byte[] src = BitConverter.GetBytes(raw);
            System.Array.Copy(src, 0, dest, destIndex, src.Length);
            return src.Length;
        }
        public static int AppendToByteArray(ulong value, byte[] dest, int destIndex)
        {
            byte[] src = BitConverter.GetBytes(value);
            System.Array.Copy(src, 0, dest, destIndex, src.Length);
            return src.Length;
        }
        public static int AppendToByteArray(int value, byte[] dest, int destIndex)
        {
            byte[] src = BitConverter.GetBytes(value);
            System.Array.Copy(src, 0, dest, destIndex, src.Length);
            return src.Length;
        }
        public static int PopFromByteArray(out float value, byte[] source, int sourceIndex)
        {
            int raw = BitConverter.ToInt32(source, sourceIndex);
            value = 0.001f * raw;
            return 4;
        }
        public static int PopFromByteArray(out ulong value, byte[] source, int sourceIndex)
        {
            value = BitConverter.ToUInt64(source, sourceIndex);
            return 8;
        }
        public static int PopFromByteArray(out int value, byte[] source, int sourceIndex)
        {
            value = BitConverter.ToInt32(source, sourceIndex);
            return 4;
        }
        /** get all buttons from a gamepad */
        public static void FillBtns(CTRE.Gamepad gamepad, ref bool[] btns, ref int pov)
        {
            gamepad.GetAllValues(ref _gamepadValues);

            pov = _gamepadValues.pov;
            for (uint i = 1; i < btns.Length; ++i)
                btns[i] = gamepad.GetButton(i);
        }
        /** @param f floating point value to string-ize. 
         * @return 0.0 format string */
        public static String FloatToString(float f)
        {
            /* detect negative values */
            int bNeg = 0;
            if(f < 0) {
                f = -f; /* make it positive */
                bNeg = 1;
            }
            /* round to tenths place by adding half of place, then truncate below*/
            f += 0.05f; 
            /* get the whole part */
            int whole = (int)(f);
            /* get the fraction part, tenths place is fine */
            f -= whole;
            f *= 10f;
            int frac = (int)f;
            /* build the string */
            if(bNeg ==0)
                return whole + "." + frac;
            return "-" + whole + "." + frac;
        }

        public static byte CheckSum(byte [] array, int len)
        {
            byte sum = 0;

            for (int i = 0; i < len; ++i)
                sum += array[i];

            return (byte)(~sum + 1);
        }

        public static void WriteSettings(byte[] source, int sourceLen)
        {
            BlockUntilCommIsGood();/* make sure eeprom works */

            int frameLen = 4 + sourceLen;
            byte[] frame = new byte[frameLen];
            frame[0] = 0xAA;
            frame[1] = 0xAb;
            frame[2] = (byte)sourceLen; // payloadLen is the number of settings bytes to save = frameLen - 4
            frame[3] = 0; // checksum must be cleared to be calced correctly.
            System.Array.Copy(source, 0, frame, 4, sourceLen);

            frame[3] = CheckSum(frame, frameLen); // calc checksum

            byte zero = CheckSum(frame, frameLen);

            OnboardEEPROM.Instance.WriteBytes(0x00, frame, frameLen);
            
            BlockUntilCommIsGood();/* make sure eeprom works */
        }
        private static void BlockUntilCommIsGood()
        {
            int goodCnt = 0;

            while (true) {
                uint jedec = OnboardEEPROM.Instance.ReadJedecID();
                if (jedec == 0xBF2541) {
                    ++goodCnt;
                } else {
                    goodCnt = 0;
                }

                if (goodCnt > 0) {
                    break;
                }

                System.Threading.Thread.Sleep(100);
            }
        }
        /**
         * Read payload bytes saved using WriteSettings.
         * @param dest byte array to fill.
         * @param cap capacity of dest.
         * @return negative number for error code, number of payload bytes read if positive.
         */
        public static int ReadSettings(byte [] dest, int cap)
        {
            BlockUntilCommIsGood();/* make sure eeprom works */

            /* attempt to deserialize */
            byte[] frame = new byte[100];
            OnboardEEPROM.Instance.ReadBytes(0x0, frame, frame.Length);

            byte hdr1 = frame[0];
            byte hdr2 = frame[1];
            byte payloadLen = frame[2];
            byte chkSum = frame[3];
            int payloadCapacity = frame.Length - 4; // how many settings bytes we can fit in temp

            if (hdr1 != 0xAA)
                return -2;
            if (hdr2 != 0xAb)
                return -3;
            if (payloadLen > payloadCapacity)
                return -4;

            byte zero = CheckSum(frame, 4 + payloadLen);
            if (zero != 0)
                return -5;

            if (payloadLen > cap)
                return -6;
            System.Array.Copy(frame, 4, dest, 0, payloadLen);
            return payloadLen;
        }
    }
}
