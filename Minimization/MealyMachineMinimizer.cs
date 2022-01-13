using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Minimization
{
    public class MealyMachineMinimizer: MachineMinimizer
    {
        
        public MealyMachineMinimizer(StreamReader inputStream, StreamWriter outputStream) : base(inputStream, outputStream)
        {
        }
        
        public void ReadMealyMachine()
        {
            string statesString = _inputStream.ReadLine();
            statesString.Insert(0, " ");
            List<string> states = statesString.Split("   ").ToList();
            string inputLine;
            while ((inputLine = _inputStream.ReadLine()) != null)
            {
                List<string> inputLineList = inputLine.Split(" ").ToList();
                int inputSignal = Convert.ToInt32(inputLineList.First());
                int i = 1;
                foreach (string state in states)
                {
                    int slashPosition = inputLineList[i].IndexOf('/');
                    int transitionState = Convert.ToInt32(inputLineList[i].Substring(0, slashPosition));
                    int outputSignal = Convert.ToInt32(inputLineList[i].Substring(slashPosition + 1, inputLineList[i].Length - slashPosition - 1));
                    if (!_transitionSourceStates.ContainsKey(Convert.ToInt32(state)))
                    {
                        _transitionSourceStates.Add(Convert.ToInt32(state), new Dictionary<int, int>());
                    }
                    _transitionSourceStates[Convert.ToInt32(state)][inputSignal] = transitionState;
                    if (!_transitionStates.ContainsKey(Convert.ToInt32(state)))
                    {
                        _transitionStates.Add(Convert.ToInt32(state), new Dictionary<int, int>());
                    }
                    _transitionStates[Convert.ToInt32(state)][inputSignal] = transitionState;
                    if (!_inputAndOutputStatesSignals.ContainsKey(Convert.ToInt32(state)))
                    {
                        _inputAndOutputStatesSignals.Add(Convert.ToInt32(state), new Dictionary<int, int>());
                    }
                    _inputAndOutputStatesSignals[Convert.ToInt32(state)][inputSignal] = outputSignal;
                    i++;
                }
            }
            Dictionary<int, List<int>> newEqualClassStates = new Dictionary<int, List<int>>();
            foreach (KeyValuePair<int, Dictionary<int,int>> signals in _inputAndOutputStatesSignals)
            {
                if (!newEqualClassStates.ContainsKey(DictionaryHash(signals.Value)))
                {
                    newEqualClassStates.Add(DictionaryHash(signals.Value), new List<int>());
                }
                newEqualClassStates[DictionaryHash(signals.Value)].Add(signals.Key);
            }
            foreach (KeyValuePair<int, List<int>> equalClassToStates in newEqualClassStates)
            {
                foreach (int state in equalClassToStates.Value)
                {
                    _equalClasses[state] = _equalClassesStates.Count+ 1;
                }
                _equalClassesStates[_equalClassesStates.Count + 1] = equalClassToStates.Value;
            }
        }
        
        public void PrintMealy()
        {
            _outputStream.Write("  ");
            foreach (int key in _equalClassesStates.Keys)
            {
                _outputStream.Write($"{key}   ");
            }
            _outputStream.WriteLine();
            int i = 1;
            while (i <= _transitionSourceStates[1].Count)
            {
                _outputStream.Write($"{i} ");
                foreach (List<int> states in _equalClassesStates.Values)
                {
                    int state = states.First();
                    int outputstate = _transitionSourceStates[state][i];
                    int outputSugnal = _inputAndOutputStatesSignals[outputstate][i];
                    int ouputstateToEqualClass =
                        _equalClassesStates.FirstOrDefault(e => e.Value.Contains(outputstate)).Key;
                    _outputStream.Write($"{ouputstateToEqualClass}/{outputSugnal} ");
                }
                i++;
                _outputStream.WriteLine();
            }
            _outputStream.Close();
        }
    }
}