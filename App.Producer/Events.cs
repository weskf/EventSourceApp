using System;
using System.Collections.Generic;

namespace App.Producer
{
    public class Events
    {
        public event EventHandler<KeyPressEventArgs> OnKeyClick;

        public void OnPress(ConsoleKey key)
        {
            List<Exception> erros = new List<Exception>();
            foreach(var manipulador in OnKeyClick.GetInvocationList())
            {
                try
                {
                    manipulador.DynamicInvoke(this, new KeyPressEventArgs(key));
                }
                catch(Exception e)
                {
                    erros.Add(e.InnerException);
                }
            }
        }
    }

    public class KeyPressEventArgs : EventArgs
    {
        public KeyPressEventArgs(ConsoleKey key)
        {
            Key = key;
        }

        public ConsoleKey Key { get; }
    }
}
