using System;
using System.IO;

namespace ManualSerializator
{
    class ClassMain
    {
        const int seed = 42;
        public static void Main(string[] args)
        {
            ClassRandom lst = Generate(5);

            using (FileStream fs = new FileStream("Data.dat", FileMode.OpenOrCreate))
            {
                lst.Serialize(fs);
            }

            using (FileStream fs = new FileStream("Data.dat", FileMode.Open))
            {
                lst.Deserialize(fs);
            }
        }

        private static ClassRandom Generate(int count)
        {
            Random rnd = new Random(seed);
            ClassRandom cr = new ClassRandom
            {
                Head = new ListNode
                {
                    Data = rnd.Next().ToString()
                },
                Count = count
            };

            ListNode prev = cr.Head;
            ListNode[] indexator = new ListNode[cr.Count];

            indexator[0] = prev;
            for (int i = 1; i < cr.Count; i++)
            {
                ListNode current = new ListNode
                {
                    Data = rnd.Next().ToString(),
                    Previous = prev
                };
                prev.Next = current;
                prev = current;

                indexator[i] = prev;
            }
            cr.Tail = prev;

            for (int i = 0; i < cr.Count; i++)
            {
                indexator[i].Random = indexator[rnd.Next(0, cr.Count)];
            }

            return cr;
        }
    }
}
