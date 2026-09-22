using System.Collections.Generic;
using UnityEngine;

namespace MidtermExam.Prob02
{
    public class LinkedListSorter
    {
        /// <summary>
        /// เรียงลำดับตัวเลขใน LinkedList จากน้อยไปมาก (Ascending Order)
        /// </summary>
        /// <param name="list">LinkedList ของตัวเลข integer</param>
        /// <returns>LinkedList ที่ได้รับการเรียงลำดับจากน้อยไปมากแล้ว</returns>
        public LinkedList<int> SortAscending(LinkedList<int> list)
        {
            if (list == null)
            {
                return null;
            }

            if (list.First == null || list.First.Next == null)
            {
                return list;
            }

            bool swapped;
            do
            {
                swapped = false;
                var node = list.First;
                while (node != null && node.Next != null)
                {
                    if (node.Value > node.Next.Value)
                    {
                        int tmp = node.Value;
                        node.Value = node.Next.Value;
                        node.Next.Value = tmp;
                        swapped = true;
                    }

                    node = node.Next;
                }
            } while (swapped);

            return list;
        }

        /// <summary>
        /// เรียงลำดับตัวเลขใน LinkedList จากมากไปน้อย (Descending Order)
        /// </summary>
        /// <param name="list">LinkedList ของตัวเลข integer</param>
        /// <returns>LinkedList ที่ได้รับการเรียงลำดับจากมากไปน้อยแล้ว</returns>
        public LinkedList<int> SortDescending(LinkedList<int> list)
        {
            if (list == null)
            {
                return null;
            }

            if (list.First == null || list.First.Next == null)
            {
                return list;
            }

            bool swapped;
            do
            {
                swapped = false;
                var node = list.First;
                while (node != null && node.Next != null)
                {
                    if (node.Value < node.Next.Value)
                    {
                        int tmp = node.Value;
                        node.Value = node.Next.Value;
                        node.Next.Value = tmp;
                        swapped = true;
                    }

                    node = node.Next;
                }
            } while (swapped);

            return list;
        }
    }
}
