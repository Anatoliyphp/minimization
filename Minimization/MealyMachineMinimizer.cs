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
                string inputSignal = inputLineList.First();
                int i = 1;
                foreach (string state in states)
                {
                    int slashPosition = inputLineList[i].IndexOf('/');
                    string transitionState = inputLineList[i].Substring(0, slashPosition);
                    int outputSignal = Convert.ToInt32(inputLineList[i].Substring(slashPosition + 1, inputLineList[i].Length - slashPosition - 1));
                    if (!_transitionSourceStates.ContainsKey(state))
                    {
                        _transitionSourceStates.Add(state, new Dictionary<string, string>());
                    }
                    _transitionSourceStates[state][inputSignal] = transitionState;
                    if (!_transitionStates.ContainsKey(state))
                    {
                        _transitionStates.Add(state, new Dictionary<string, string>());
                    }
                    _transitionStates[state][inputSignal] = transitionState;
                    if (!_inputAndOutputStatesSignals.ContainsKey(state))
                    {
                        _inputAndOutputStatesSignals.Add(state, new Dictionary<string, int>());
                    }
                    _inputAndOutputStatesSignals[state][inputSignal] = outputSignal;
                    i++;
                }
            }
            Dictionary<int, List<string>> newEqualClassStates = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<string, Dictionary<string,int>> signals in _inputAndOutputStatesSignals)
            {
                if (!newEqualClassStates.ContainsKey(DictionaryHash(signals.Value)))
                {
                    newEqualClassStates.Add(DictionaryHash(signals.Value), new List<string>());
                }
                newEqualClassStates[DictionaryHash(signals.Value)].Add(signals.Key);
            }
            foreach (KeyValuePair<int, List<string>> equalClassToStates in newEqualClassStates)
            {
                foreach (string state in equalClassToStates.Value)
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
            
            List<string> inputSignals = new List<string>();
            foreach (var inputSignal in _transitionSourceStates.First().Value)
            {
                if (!inputSignals.Contains(inputSignal.Value))
                {
                    inputSignals.Add(inputSignal.Value);
                }
            }
            
            foreach (string inputSignal in inputSignals)
            {
                _outputStream.Write($"{inputSignal} ");
                foreach (List<string> states in _equalClassesStates.Values)
                {
                    string state = states.First();
                    string outputstate = _transitionSourceStates[state][inputSignal];
                    int outputSugnal = _inputAndOutputStatesSignals[outputstate][inputSignal];
                    int ouputstateToEqualClass =
                        _equalClassesStates.FirstOrDefault(e => e.Value.Contains(outputstate)).Key;
                    _outputStream.Write($"{ouputstateToEqualClass}/{outputSugnal} ");
                }
                _outputStream.WriteLine();
            }
            _outputStream.Close();
        }
    }
}