using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WindowsDock.Core
{
    class StringDelegateCommand : ICommand
    {
        Action<string> m_ExecuteTargets = delegate { };
        Func<bool> m_CanExecuteTargets = delegate { return false; };
        bool m_Enabled = false;


        public bool CanExecute(object parameter)
        {
            Delegate[] targets = m_CanExecuteTargets.GetInvocationList();
            foreach (Func<bool> target in targets)
            {
                m_Enabled = false;
                bool localEnable = target.Invoke();
                if (localEnable)
                {
                    m_Enabled = true;
                    break;
                }
            }
            return m_Enabled;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            if (m_Enabled)
                m_ExecuteTargets(parameter != null ? parameter.ToString() : null);
        }


        public event Action<string> ExecuteTargets
        {
            add
            {
                m_ExecuteTargets += value;
            }
            remove
            {
                m_ExecuteTargets -= value;
            }
        }

        public event Func<bool> CanExecuteTargets
        {
            add
            {
                m_CanExecuteTargets += value;
                CanExecuteChanged(this, EventArgs.Empty);
            }

            remove
            {
                m_CanExecuteTargets -= value;
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
