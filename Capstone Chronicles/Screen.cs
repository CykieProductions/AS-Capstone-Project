using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    internal class Screen
    {
        GUIComponent[] elements;
        /// <summary> The menu currently recieving input </summary>
        public static Menu? ActiveMenu { get; set; }

        public Screen(params GUIComponent[] inElements)
        {
            elements = inElements;

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].SetParentScreen(this);
            }
        }

        public void Refresh()
        {
            Console.Clear();
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Display();
            }
        }
    }
}
