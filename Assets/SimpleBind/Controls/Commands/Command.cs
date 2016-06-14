// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;

namespace SimpleBind.EventBinding
{
    public class Command
    {
        private bool _canExecute;

        private readonly Action _executeAction;
        private readonly Func<bool> _canExecuteAction;

        public string Label { get; private set; }

        public bool CanExecute
        {
            get
            {
                Refresh();
                return _canExecute;
            }
        }

        public Command(string label, Action executeAction) : this(label, executeAction, null)
        {
        }

        public Command(string label, Action executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
            Label = label;
            Refresh();
        }

        public void Execute()
        {
            _executeAction();
        }

        private void Refresh()
        {
            _canExecute = _canExecuteAction == null || _canExecuteAction();
        }
    }
}
