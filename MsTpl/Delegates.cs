using System;

namespace MsTpl
{
    internal class Delegates
    {

        private delegate void MyDelMethodDef();

        private Func<int> aFuncWithRet;

        private Func<int, string> aFuncWithRetAndInput;

        private Action anAction;

        private Action<int, int> anActionWithTwoInputParam;

        public void RunTests()
        {
            anAction = VoidNoInput;
            anActionWithTwoInputParam = VoidTwoInputs;
            aFuncWithRet = AMethodWithReturn;
            aFuncWithRetAndInput = FuncWithRetAndInput;
        }

        private void VoidNoInput() { }
        private void VoidTwoInputs(int a, int b) { }
        private int AMethodWithReturn() { return 0; }
        private string FuncWithRetAndInput(int a) { return ""; }

    }
}