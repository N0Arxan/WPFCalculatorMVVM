using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CalculatorApp.Models
{
     /// <summary>
    /// Handles the core calculation logic for the calculator using the Shunting-yard algorithm.
    /// This provides fine-grained control over expression evaluation, including custom error handling.
    /// </summary>
    public class CalculatorModel
    {
        /// <summary>
        /// Evaluates a given mathematical expression string.
        /// </summary>
        /// <param name="expression">The infix mathematical expression (e.g., "5*2+10").</param>
        /// <returns>The result of the calculation as a double.</returns>
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
        /// Converts an infix expression string to a queue of tokens in Reverse Polish Notation (RPN).
        /// </summary>
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
        /// Evaluates a queue of tokens in RPN format.
        /// </summary>
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
        /// A tokenizer to split the expression into numbers and operators.
        /// </summary>
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
        /// Applies a mathematical operation to two operands.
        /// This is where we explicitly check for division by zero.
        /// </summary>
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
        
        private bool IsOperator(char c) => "+-×÷".Contains(c);
        private int GetPrecedence(char op) => (op == '+' || op == '-') ? 1 : 2;
    }
}