using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bin2homebrew
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            Console.Write("Process begins.\n");

            foreach (string arg in args)
            {
                byte[] argData = File.ReadAllBytes(arg);
                string
                    homebrew = "function writeHomebrewEN(p, addr) {\n"
                    , argText = BitConverter.ToString(argData).Replace("-", "");

                int
                    counter = 0
                    , argBytes = argText.Length / 8
                    , argRemainingBits = argText.Length % 8;

                List<string> argDataTextList =
                        Enumerable.Range(0, argBytes)
                        .Select(i => BitConverter.ToString(StringToByteArrayReversed(argText.Substring(8 * i, 8))).Replace("-", "").ToLowerInvariant()).ToList();

                Console.Write("Converting {0} to homebrew.js.\n", Path.GetFileName(arg));

                //if (Convert.ToBoolean(argRemainingBits))
                    //argDataTextList.Add(int.Parse(BitConverter.ToString(StringToByteArrayReversed(argText.Substring(argText.Length - argRemainingBits))).Replace("-", ""), System.Globalization.NumberStyles.HexNumber).ToString("X8"));

                foreach (string argDataTextListCell in argDataTextList)
                    homebrew += "\tp.write4(addr.add32(0x" + (counter++ * 4).ToString("X8").ToLowerInvariant() + "), 0x" + argDataTextListCell + ");\n";

                homebrew += "}";

                string homebrewFile =
                    Path.GetDirectoryName(arg)
                    + Path.DirectorySeparatorChar
                    + "homebrew.js";

                File.WriteAllText(homebrewFile, homebrew);

                Console.Write("Successfully converted.\n");
            }

            Console.Write("Process completed.\nPress any key to close this window...");

            Console.ReadKey();
        }
        public static byte[] StringToByteArrayReversed(string hex)
        {
            byte[] hexRev =
                Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

            Array.Reverse(hexRev);
            return hexRev;
        }
    }
}
