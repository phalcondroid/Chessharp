using System;
using System.Collections.Generic;

namespace Chessharp.Core.Structures
{
    public class CValue
    {
        object value;

        public CValue()
        {
            value = new object();
        }

        public CValue(object val)
        {
            SetValue(val);
        }

        public object GetValue()
        {
            return value;
        }

        public void SetValue(object val)
        {
            value = val;
        }

        public byte GetByte()
        {
            return (byte) value;
        }

        public sbyte GetSbyte()
        {
            return (sbyte) value;
        }

        public int GetInt()
        {
            return Convert.ToInt32(value);
        }

        public long GetLong()
        {
            return (long)value;
        }

        public double GetDouble()
        {
            return (double)value;
        }

        public string GetString()
        {
            return value.ToString();
        }

        public Boolean GetBool()
        {
            return (bool)value;
        }

        public Dictionary<string, string> GetDicStrStr()
        {
            return (Dictionary<string, string>)value;
        }

        public Dictionary<int, string> GetDicIntStr()
        {
            return (Dictionary<int, string>)value;
        }

        public Dictionary<string, int> GetDicStrInt()
        {
            return (Dictionary<string, int>)value;
        }

        public Dictionary<int, int> GetDicIntInt()
        {
            return (Dictionary<int, int>)value;
        }

        public Dictionary<string, bool> GetDicStrBool()
        {
            return (Dictionary<string, bool>)value;
        }

        public Dictionary<int, bool> GetDicIntBool()
        {
            return (Dictionary<int, bool>)value;
        }

        public string Type
        {
            get
            {
                Type t = value.GetType();
                if (t.Equals(typeof(byte)))
                {
                    return "byte";
                }
                else if (t.Equals(typeof(sbyte)))
                {
                    return "sbyte";
                }
                else if (t.Equals(typeof(int)))
                {
                    return "integer";
                }
                else if (t.Equals(typeof(long)))
                {
                    return "long";
                }
                else if (t.Equals(typeof(double)))
                {
                    return "double";
                }
                else if (t.Equals(typeof(string)))
                {
                    return "string";
                }
                else if (t.Equals(typeof(bool)))
                {
                    return "boolean";
                }
                else if (t.Equals(typeof(Dictionary<string, string>)))
                {
                    return "Dictionary<string, string>";
                }
                else if (t.Equals(typeof(Dictionary<int, string>)))
                {
                    return "Dictionary<int, string>";
                }
                else if (t.Equals(typeof(Dictionary<int, int>)))
                {
                    return "Dictionary<int, int>";
                }
                else if (t.Equals(typeof(Dictionary<string, bool>)))
                {
                    return "Dictionary<string, bool>";
                }
                else
                {
                    return "none";
                }
            }
        }
    }
}
