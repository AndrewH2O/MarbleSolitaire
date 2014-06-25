﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbleSolitaire
{
    public class SolverTestData
    {

        public List<MaskData> MaskData = new List<MaskData>()
        {
            /*
            //blank
            new MaskData(){
                GameIndex=0,//0-32
                Direction=0,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            }
             */
            new MaskData(){
                GameIndex=8,//0-32
                Direction=0,//nswe 0123
                MaskPrePost = 
                "  100  ,  000  ,  100  ," + //- (0-2)
                "  100  ,  100  ,  000  ," + //- (3-5)
                "0010000,0010000,0000000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },   
            new MaskData(){
                GameIndex=8,//0-32
                Direction=1,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0010000,0010000,0000000," + //- (6-12)
                "0010000,0010000,0000000," + //- (13-19)
                "0010000,0000000,0010000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },  
            new MaskData(){
                GameIndex=8,//0-32
                Direction=2,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "1110000,0110000,1000000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },   
            new MaskData(){
                GameIndex=8,//0-32
                Direction=3,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0011100,0011000,0000100," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },

            new MaskData(){
                GameIndex=32,//0-32
                Direction=0,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000100,0000000,0000100," + //- (20-26)
                "  001  ,  001  ,  000  ," + //- (27-29)
                "  001  ,  001  ,  000  ,"   //- (30-32)
            },
            new MaskData(){
                GameIndex=32,//0-32
                Direction=2,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  111  ,  011  ,  100  ,"   //- (30-32)
            },

            new MaskData(){
                GameIndex=17,//0-32
                Direction=0,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  001  ,  000  ,  001  ," + //- (3-5)
                "0000100,0000100,0000000," + //- (6-12)
                "0000100,0000100,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },   
            new MaskData(){
                GameIndex=17,//0-32
                Direction=1,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000100,0000100,0000000," + //- (13-19)
                "0000100,0000100,0000000," + //- (20-26)
                "  001  ,  000  ,  001  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },  
            new MaskData(){
                GameIndex=17,//0-32
                Direction=2,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0011100,0001100,0010000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },   
            new MaskData(){
                GameIndex=17,//0-32
                Direction=3,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000111,0000110,0000001," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },
            new MaskData(){
                GameIndex=1,//0-32
                Direction=1,//nswe 0123
                MaskPrePost = 
                "  010  ,  010  ,  000  ," + //- (0-2)
                "  010  ,  010  ,  000  ," + //- (3-5)
                "0001000,0000000,0001000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },
            new MaskData(){
                GameIndex=19,//0-32
                Direction=2,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000111,0000011,0000100," + //- (13-19)
                "0000000,0000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },
            new MaskData(){
                GameIndex=20,//0-32
                Direction=0,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "1000000,0000000,1000000," + //- (6-12)
                "1000000,1000000,0000000," + //- (13-19)
                "1000000,1000000,0000000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },
            new MaskData(){
                GameIndex=20,//0-32
                Direction=3,//nswe 0123
                MaskPrePost = 
                "  000  ,  000  ,  000  ," + //- (0-2)
                "  000  ,  000  ,  000  ," + //- (3-5)
                "0000000,0000000,0000000," + //- (6-12)
                "0000000,0000000,0000000," + //- (13-19)
                "1110000,1100000,0010000," + //- (20-26)
                "  000  ,  000  ,  000  ," + //- (27-29)
                "  000  ,  000  ,  000  ,"   //- (30-32)
            },


        };
        
        
        List<int> getStart()
        {
            return new List<int>() 
            { 
                 1,1,1,0,0,//6     - (0-2)
                0,0,1,1,1,0,0,//13 - (3-5)
                1,1,1,1,1,1,1,//20 - (6-12)
                1,1,1,0,1,1,1,//27 - (13-19)
                1,1,1,1,1,1,1,//34 - (20-26)
                0,0,1,1,1,0,0,//41 - (27-29)
                0,0,1,1,1,0,0//48 -  (30-32)
            };
        }
 
    }

    
    public class MaskData
    {
        
        public ushort GameIndex { get; set; }
        public byte Direction { get; set; }
        public string Mask { get; set; }
        public string Pre { get; set; }
        public string Post { get; set; }
        public string MaskPrePost { get; set; }
        public void SplitMaskPrePost()
        {
            StringBuilder mask = new StringBuilder();
            StringBuilder pre = new StringBuilder();
            StringBuilder post = new StringBuilder();
            int offset;
            int linelength = 3 * 8;
            int lineSubsection = 8;
            for (int i = 0; i < MaskPrePost.Length; i++)
            {
                offset = ((i % linelength) / lineSubsection);
                //map
                if (offset == 0) mask.Append(MaskPrePost[i]);
                if (offset == 1) pre.Append(MaskPrePost[i]);
                if (offset == 2) post.Append(MaskPrePost[i]);
            }

            Mask = mask.ToString();
            Pre = pre.ToString();
            Post = post.ToString();
        }

    }
    
}