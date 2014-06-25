
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleSolCommonLib.Common
{
    public class Node4
    {
        public readonly static int EMPTY_LINK = -1;
        public readonly static int NUMBER_LINK = 4;

        public int[] Targets { get; set; }
        public int[] Jumped { get; set; }
        public int[] Source { get; set; }

        public int Content { get; set; }

        public Node4()
        { 
            this.Content = Node4.EMPTY_LINK;
            //NSWE direction
            Targets = initLx();
            Jumped = initLx();
            Source = initLx();
        }

        private int[] initLx()
        {
           int[] lx = new int[Node4.NUMBER_LINK];
           for (int i = 0; i < Node4.NUMBER_LINK; i++)
           {
               lx[i] = EMPTY_LINK;
           }
           return lx;
        }

        

        
    }
}
