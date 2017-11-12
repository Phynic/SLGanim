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
        public int value;
        public int valueMax;
        public int bonus;

        public Attribute()
        {

        }

        public Attribute(string eName, string cName, int value, int valueMax, int bonus)
        {
            this.eName = eName;
            this.cName = cName;
            this.value = value;
            this.valueMax = valueMax;
            this.bonus = bonus;
        }
    }

    [System.Serializable]
    public enum Material
    {
        none,
        aluminum,   //铝
        steel,      //钢石
    }

}

