using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ModelViewViewModel.commands
{

    public class TriggerCommand : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(TriggerCommand), new PropertyMetadata(null, OnCommandParameterChanged));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(TriggerCommand), new PropertyMetadata(null, OnCommandChanged));

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (!IsAssociatedObjectEnabled())
            {
                return;
            }

            ICommand command = Command;
            object commandParameter = CommandParameter;

            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            SetEnabledByCanExecute();
        }

        private static void OnCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as TriggerCommand;
            if (element == null)
            {
                return;
            }

            if (dependencyPropertyChangedEventArgs.OldValue != null)
            {
                ((ICommand)dependencyPropertyChangedEventArgs.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)dependencyPropertyChangedEventArgs.NewValue;

            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }

            element.SetEnabledByCanExecute();
        }

        private static void OnCommandParameterChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sender = dependencyObject as TriggerCommand;
            if (sender == null)
            {
                return;
            }

            if (sender.AssociatedObject == null)
            {
                return;
            }

            sender.SetEnabledByCanExecute();
        }

        private bool IsAssociatedObjectEnabled()
        {
            FrameworkElement element = AssociatedObject;

            return element != null && element.IsEnabled;
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            SetEnabledByCanExecute();
        }

        private void SetEnabledByCanExecute()
        {
            FrameworkElement element = AssociatedObject;

            if (element == null)
            {
                return;
            }

            ICommand command = Command;

            if (command != null)
            {
                element.IsEnabled = command.CanExecute(CommandParameter);
            }
        }
    }
}