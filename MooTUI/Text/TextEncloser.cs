using System;
using System.Collections.Generic;
using System.Text;

namespace MooTUI.Text
{
    public class TextEnclosureBuilder
    {
        private string _text;

        public TextEnclosureBuilder(string text)
        {
            _text = text;
        }

        public void Space() =>           _text = " " + _text + " ";
        public void TriangleBracket() => _text = "<" + _text + ">";
        public void CurlyBracket() =>    _text = "{" + _text + "}";
        public void SquareBracket() =>   _text = "[" + _text + "]";
        public void ForwardSlash() =>    _text = "/" + _text + "/";

        public string Build() => _text;
    }
}
