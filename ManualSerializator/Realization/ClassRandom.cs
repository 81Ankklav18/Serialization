using System.Collections.Generic;
using System.IO;

namespace ManualSerializator
{
    class ClassRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        // Структура, хранящая ссылки как целые числа для дальнейшей сериализации/десериализации
        struct SerializedListNode
        {
            public int randomNodeId; //Числовое представления ссылки на случайный элемент
            public string data; //Поле соответствует полю Data класса ListNode
            public ListNode serialized; //Ссылка на сериализуемый объект

            public SerializedListNode(int randomNodeId, string data, ListNode serialized)
            {
                this.randomNodeId = randomNodeId;
                this.data = data;
                this.serialized = serialized;
            }
        }

        public void Serialize(Stream s)
        {
            SerializedListNode[] serializedData = new SerializedListNode[Count];
            Dictionary<ListNode, int> indexator = new Dictionary<ListNode, int>(); //Необходимо для преобразования ссылки в число

            ListNode temp = Head;
            for (int i = 0; i < Count; i++)
            {
                indexator.Add(temp, i);
                serializedData[i] = new SerializedListNode(-1, temp.Data, temp); //-1 == null
                temp = temp.Next;
            }

            //Заменяем ссылки на номера
            for (int i = 0; i < Count; i++)
            {
                serializedData[i].randomNodeId = indexator[serializedData[i].serialized.Random];
            }

            using (BinaryWriter bw = new BinaryWriter(s))
            {
                bw.Write(Count);

                for (int i = 0; i < Count; i++)
                {
                    bw.Write(serializedData[i].randomNodeId);
                    bw.Write(serializedData[i].data);
                }
            }
        }

        public void Deserialize(Stream s)
        {
            SerializedListNode[] serializedData;

            using (BinaryReader br = new BinaryReader(s))
            {
                Count = br.ReadInt32();

                serializedData = new SerializedListNode[Count];
                for (int i = 0; i < Count; i++)
                {
                    serializedData[i].randomNodeId = br.ReadInt32();
                    serializedData[i].data = br.ReadString();
                }
            }

            Head = new ListNode
            {
                Data = serializedData[0].data
            };

            ListNode prev = Head;
            ListNode[] indexator = new ListNode[Count];

            //Восстанавливаем ссылки полей Next, Previous
            indexator[0] = prev;
            for (int i = 1; i < Count; i++)
            {
                ListNode current = new ListNode
                {
                    Data = serializedData[i].data,
                    Previous = prev
                };
                prev.Next = current;
                prev = current;

                indexator[i] = prev;
            }
            Tail = prev;

            //Восстанавливаем ссылки на поле Random
            for (int i = 0; i < Count; i++)
            {
                indexator[i].Random = indexator[serializedData[i].randomNodeId];
            }
        }

    }
}
