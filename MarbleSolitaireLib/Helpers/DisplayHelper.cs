using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleSolitaireLib.Helpers
{
    public class DisplayHelper:IDisposable
    {
        StringBuilder _contents;

        public StringBuilder Contents
        {
            get { return _contents; }
        }
        
        public DisplayHelper()
        {
            _contents = new StringBuilder();
        }

        public void Add(string s, object arg0)
        {
            if (_contents == null) return;
            _contents.AppendLine(string.Format(s,arg0));
        }

        public void Add(string s)
        {
            _contents.AppendLine(string.Format(s));
        }

        public void Add(object o)
        {
            if (_contents == null) return;
            _contents.AppendLine(string.Format(o.ToString()));
        }

        public void Add(StringBuilder sb)
        {
            _contents.Append(sb.ToString());
        }

        public void Dispose()
        {
            if (_contents != null) _contents = null;
        }



        public void Append(string s)
        {
            _contents.Append(string.Format(s));
        }

        public void Append(object o)
        {
            if (_contents == null) return;
            _contents.Append(string.Format(o.ToString()));
        }



        public void LineBreak()
        {
            _contents.AppendLine();
        }
    }
}
