using System;
using System.Collections.Generic;

namespace ProjektLS22
{
    /// <summary>
    /// For simple input handling. Stores a map of keys and actions to perform.
    /// </summary>
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
        /// <summary>
        /// Used for registering more keys at once with the same action, but different parameter.
        /// </summary>
        public void RegisterParametricOption(string keys, Action<int> action)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                int j = i;
                funcs.Add(char.ToUpper(keys[i]), () => action(j));
            }
        }
        /// <summary>
        /// Waits until a valid key is pressed and then executes the corresponding action.
        /// </summary>
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