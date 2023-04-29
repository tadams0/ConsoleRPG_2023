using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleRPG_2023.RolePlayingGame
{
    /// <summary>
    /// A class which generates random things.
    /// </summary>
    public static class RandomGenerator
    {
        private static Random rand = new Random();

        private static readonly string consonants = "bcdfghjklmnpqrstvwxyz";

        private static readonly string vowels = "aeiou";

        public static void SetRandomSeed(string seed)
        {
            rand = new Random(seed.GetHashCode());
        }

        public static long RandomLong()
        {
            int first32Bits = rand.Next(int.MinValue, int.MaxValue);
            int last32Bits = rand.Next(int.MinValue, int.MaxValue);

            //Shift the bits by 32 (so 0-31 are 0 and 31-64 are the int value) and using a bit-wise OR to combine the bits.
            //The last 32 bits sits in the original 0-31 spot, so combined all bits are accounted for.
            return (long)first32Bits << 32 | (long)(uint)last32Bits;
        }

        public static string GenerateRandomFirstName(Race race, bool masculine)
        {
            float vowelEndChanceMultiplier = 1;
            if (masculine)
            {
                vowelEndChanceMultiplier = 2f;
            }
            switch (race)
            {
                default:
                case Race.Human: return GenerateGenericNameAlgorithim(race, 3, 3, 80, 20, 25, 40, (int)(45 * vowelEndChanceMultiplier));
                case Race.Dwarf: return GenerateGenericNameAlgorithim(race, 4, 2, 80, 20, 95, 80, (int)(40 * vowelEndChanceMultiplier));
                case Race.Orc: return GenerateGenericNameAlgorithim(race, 3, 1, 50, 80, 70, 10, (int)(35 * vowelEndChanceMultiplier));
                case Race.High_Elf: return GenerateGenericNameAlgorithim(race, 4, 1, 90, 20, 96, 40, (int)(15 * vowelEndChanceMultiplier));
            }
            
        }

        private static string GenerateGenericNameAlgorithim(Race race, int maxParts, int minParts, int consonantThenVowelChance, int VowelThenConsonantChance, int rerollSinglePartChance, 
            int ckChance, int consonantEndChance)
        {
            string result = string.Empty;
            int parts = rand.Next(minParts, maxParts + 1);
            if (parts == 1 && rand.Next(0,100) < rerollSinglePartChance)
            {
                parts += rand.Next(1, 3);
            }

            char lastCharacter; //reusable variable.
            string newPart;
            for (int i = 0; i < parts; i++)
            {
                if (result.Length > 0)
                {
                    lastCharacter = result.Last();
                    if (lastCharacter == 'c' && rand.Next(0, 100) < ckChance)
                    {
                        result += 'k';
                        continue;
                    }
                    else  if (consonants.Contains(lastCharacter)) //If the last letter of the current in-progress name is a consonant..
                    {
                        if (rand.Next(0, 100) < consonantThenVowelChance)
                        {//x% chance of random vowel as a part.
                            result += GetRandomVowel();
                            continue;
                        }
                    }
                    else
                    { //If the last letter was a vowel

                        if (rand.Next(0, 100) < VowelThenConsonantChance) //x% chance of a consonent
                        {//x% chance of random vowel as a part.
                            result += GetRandomConsonant();
                            continue;
                        }
                    }
                }

                newPart = GetRaceFirstNamePart(race);
                if (newPart.Length >= 5) //single parts with >= 5 characters count as 2 parts.
                {
                    //But we only subtract, so if it's the last part anyways, the name will be extra long.
                    parts--;
                }

                //Add the new part to the name.
                result += newPart;

                if (i == 0)
                {//On the first iteration, we cap the first letter of the first part.
                    result = CapFirstLetter(result);
                }
                else if (i == parts - 1) //if we're on the last iteration...
                {
                    if (result.Length <= 2 && rand.Next(0, 100) < 95)
                    {
                        parts += 1;
                    }
                    else if (result.Length <= 3 && rand.Next(0,100) < 70)
                    { //70% chance to add another part to 3 letter names.
                        parts += 1;
                    }
                }
            }

            lastCharacter = result.Last();
            bool endInConsonant = rand.Next(0, 100) < consonantEndChance;
            if (endInConsonant && !consonants.Contains(lastCharacter))
            {
                result += GetRandomConsonant();
            }
            else if (!endInConsonant && consonants.Contains(lastCharacter))
            {
                result += GetRandomVowel();
            }

            return result;
        }

        private static string GetRaceFirstNamePart(Race race)
        {

            switch (race)
            {
                default:
                case Race.Human: return GetHumanFirstNamePart();
                case Race.Dwarf: return GetDwarfFirstNamePart();
                case Race.Orc: return GetOrcFirstNamePart();
                case Race.High_Elf: return GetHighElfFirstNamePart();
            }
        }

        private static string GetHumanFirstNamePart()
        {
            List<string> parts = new List<string>();

            parts.Add("bo");
            parts.Add("tay");
            parts.Add("ta");
            parts.Add("jac");
            parts.Add("ja");
            parts.Add("za");
            parts.Add("zac");
            parts.Add("ob");
            parts.Add("oon");
            parts.Add("ori");
            parts.Add("bla");
            parts.Add("na");
            parts.Add("lo");
            parts.Add("lor");
            parts.Add("bin");
            parts.Add("be");
            parts.Add("gre");
            parts.Add("gr");
            parts.Add("kn");
            parts.Add("sa");
            parts.Add("eli");
            parts.Add("zi");
            parts.Add("beth");
            parts.Add("ala");
            parts.Add("an");
            parts.Add("dre");
            parts.Add("than");
            parts.Add("stin");
            parts.Add("stan");
            parts.Add("bas");
            parts.Add("ion");
            parts.Add("ot");

            int randomIndex = rand.Next(0, parts.Count);

            return parts[randomIndex];
        }

        private static string GetDwarfFirstNamePart()
        {
            List<string> parts = new List<string>();

            parts.Add("bo");
            parts.Add("ob");
            parts.Add("ori");
            parts.Add("na");
            parts.Add("lo");
            parts.Add("lor");
            parts.Add("bin");
            parts.Add("be");
            parts.Add("g");
            parts.Add("gr");
            parts.Add("kr");
            parts.Add("kra");
            parts.Add("kag");
            parts.Add("nac");
            parts.Add("gra");
            parts.Add("nag");
            parts.Add("loth");
            parts.Add("th");
            parts.Add("log");
            parts.Add("bar");
            parts.Add("thor");
            parts.Add("th");
            parts.Add("thar");
            parts.Add("thar");
            parts.Add("dun");
            parts.Add("dun");
            parts.Add("den");
            parts.Add("dar");
            parts.Add("mac");
            parts.Add("thor");
            parts.Add("fin");
            parts.Add("fen");
            parts.Add("bro");
            parts.Add("god");
            parts.Add("beard");

            int randomIndex = rand.Next(0, parts.Count);

            return parts[randomIndex];
        }

        private static string GetOrcFirstNamePart()
        {
            List<string> parts = new List<string>();

            parts.Add("bo");
            parts.Add("ob");
            parts.Add("og");
            parts.Add("nor");
            parts.Add("log");
            parts.Add("lorg");
            parts.Add("bag");
            parts.Add("gru");
            parts.Add("um");
            parts.Add("gr");
            parts.Add("krag");
            parts.Add("sma");
            parts.Add("gli");
            parts.Add("mag");
            parts.Add("gro");
            parts.Add("glub");
            parts.Add("buk");
            parts.Add("rag");
            parts.Add("ro");
            parts.Add("thar");
            parts.Add("uk");
            parts.Add("gruk");
            parts.Add("kog");
            parts.Add("kor");
            parts.Add("rug");
            parts.Add("gum");
            parts.Add("yob");
            parts.Add("yor");
            parts.Add("kon");
            parts.Add("blood");
            parts.Add("bro");
            parts.Add("kun");

            int randomIndex = rand.Next(0, parts.Count);

            return parts[randomIndex];
        }

        private static string GetHighElfFirstNamePart()
        {
            List<string> parts = new List<string>();

            parts.Add("el");
            parts.Add("zini");
            parts.Add("sam");
            parts.Add("ara");
            parts.Add("zay");
            parts.Add("tay");
            parts.Add("lor");
            parts.Add("loy");
            parts.Add("yen");
            parts.Add("if");
            parts.Add("fer");
            parts.Add("mala");
            parts.Add("lef");
            parts.Add("yar");
            parts.Add("yol");
            parts.Add("yol");
            parts.Add("yel");
            parts.Add("yal");
            parts.Add("yla");
            parts.Add("ylo");
            parts.Add("sar");
            parts.Add("rin");
            parts.Add("rana");
            parts.Add("pra");
            parts.Add("prus");
            parts.Add("sel");
            parts.Add("sin");
            parts.Add("yae");
            parts.Add("sei");
            parts.Add("un");
            parts.Add("uny");
            parts.Add("sun");
            parts.Add("euy");
            parts.Add("fae");
            parts.Add("fei");

            int randomIndex = rand.Next(0, parts.Count);

            return parts[randomIndex];
        }

        private static char GetRandomVowel()
        {
            return vowels[rand.Next(0, vowels.Length)];
        }
        private static char GetRandomConsonant()
        {
            return consonants[rand.Next(0, vowels.Length)];
        }

        private static string CapFirstLetter(string str)
        {
            return str.Substring(0,1).ToUpper() + str.Substring(1);
        }

        public static string GenerateRandomLastName(Race race)
        {

            return null;
        }

    }
}
