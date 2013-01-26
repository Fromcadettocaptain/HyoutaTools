﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HyoutaTools.Other.LuxPain
{
    static class Util
    {
        public static Encoding ShiftJISEncoding = Encoding.GetEncoding("shift-jis");

        public static int AlignToByteBoundary(int x, int boundary)
        {
            int diff = x % boundary;
            if (diff == 0) return x;

            diff = boundary - diff;
            return x + diff;
        }

        public static UInt32 SwapEndian(UInt32 x)
        {
            return x = (x >> 24) |
                      ((x << 8) & 0x00FF0000) |
                      ((x >> 8) & 0x0000FF00) |
                       (x << 24);
        }

        public static UInt64 SwapEndian(UInt64 x)
        {
            return   x = (x>>56) |
                        ((x<<40) & 0x00FF000000000000) |
                        ((x<<24) & 0x0000FF0000000000) |
                        ((x<<8)  & 0x000000FF00000000) |
                        ((x>>8)  & 0x00000000FF000000) |
                        ((x>>24) & 0x0000000000FF0000) |
                        ((x>>40) & 0x000000000000FF00) |
                         (x<<56);
        }



        public static byte[] StringToBytes(String s)
        {
            //byte[] bytes = ShiftJISEncoding.GetBytes(s);
            //return bytes.TakeWhile(subject => subject != 0x00).ToArray();
            return ShiftJISEncoding.GetBytes(s);
        }

        public static void DisplayException(Exception e)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(e.Message);
        }

        public static String GetText(int Pointer, byte[] File)
        {
            if (Pointer == -1) return null;

            try
            {
                int i = Pointer;
                while (File[i] != 0x00)
                {
                    i++;
                }
                String Text = Util.ShiftJISEncoding.GetString(File, Pointer, i - Pointer);
                return Text;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static String GetTextDoubleByte(int Pointer, byte[] File)
        {
            if (Pointer == -1) return null;

            try
            {
                int i = Pointer;
                while ( !(File[i] == 0x00 && File[i+1] == 0x00) )
                {
                    i += 2;
                }
                String Text = Util.ShiftJISEncoding.GetString(File, Pointer, i - Pointer);
                return Text;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static String GetTextLuxPain(int Pointer, byte[] File)
        {
            if (Pointer == -1) return null;

            try
            {
                int i = Pointer;

                StringBuilder Text = new StringBuilder();
                int CommandParametersLeft = 0;

                while ( true )
                {
                    // who wrote this ingame text parsing algo? uugh
                    if (CommandParametersLeft <= 0 && File[i] == 0x00 && File[i + 1] == 0x00)
                    { // 0000 indicates end of text, except when a command with parameters is active
                        break;
                    }

                    if (CommandParametersLeft <= 0 && File[i] == 0xFF)
                    { //0xFFxx == command, command may have arguments
                        switch (File[i + 1])
                        {
                            case 0x02: CommandParametersLeft = 1; break;
                            case 0x04: CommandParametersLeft = 1; break;
                            case 0x06: CommandParametersLeft = 1; break;
                            case 0x07: CommandParametersLeft = 2; break;
                            case 0x0a: CommandParametersLeft = 1; break;
                            case 0x0e: CommandParametersLeft = 2; break;
                            case 0x0f: CommandParametersLeft = 2; break;
                            case 0x10: CommandParametersLeft = 2; break;
                        }
                    }
                    else
                    {
                        --CommandParametersLeft;
                    }

                    String tmp;
                    if (File[i] < 0x80 || File[i] >= 0xF0)
                    {
                        tmp = string.Format("<{0:x2}{1:x2}>", File[i], File[i + 1]);
                    }
                    else
                    {
                        tmp = Util.ShiftJISEncoding.GetString(File, i, 2);
                    }
                    Text.Append(tmp);
                    i += 2;
                }
                return Text.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static String GetTextUTF8(int Pointer, byte[] File)
        {
            if (Pointer == -1) return null;

            try
            {
                int i = Pointer;
                while (File[i] != 0x00)
                {
                    i++;
                }
                String Text = Encoding.UTF8.GetString(File, Pointer, i - Pointer);
                return Text;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}