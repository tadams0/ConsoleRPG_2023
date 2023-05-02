using ConsoleRPG_2023.RolePlayingGame.Items;
using CustomConsoleColors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023.RolePlayingGame.Renderers
{
    /// <summary>
    /// Defines a class that renders a grid of objects.
    /// </summary>
    public class GridRenderer
    {
        /// <summary>
        /// The number of columns to render.
        /// </summary>
        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }


        public int MinIndex
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the number of items rendered in the grid based on the last rendering's filter and other settings.
        /// </summary>
        public int LastRenderedItemCount
        {
            get { return lastRenderedItemCount; }
        }

        private int columns = 5;
        private int marginBetweenColumns = 1;

        private int lastRenderedItemCount;

        private Dictionary<object, Func<object, string>> objectMapping = new Dictionary<object, Func<object, string>>();
        private List<object> lastRenderList;

        private Func<List<object>, List<object>> filter = null;

        private int width;

        public GridRenderer(int width)
        {
            this.width = width;
        }

        public void Clear()
        {
            objectMapping.Clear();
            if (lastRenderList != null)
            {
                lastRenderList.Clear();
            }
        }

        /// <summary>
        /// Gets the row index of the given item index. Note that 0 represents the top of the grid render and not the entire console window.
        /// </summary>
        /// <param name="index">The index of the specific item.</param>
        public int GetRowOfIndex(int index)
        {
            return index / columns;
        }

        public void AddItem(object obj, Func<object, string> displayConversionFunction)
        {
            objectMapping[obj] = displayConversionFunction;
        }

        /// <summary>
        /// Adds multiple items at once all using the same display conversion function.
        /// </summary>
        public void AddItemRange(IEnumerable<object> items, Func<object, string> displayConversionFunction)
        {
            foreach (object item in items)
            {
                objectMapping[item] = displayConversionFunction;
            }
        }

        public int GetMaximumItemDisplayWidth()
        {
            int totalMarginSpace = (columns - 1) * marginBetweenColumns;
            int totalSpaceAvailable = (width - totalMarginSpace);
            int spacePerItemDisplay = totalSpaceAvailable / columns;

            return spacePerItemDisplay;
        }

        public object GetObjectAtIndex(int index)
        {
            if (index < 0 || index >= lastRenderList.Count)
            {
                return null;
            }
            return lastRenderList[index];
        }

        public void SetFilter(Func<List<object>, List<object>> filter)
        {
            this.filter = filter;
        }

        public List<object> FilterList(List<object> list)
        {
            if (filter == null)
            {
                return list;
            }

            return filter(list);
        }

        public void Render()
        {
            Render(-1);
        }

        public void Render(int selectedIndex)
        {
            int totalMarginSpace = (columns - 1) * marginBetweenColumns;
            int totalSpaceAvailable = (width - totalMarginSpace);
            int spacePerItemDisplay = totalSpaceAvailable / columns;
            int remainderSpace = totalSpaceAvailable % columns;

            //Retrieve the objects to render.
            List<object> items = objectMapping.Select(x=>x.Key).ToList();

            //Filter them according to any filter function set.
            items = FilterList(items);

            //Saving the rendered list incase future index lookups are needed.
            lastRenderList = items;

            lastRenderedItemCount = items.Count;

            string marginDisplay = new string(' ', marginBetweenColumns);

            if (columns <= 1)
            { //No margin on single column renderings.
                marginDisplay = string.Empty;
            }

            string leftRemainderPadding = new string(' ', remainderSpace / 2);
            string rightRemainderPadding = new string(' ', remainderSpace - (remainderSpace / 2));

            string currentItemDisplay;

            StringBuilder sb = new StringBuilder();
            StringBuilder lineSb = new StringBuilder(width + 1);

            string marginBackgroundColor = VirtualConsoleSequenceBuilder.GetColorBackgroundSequence(Color.Black);

            Func<object, string> displayFunc;
            string columnDisplay = string.Empty;
            string ansiStrippedDisplay;
            string rightPadding;
            object currentItem;

            for (int i = 0; i < items.Count; i++)
            {
                currentItem = items[i];
                displayFunc = objectMapping[currentItem];

                currentItemDisplay = displayFunc(currentItem);
                ansiStrippedDisplay = VirtualConsoleSequenceBuilder.StripAnsiCodes(currentItemDisplay);

                if (ansiStrippedDisplay.Length < spacePerItemDisplay)
                {//If the item does surpass the space allotted per item, then we must pad right.
                    rightPadding = new string(' ', spacePerItemDisplay - ansiStrippedDisplay.Length);

                    currentItemDisplay += rightPadding;
                }

                columnDisplay = currentItemDisplay;

                if (i == selectedIndex + 1)
                {

                    lineSb.Append(VirtualConsoleSequenceBuilder.Default);
                }

                if (i % columns == 0)
                {//The first column on a new row.
                    //Append the line to the entire string builder.
                    sb.Append(lineSb.ToString());
                    lineSb.Clear();

                    columnDisplay = rightRemainderPadding + leftRemainderPadding + columnDisplay + marginBackgroundColor + marginDisplay;
                }
                else if (i % columns != columns - 1)//If it is NOT the column just before a new row.
                {
                    columnDisplay = columnDisplay + marginBackgroundColor + marginDisplay;
                }

                if (i == selectedIndex)
                {
                    lineSb.Append(VirtualConsoleSequenceBuilder.Blinking);
                    lineSb.Append(VirtualConsoleSequenceBuilder.Invert);
                }

                lineSb.Append(columnDisplay);
            }

            //Append the final line.
            string finalLine = lineSb.ToString();
            sb.Append(finalLine);

            //Append the foreground/background color erasure.
            string removeColors = VirtualConsoleSequenceBuilder.Default;
            sb.Append(removeColors);

            //Append a string to fill in the buffer. This is only really needed if the display is not a perfect rectangle of 
            //item displays and there is a remainder of empty column space at the bottom.
            string finalLineNoAnsiCode = VirtualConsoleSequenceBuilder.StripAnsiCodes(finalLine);
            int finalPaddingLength = width - finalLineNoAnsiCode.Length;
            if (finalPaddingLength > 0)
            {
                string finalPadding = new string(' ', finalPaddingLength);
                sb.Append(finalPadding);
            }

            Console.WriteLine(sb.ToString());
        }


    }
}
