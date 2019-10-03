using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
namespace SLG
{
    [System.Serializable]
    public class Attribute
    {
        public string eName;
        public string cName;

        public int ID { get; private set; }
        public int Value { get; private set; }
        public int ValueMax { get; private set; }
        public Attribute(int ID)
        {
            this.ID = ID;
            var info = AttributeInfoDictionary.GetParam(ID);
            eName = info.eName;
            cName = info.cName;
            Value = 0;
            ValueMax = info.valueMax;
        }

        public void ChangeValueTo(int value)
        {
            //值大于零小于最大值，用于抹除值大于最大值的部分。
            Value = Mathf.Clamp(value, 0, ValueMax);
        }

        public void ChangeValueMaxTo(int valueMax)
        {
            ValueMax = valueMax;
            //最大值大于等于零
            ValueMax = Mathf.Clamp(valueMax, 0, ValueMax);
            //值大于零小于最大值，用于抹除值大于最大值的部分。
            Value = Mathf.Clamp(Value, 0, ValueMax);
        }

        public void PlusValue(int value)
        {
            Value += value;
            //值大于零小于最大值，用于抹除值大于最大值的部分。
            Value = Mathf.Clamp(Value, 0, ValueMax);
        }

        public void PlusValueMax(int valueMax)
        {
            ValueMax += valueMax;
            //最大值大于等于零
            ValueMax = Mathf.Clamp(ValueMax, 0, ValueMax);
            //值大于零小于最大值，用于抹除值大于最大值的部分。
            Value = Mathf.Clamp(Value, 0, ValueMax);
        }
    }
}

