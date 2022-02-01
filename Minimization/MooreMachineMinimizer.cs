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
                if (!_equalClassesStates.ContainsKey(Convert.ToInt32(outputStateSignals[i])))
                {
                    _equalClassesStates.Add(Convert.ToInt32(outputStateSignals[i]), new List<string>());
                }

                _equalClassesStates[Convert.ToInt32(outputStateSignals[i])].Add(s);
                _equalClasses[s] = Convert.ToInt32(outputStateSignals[i]);
                statesOrders.Add(s);
                ++i;
            }

            string inputLine;
            while ((inputLine = _inputStream.ReadLine()) != null)
            {
                List<string> inputLineList = inputLine.Split(" ").ToList();
                string inputSignal = inputLineList.First();
                i = 1;
                foreach (string state in statesOrders)
                {
                    string newState = inputLineList[i];
                    if (!_transitionSourceStates.ContainsKey(state))
                    {
                        _transitionSourceStates.Add(state, new Dictionary<string, string>());
                    }

                    _transitionSourceStates[state].Add(inputSignal, newState);
                    if (!_transitionStates.ContainsKey(state))
                    {
                        _transitionStates.Add(state, new Dictionary<string, string>());
                    }

                    _transitionStates[state].Add(inputSignal, newState);
                    if (!_inputAndOutputStatesSignals.ContainsKey(state))
                    {
                        _inputAndOutputStatesSignals.Add(state, new Dictionary<string, int>());
                    }

                    _inputAndOutputStatesSignals[state]
                        .Add(inputSignal, Convert.ToInt32(outputStateSignals[i - 1]));
                    ++i;
                }
            }
        }

        public void PrintMoore()
        {
            _outputStream.Write("  ");
            foreach (List<string> states in _equalClassesStates.Values)
            {
                string state = states.First();
                _outputStream.Write($"{state} ");
            }
            _outputStream.WriteLine();
            _outputStream.Write("  ");
            foreach (int key in _equalClassesStates.Keys)
            {
                _outputStream.Write($"{key} ");
            }
            _outputStream.WriteLine();
            List<string> inputSignals = new List<string>();
            foreach (var inputSignal in _transitionSourceStates.First().Value)
            {
                if (!inputSignals.Contains(inputSignal.Key))
                {
                    inputSignals.Add(inputSignal.Key);
                }
            }

            foreach (string inputSignal in inputSignals)
            {
                _outputStream.Write($"{inputSignal} ");
                foreach (List<string> states in _equalClassesStates.Values)
                {
                    string state = states.First();
                    string outputSignal = _transitionSourceStates[state][inputSignal];
                    //int ouputSignalToEqualClass =
                    //    _equalClassesStates.FirstOrDefault(e => e.Value.Contains(outputSignal)).Key;
                    _outputStream.Write($"{outputSignal} ");
                }
                _outputStream.WriteLine();
            }
            _outputStream.Close();
        }
    }
}