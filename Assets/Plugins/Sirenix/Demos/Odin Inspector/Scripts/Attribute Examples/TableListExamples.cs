namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;
    using System.Collections.Generic;

    public class TableListExamples : SerializedMonoBehaviour
    {
        [TableList]
        public List<A> TableList = new List<A>();
    }

    public class A
    {
        [TableColumnWidth(50)]
        public bool Toggle;

        public string Message;

        [TableColumnWidth(160)]
        [HorizontalGroup("Actions")]
        public void Test1() { }

        [HorizontalGroup("Actions")]
        public void Test2() { }
    }

    public class B : A
    {
        [HorizontalGroup("Actions")]
        public void Test3() { }
    }

    public class C : A
    {
        [HorizontalGroup("Actions")]
        public void Test4() { }
    }
}