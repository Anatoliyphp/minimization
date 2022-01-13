using System.Collections.Generic;
using System.IO;

namespace Minimization
{
    public class MachineMinimizer
    {
        protected readonly StreamReader _inputStream;
        protected readonly StreamWriter _outputStream;
        protected readonly Dictionary<int, int> _equalClasses = new();
        protected readonly Dictionary<int, Dictionary<int,int>> _transitionSourceStates = new();
        protected readonly Dictionary<int, Dictionary<int,int>> _transitionStates = new();
        protected readonly Dictionary<int, Dictionary<int, int>> _inputAndOutputStatesSignals = new();
        protected readonly Dictionary<int, List<int>>_equalClassesStates = new();

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
            foreach (KeyValuePair<int, Dictionary<int, int>> statesTransition in _transitionStates)
            {
                foreach (KeyValuePair<int, int> transition in statesTransition.Value)
                {
                    statesTransition.Value[transition.Key] = _equalClasses[_transitionSourceStates[statesTransition.Key][transition.Key]];
                }
            }
        }
        
        public bool UpdateEqualClasses()
        {
            bool isMinimized = false;
            int numberOfOldEqualClasses = _equalClassesStates.Count;
            
            List<Dictionary<int, List<int>>> newEqualClassesStates = new List<Dictionary<int, List<int>>>();
            for (int i = 0; i < numberOfOldEqualClasses; i++)
            {
                newEqualClassesStates.Add(new Dictionary<int, List<int>>());
            }

            int index = 0;
            foreach (KeyValuePair<int, List<int>> equalClassToStatesElement in _equalClassesStates)
            {
                foreach (int state in equalClassToStatesElement.Value)
                {
                    int equalClassHash = DictionaryHash(_transitionStates[state]);
                    if (!newEqualClassesStates[index].ContainsKey(equalClassHash))
                    {
                        newEqualClassesStates[index].Add(equalClassHash, new List<int>());
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
            foreach (Dictionary<int, List<int>> newEqualClassStates in newEqualClassesStates)
            {
                foreach (KeyValuePair<int, List<int>> newEqualClassStatesElement in newEqualClassStates)
                {
                    foreach (int state in newEqualClassStatesElement.Value)
                    {
                        _equalClasses[state] = _equalClassesStates.Count + 1;
                    }
                    _equalClassesStates[_equalClassesStates.Count + 1] = newEqualClassStatesElement.Value;
                }
            }
            return isMinimized;
        }

        public int DictionaryHash(Dictionary<int, int> equalClass)
        {
            int hashCode = 23;
            foreach (KeyValuePair<int, int> el in equalClass )
            {
                hashCode = (hashCode * 37) + el.Key;
                hashCode = (hashCode * 37) + el.Value;
            }

            return hashCode;
        }
    }
}