using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Minimization
{
    public class MachineMinimizer
    {
        protected readonly StreamReader _inputStream;
        protected readonly StreamWriter _outputStream;
        protected readonly Dictionary<string, int> _equalClasses = new();
        protected readonly Dictionary<string, Dictionary<string,string>> _transitionSourceStates = new();
        protected readonly Dictionary<string, Dictionary<string,string>> _transitionStates = new();
        protected readonly Dictionary<string, Dictionary<string, int>> _inputAndOutputStatesSignals = new();
        protected readonly Dictionary<int, List<string>>_equalClassesStates = new();

        public MachineMinimizer(StreamReader inputStream, StreamWriter outputStream)
        {
            _inputStream = inputStream;
            _outputStream = outputStream;
        }
        
        public bool Minimize()
        {
            UpdateTransitions();
            return UpdateEqualClasses();
        }
        
        /*извлекаем изначальное состояние перехода и подбираем к нему новый класс эквивалентности*/
        public void UpdateTransitions()
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> statesTransition in _transitionStates)
            {
                foreach (KeyValuePair<string, string> transition in statesTransition.Value)
                {
                    if (transition.Value == "#")
                    {
                        continue;
                    }
                    statesTransition.Value[transition.Key] = _equalClasses[_transitionSourceStates[statesTransition.Key][transition.Key]].ToString();
                }
            }
        }
        
        public bool UpdateEqualClasses()
        {
            bool isMinimized = false;
            int numberOfOldEqualClasses = _equalClassesStates.Count;
            
            List<Dictionary<int, List<string>>> newEqualClassesStates = new List<Dictionary<int, List<string>>>();
            for (int i = 0; i < numberOfOldEqualClasses; i++)
            {
                newEqualClassesStates.Add(new Dictionary<int, List<string>>());
            }

            int index = 0;
            foreach (KeyValuePair<int, List<string>> equalClassToStatesElement in _equalClassesStates)
            {
                foreach (string state in equalClassToStatesElement.Value)
                {
                    int equalClassHash = DictionaryHash(_transitionStates[state]);
                    if (!newEqualClassesStates[index].ContainsKey(equalClassHash))
                    {
                        newEqualClassesStates[index].Add(equalClassHash, new List<string>());
                    }
                    newEqualClassesStates[index][equalClassHash].Add(state);
                }
                if (newEqualClassesStates[index].Count != 1)
                {
                    isMinimized = true;
                }

                ++index;
                if (index == numberOfOldEqualClasses)
                {
                    break;
                }
            }
            _equalClassesStates.Clear();
            foreach (Dictionary<int, List<string>> newEqualClassStates in newEqualClassesStates)
            {
                foreach (KeyValuePair<int, List<string>> newEqualClassStatesElement in newEqualClassStates)
                {
                    foreach (string state in newEqualClassStatesElement.Value)
                    {
                        _equalClasses[state] = _equalClassesStates.Count + 1;
                    }
                    _equalClassesStates[_equalClassesStates.Count + 1] = newEqualClassStatesElement.Value;
                }
            }
            return isMinimized;
        }

        public int DictionaryHash(Dictionary<string, string> equalClass)
        {
            int hashCode = 23;
            foreach (KeyValuePair<string, string> el in equalClass )
            {
                char[] charArray = el.Key.ToCharArray();
                foreach (char symbol in charArray)
                {
                    int symbolCode = symbol;
                    hashCode = (hashCode * 37) + symbolCode;
                }
                char[] charArrayV = el.Value.ToCharArray();
                foreach (char symbol in charArrayV)
                {
                    int symbolCode = symbol;
                    hashCode = (hashCode * 37) + symbolCode;
                }
            }

            return hashCode;
        }

        public int DictionaryHash(Dictionary<string, int> equalClass)
        {
            int hashCode = 23;
            foreach (KeyValuePair<string, int> el in equalClass )
            {
                char[] charArray = el.Key.ToCharArray();
                foreach (char symbol in charArray)
                {
                    int symbolCode = symbol;
                    hashCode = (hashCode * 37) + symbolCode;
                }
                hashCode = (hashCode * 37) + el.Value;
            }

            return hashCode;
        }
    }
}