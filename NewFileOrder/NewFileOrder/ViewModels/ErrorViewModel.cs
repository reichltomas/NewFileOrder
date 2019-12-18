using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace NewFileOrder.ViewModels
{
    class ErrorViewModel : ViewModelBase
    {
        string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            private set => this.RaiseAndSetIfChanged(ref errorMessage, value);
        }

        public ErrorViewModel(string msg)
        {
            ErrorMessage = msg;
        }
    }
}
