using System;
using System.Collections.Generic;

namespace Chessharp.Core
{
    public class Value
    {
        int type;
        object value;

        public Value(int val)
        {
            this.type = 1;
            this.value = val;
        }

        public Value(string val)
        {
            this.type = 2;
            this.value = val;
        }

        public Value(bool val)
        {
            this.type = 3;
            this.value = val;
        }

        public Value(float val)
        {
            this.type = 4;
            this.value = val;
        }

        public Value(double val)
        {
            this.type = 5;
            this.value = val;
        }

        object Get()
        {
            return this.value;
        }

        public string getType()
        {
            string typeName = "";
            switch (this.type) {
                case 1:
                    typeName = "string";
                    break;
                case 2:
                    typeName = "integer";
                    break;
                case 3:
                    typeName = "boolean";
                    break;
                case 4:
                    typeName = "float";
                    break;
                case 5:
                    typeName = "double";
                    break;
                default:
                    typeName = "undefined";
                    break;

            }
            return typeName;
        }
    }
}
