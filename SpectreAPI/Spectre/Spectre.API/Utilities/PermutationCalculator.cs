using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectre.API.Utilities
{
    public class PermutationCalculator
    {
        public static List<string> Result;
        public PermutationCalculator()
        {
            Result = new List<string>();

        }


       public  IList<IList<string>> Permute(string[] nums)
        {
            var list = new List<IList<string>>();
            return DoPermute(nums, 0, nums.Length - 1, list);
        }

        static IList<IList<string>> DoPermute(string[] nums, int start, int end, IList<IList<string>> list)
        {
            if (start == end)
            {
                // We have one of our possible n! solutions,
                // add it to the list.
                list.Add(new List<string>(nums));
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    Swap(ref nums[start], ref nums[i]);
                    DoPermute(nums, start + 1, end, list);
                    Swap(ref nums[start], ref nums[i]);
                }
            }

            return list;
        }

        static void Swap(ref string a, ref string b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

       public static void PrintResult(IList<IList<string>> lists)
        {
            Console.WriteLine("[");
            foreach (var list in lists)
            {
                Console.WriteLine($"    [{string.Join(',', list)}]");
            }
            Console.WriteLine("]");
            
        }

    }
}
