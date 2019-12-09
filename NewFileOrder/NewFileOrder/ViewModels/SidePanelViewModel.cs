using Avalonia.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.ViewModels
{
    class SidePanelViewModel : ViewModelBase
    {
        string searhPhrase;

        public string SearchPhrase 
        {
            get => searhPhrase;
            private set => this.RaiseAndSetIfChanged(ref searhPhrase, value);
        }

        public ReactiveCommand<KeyEventArgs, bool> SearchbarKeyPressed { get; }

        public SidePanelViewModel()
        {
            SearchbarKeyPressed = ReactiveCommand.Create<KeyEventArgs, bool> (
                argsPressed => (argsPressed.Key == Key.Return));
            SearchbarKeyPressed.Subscribe(value =>
                {
                    if (value)
                        Console.WriteLine(SearchPhrase);
                });
        }

    }
}
