using System;
using System.Collections.Generic;

namespace ProjektLS22
{

    public class InputHandler
    {
        Dictionary<char, Action> funcs = new Dictionary<char, Action>();
        public void RegisterOption(char key, Action action)
        {
            funcs.Add(char.ToUpper(key), action);
        }
        public void ClearOptions()
        {
            funcs.Clear();
        }

        public void RegisterParametricOption(string keys, Action<int> action)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                int j = i;
                funcs.Add(char.ToUpper(keys[i]), () => action(j));
            }
        }

        public void ProcessInput()
        {
            while (true)
            {
                char k = char.ToUpper(Console.ReadKey(true).KeyChar);
                if (funcs.ContainsKey(k))
                {
                    funcs[k]();
                    break;
                }
            }
        }
    }
}