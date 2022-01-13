using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Minimization
{ 
    public class MooreMachineMinimizer: MachineMinimizer
    {
        
        public MooreMachineMinimizer(StreamReader inputStream, StreamWriter outputStream) : base(inputStream, outputStream)
        {
        }
        
        public void ReadMooreMachine()
        {
            List<string> outputStateSignals = _inputStream.ReadLine().Substring(2).Split(" ").ToList();
            List<string> statesOrders = new List<string>();
            int i = 0;
            List<string> states = _inputStream.ReadLine().Substring(2).Split(" ").ToList();
            foreach (string s in states)
            {
                int state = Convert.ToInt32(s);
                if (!_equalClassesStates.ContainsKey(Convert.ToInt32(outputStateSignals[i])))
                {
                    _equalClassesStates.Add(Convert.ToInt32(outputStateSignals[i]), new List<int>());
                }

                _equalClassesStates[Convert.ToInt32(outputStateSignals[i])].Add(state);
                _equalClasses[state] = Convert.ToInt32(outputStateSignals[i]);
                statesOrders.Add(s);
                ++i;
            }

            string inputLine;
            while ((inputLine = _inputStream.ReadLine()) != null)
            {
                List<string> inputLineList = inputLine.Split(" ").ToList();
                int inputSignal = Convert.ToInt32(inputLineList.First());
                i = 1;
                foreach (string state in statesOrders)
                {
                    int newState = Convert.ToInt32(inputLineList[i]);
                    if (!_transitionSourceStates.ContainsKey(Convert.ToInt32(state)))
                    {
                        _transitionSourceStates.Add(Convert.ToInt32(state), new Dictionary<int, int>());
                    }

                    _transitionSourceStates[Convert.ToInt32(state)].Add(inputSignal, newState);
                    if (!_transitionStates.ContainsKey(Convert.ToInt32(state)))
                    {
                        _transitionStates.Add(Convert.ToInt32(state), new Dictionary<int, int>());
                    }

                    _transitionStates[Convert.ToInt32(state)].Add(inputSignal, newState);
                    if (!_inputAndOutputStatesSignals.ContainsKey(Convert.ToInt32(state)))
                    {
                        _inputAndOutputStatesSignals.Add(Convert.ToInt32(state), new Dictionary<int, int>());
                    }

                    _inputAndOutputStatesSignals[Convert.ToInt32(state)]
                        .Add(inputSignal, Convert.ToInt32(outputStateSignals[i - 1]));
                    ++i;
                }
            }
        }

        public void PrintMoore()
        {
            _outputStream.Write("  ");
            foreach (List<int> states in _equalClassesStates.Values)
            {
                int state = states.First();
                _outputStream.Write($"{_inputAndOutputStatesSignals[state][1]} ");
            }
            _outputStream.WriteLine();
            _outputStream.Write("  ");
            foreach (int key in _equalClassesStates.Keys)
            {
                _outputStream.Write($"{key} ");
            }
            _outputStream.WriteLine();
            int i = 1;
            while (i <= _transitionSourceStates[1].Count)
            {
                _outputStream.Write($"{i} ");
                foreach (List<int> states in _equalClassesStates.Values)
                {
                    int state = states.First();
                    int outputSignal = _transitionSourceStates[state][i];
                    int ouputSignalToEqualClass =
                        _equalClassesStates.FirstOrDefault(e => e.Value.Contains(outputSignal)).Key;
                    _outputStream.Write($"{ouputSignalToEqualClass} ");
                }
                i++;
                _outputStream.WriteLine();
            }
            _outputStream.Close();
        }
    }
}