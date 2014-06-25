using MarbleSolitaireLib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.Helpers
{
    ///TODO error trapping strategy/ exception handling
    public class SolverIO<T> 
    {

        static string AppendPathTo(string fileName)
        {
            string result = SolverIO<int>.GetBaseDirectory() + fileName;
            return result;
        }

        public static string GetBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar;
        }

        public static void SaveBinary(T o)
        {
            
            string fileName = "EnumDto.dat";
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = File.Open(AppendPathTo(fileName), FileMode.Create))
            {
                formatter.Serialize(stream, o);
            }
            
        }

        public static void SaveBinary(T o,string fileName)
        {
            
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = File.Open(AppendPathTo(fileName), FileMode.Create))
            {
                formatter.Serialize(stream, o);
            }

        }

        public static T RetrieveBinary()
        {
            string fileName = "EnumDto.dat";
            IFormatter formatter = new BinaryFormatter();
            T result;
            using (Stream stream = File.Open(AppendPathTo(fileName), FileMode.Open))
            {
                result = (T)formatter.Deserialize(stream);
            }
            return result;
        }

        public static T RetrieveBinary(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            T result;
            using (Stream stream = File.Open(AppendPathTo(fileName), FileMode.Open))
            {
                result = (T)formatter.Deserialize(stream);
            }
            return result;
        }

        public static void SaveText(Object o)
        {
            string fileName = "snapShots.txt";
            using (FileStream fs = new FileStream(AppendPathTo(fileName), FileMode.Create))
            {
                StreamWriter w = new StreamWriter(fs);
                w.Write(o.ToString());
                w.Flush();
            }
            o = null;
        }

        public static void SaveText(Object o, string fileName)
        {

            using (FileStream fs = new FileStream(AppendPathTo(fileName), FileMode.Create))
            {
                StreamWriter w = new StreamWriter(fs);
                w.Write(o.ToString());
                w.Flush();
            }
            o = null;
        }
    }


}
