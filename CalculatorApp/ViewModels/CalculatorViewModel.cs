using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculatorApp.Models;
using System.Windows.Input;

namespace CalculatorApp.ViewModels
{
    public class CalculatorViewModel : ObservableObject
    {
        private readonly CalculatorModel _calculator;
        private string _displayText = "0";
        private bool _isNewEntry = true;
        private bool _isErrorState = false;

        // The text displayed on the calculator's screen.
        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        // Commands for the calculator buttons.
        public ICommand AppendCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ClearCommand { get; }

        public CalculatorViewModel()
        {
            _calculator = new CalculatorModel();

            AppendCommand = new RelayCommand(Append);
            OperatorCommand = new RelayCommand(ApplyOperator);
            CalculateCommand = new RelayCommand(Calculate, CanCalculate);
            ClearCommand = new RelayCommand(Clear);
        }

        // Resets the calculator to its initial state.
        private void Clear(object obj)
        {
            DisplayText = "0";
            _isNewEntry = true;
            _isErrorState = false;
        }

        // Appends a number or a decimal point to the display.
        private void Append(object parameter)
        {
            if (_isErrorState) Clear(null);

            var input = parameter.ToString();

            if (_isNewEntry)
            {
                DisplayText = (input == ".") ? "0." : input;
                _isNewEntry = false;
            }
            else
            {
                if (input == "." && DisplayText.Contains(".")) return; // Prevent multiple dots
                DisplayText += input;
            }
        }

        // Applies an operator (+, -, ×, ÷)
        private void ApplyOperator(object parameter)
        {
            if (_isErrorState) Clear(null);
            
            _isNewEntry = false;

            var op = parameter.ToString();
            string[] operators = { "+", "-", "×", "÷" };

            // Avoid chaining operators like "5 * / 2" by replacing the last one.
            if (DisplayText.Length > 0 && operators.Contains(DisplayText.Trim().Last().ToString()))
            {
                DisplayText = DisplayText.Trim().Substring(0, DisplayText.Trim().Length - 1) + op;
            }
            else
            {
                DisplayText += op;
            }
        }

        // Performs the calculation when '=' is pressed.
        private void Calculate(object obj)
        {
            try
            {
                // This will call the new Shunting-yard implementation.
                var result = _calculator.Evaluate(DisplayText);
                DisplayText = result.ToString();
            }
            catch // This block now catches errors from the Shunting-yard model.
            {
                DisplayText = "Error";
                _isErrorState = true;
            }
            finally
            {
                // After a calculation, the next number should start a new entry.
                _isNewEntry = true;
            }
        }

        // Determines if the '=' button should be enabled.
        private bool CanCalculate(object obj)
        {
            // Prevents calculation if the expression is invalid, e.g., "5 +"
            if (string.IsNullOrWhiteSpace(DisplayText) || "Error".Equals(DisplayText))
            {
                return false;
            }
            
            char lastChar = DisplayText.Trim().Last();
            return !IsOperator(lastChar);
        }

        private bool IsOperator(char c)
        {
            return "+-×÷".Contains(c);
        }
    }
}
