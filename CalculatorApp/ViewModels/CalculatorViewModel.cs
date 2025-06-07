using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculatorApp.Models;
using System.Windows.Input;

namespace CalculatorApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Calculator. This class manages the application's state and logic,
    /// acting as the intermediary between the View (UI) and the Model (calculation engine).
    /// It exposes properties and commands for data binding in the XAML.
    /// </summary>
    public class CalculatorViewModel : ObservableObject
    {
        private readonly CalculatorModel _calculator;
        private string _displayText = "0";
        private bool _isNewEntry = true;
        private bool _isErrorState = false;

        /// <summary>
        /// Gets or sets the text to be displayed on the calculator screen.
        /// This property is bound to the main TextBlock in the UI. When its value
        /// changes, it notifies the UI to update.
        /// </summary>
        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to append a number or decimal point to the display.
        /// </summary>
        public ICommand AppendCommand { get; }
        /// <summary>
        /// Gets the command to apply a mathematical operator.
        /// </summary>
        public ICommand OperatorCommand { get; }
        /// <summary>
        /// Gets the command to calculate a mathematical operation.
        /// </summary>
        public ICommand CalculateCommand { get; }
        /// <summary>
        /// Gets the command to clear the calculator's state and display.
        /// </summary>
        public ICommand ClearCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorViewModel"/> class.
        /// Sets up the calculator model and initializes the ICommand properties.
        /// </summary>
        public CalculatorViewModel()
        {
            _calculator = new CalculatorModel();

            AppendCommand = new RelayCommand(Append);
            OperatorCommand = new RelayCommand(ApplyOperator);
            CalculateCommand = new RelayCommand(Calculate, CanCalculate);
            ClearCommand = new RelayCommand(Clear);
        }

        /// <summary>
        /// Resets the calculator to its default state.
        /// </summary>
        /// <param name="obj">Unused command parameter.</param>
        private void Clear(object obj)
        {
            DisplayText = "0";
            _isNewEntry = true;
            _isErrorState = false;
        }

        /// <summary>
        /// Appends a digit or decimal point to the current display text.
        /// </summary>
        /// <param name="parameter">The string representation of the digit or "." to append.</param>
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

        /// <summary>
        /// Appends a mathematical operator to the expression.
        /// </summary>
        /// <param name="parameter">The string representation of the operator to apply.</param>
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

       
        /// <summary>
        /// Evaluates the current expression and displays the result.
        /// Calls the model's Evaluate method and handles any exceptions by
        /// setting the display to an "Error" state.
        /// </summary>
        /// <param name="obj">Unused command parameter.</param>
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

        /// <summary>
        /// Determines whether the 'Equals' command can be executed.
        /// </summary>
        /// <param name="obj">Unused command parameter.</param>
        /// <returns>True if the expression is in a valid state to be calculated.</returns>
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

        /// <summary>
        /// Helper method to check if a character is an operator.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is a valid operator.</returns>
        private bool IsOperator(char c)
        {
            return "+-×÷".Contains(c);
        }
    }
}
