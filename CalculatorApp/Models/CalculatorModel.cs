using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CalculatorApp.Models
{
    /// <summary>
    /// Core calculation engine for the calculator.
    /// This class implements the Shunting-yard algorithm to parse and evaluate
    /// mathematical expressions from an infix notation string. It correctly handles
    /// operator precedence and provides specific error handling for invalid operations,
    /// such as division by zero.
    /// </summary>
    public class CalculatorModel
    {
        /// <summary>
        /// Evaluates a mathematical expression provided in infix notation.
        /// </summary>
        /// <param name="expression">The mathematical expression string (e.g., "3+5*2").</param>
        /// <returns>A double representing the calculated result.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the expression is malformed, contains a syntax error, or involves an invalid operation like division by zero.
        /// </exception>
        public double Evaluate(string expression)
        {
            try
            {
                // Step 1: Convert the infix expression to Reverse Polish Notation (RPN) using Shunting-yard.
                var rpnQueue = ToReversePolishNotation(expression);

                // Step 2: Evaluate the RPN expression.
                var result = EvaluateRpn(rpnQueue);
                
                // Check for NaN or Infinity results which can occur from operations like 0/0
                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    throw new InvalidOperationException("Invalid operation result.");
                }

                return result;
            }
            catch(Exception)
            {
                // Catch any exception during parsing or evaluation (e.g., division by zero, syntax error)
                // and re-throw as a standardized exception for the ViewModel to handle.
                throw new InvalidOperationException("Invalid Expression");
            }
        }
        
        /// <summary>
        /// Converts an infix expression to a Reverse Polish Notation (RPN) queue.
        /// Implements the Shunting-yard algorithm to reorder tokens based on operator precedence.
        /// </summary>
        /// <param name="infix">The expression string in infix notation.</param>
        /// <returns>A queue of strings representing the expression in RPN.</returns>
        private Queue<string> ToReversePolishNotation(string infix)
        {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<char>();
            var tokens = Tokenize(infix);

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out _))
                {
                    outputQueue.Enqueue(token);
                }
                else if (IsOperator(token[0]))
                {
                    while (operatorStack.Any() && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token[0]))
                    {
                        outputQueue.Enqueue(operatorStack.Pop().ToString());
                    }
                    operatorStack.Push(token[0]);
                }
            }

            while (operatorStack.Any())
            {
                outputQueue.Enqueue(operatorStack.Pop().ToString());
            }

            return outputQueue;
        }

        /// <summary>
        /// Evaluates an expression stored in a Reverse Polish Notation (RPN) queue.
        /// </summary>
        /// <param name="rpnQueue">The queue of tokens in RPN format.</param>
        /// <returns>The final calculated result as a double.</returns>
        private double EvaluateRpn(Queue<string> rpnQueue)
        {
            var operandStack = new Stack<double>();

            while (rpnQueue.Any())
            {
                var token = rpnQueue.Dequeue();
                if (double.TryParse(token, out double number))
                {
                    operandStack.Push(number);
                }
                else if (IsOperator(token[0]))
                {
                    if (operandStack.Count < 2) throw new ArgumentException("Syntax Error.");

                    var operand2 = operandStack.Pop();
                    var operand1 = operandStack.Pop();

                    double result = ApplyOperation(token[0], operand1, operand2);
                    operandStack.Push(result);
                }
            }
            
            if (operandStack.Count != 1) throw new ArgumentException("Syntax Error.");

            return operandStack.Pop();
        }

        /// <summary>
        /// Splits the raw expression string into a list of numbers and operators.
        /// </summary>
        /// <param name="expression">The raw input string from the calculator display.</param>
        /// <returns>A list of strings, where each string is a number or an operator.</returns>
        private List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            string numberBuffer = "";

            foreach (char c in expression)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    numberBuffer += c;
                }
                else if (IsOperator(c))
                {
                    if (!string.IsNullOrEmpty(numberBuffer))
                    {
                        tokens.Add(numberBuffer);
                        numberBuffer = "";
                    }
                    tokens.Add(c.ToString());
                }
            }
            if (!string.IsNullOrEmpty(numberBuffer))
            {
                tokens.Add(numberBuffer);
            }
            return tokens;
        }


        /// <summary>
        /// Applies a single mathematical operation to two operands.
        /// </summary>
        /// <param name="op">The character representing the operator (+, -, ×, ÷).</param>
        /// <param name="a">The first operand (left-hand side).</param>
        /// <param name="b">The second operand (right-hand side).</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="DivideByZeroException">Thrown specifically when attempting to divide by zero.</exception>
        private double ApplyOperation(char op, double a, double b)
        {
            switch (op)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '×': return a * b;
                case '÷':
                    if (b == 0)
                    {
                        // *** THIS IS THE CRITICAL CHECK ***
                        throw new DivideByZeroException("Cannot divide by zero.");
                    }
                    return a / b;
                default:
                    throw new ArgumentException("Invalid operator.");
            }
        }
        
        /// <summary>
        /// Checks if a character is a valid operator.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is one of +, -, ×, or ÷.</returns>
        private bool IsOperator(char c) => "+-×÷".Contains(c);
        
        /// <summary>
        /// Determines the precedence of a mathematical operator.
        /// </summary>
        /// <param name="op">The operator character.</param>
        /// <returns>An integer representing precedence (2 for ×/÷, 1 for +/-).</returns>
        private int GetPrecedence(char op) => (op == '+' || op == '-') ? 1 : 2;
    }
}