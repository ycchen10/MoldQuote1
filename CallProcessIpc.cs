using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoldQuote
{
    public class CallProcessIpc
    {
        //定义DLL文件名，此文件路径要加到系统Path中
        private const string _fileDll = @"CallProcessIpc.dll";
        //调用非托管Dll，NxToErpQuote是CallProcessIpc.dll公开的函数名称
        [DllImport(_fileDll, EntryPoint = "NxToErpQuote", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //C#中的申明
        public static extern Boolean NxToErpQuote(string MessageData );
    }
}
