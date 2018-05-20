using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace TinyBDD
{
    public class TestClause<T>
    {
        private readonly T _value;
        private readonly List<string> _testReport = new List<string>();

        #region Constructor
        public TestClause(T value, string methodName, Expression<Action<T>> item)
        {
            Console.WriteLine("Scenario: " + UppercaseFirst(GetFormattedString(methodName).ToLower()) + "\n");
            _value = value;
            ExecuteStep(item, false, "Given ");
        }
        #endregion

        #region Steps
        public TestClause<T> And(Expression<Action<T>> item, bool ignoreParameter = false)
        {
            ExecuteStep(item, ignoreParameter, "And ");
            return this;
        }

        public TestClause<T> And(string message)
        {
            Console.WriteLine("And "+message);
            return this;
        }

        public TestClause<T> When(Expression<Action<T>> item, string customMessage)
        {
            ExecuteStep(item, customMessage);
            return this;
        }

        public TestClause<T> When(Expression<Action<T>> item, bool ignoreParameter = false)
        {
            ExecuteStep(item, ignoreParameter, "When ");
            return this;
        }

        public TestClause<T> Then(Expression<Action<T>> item, string customMessage)
        {
            ExecuteStep(item, customMessage);
            return this;
        }

        public TestClause<T> Then(Expression<Action<T>> item, bool ignoreParameter = false)
        {
            ExecuteStep(item, ignoreParameter, "Then ");
            return this;
        }
        #endregion

        private void ExecuteStep(Expression<Action<T>> expression, bool embedVariables, string prefix = "")
        {
            var message = ComposeMessage(expression, prefix, embedVariables);
            Execute(expression, message);
        }

        private void ExecuteStep(Expression<Action<T>> expression, string message)
        {
            message = ComposeCustomMessage(expression, message);
            Execute(expression, message);
        }

        private void Execute(Expression<Action<T>> expression, string message)
        {
            try
            {
                expression.Compile().Invoke(_value);
                var formattedMessage = UppercaseFirst(message);
                _testReport.Add(formattedMessage);
                Console.WriteLine(formattedMessage);
            }
            catch (Exception e)
            {
                var errorMessage = UppercaseFirst(message) + " failed: " + e.Message;
                _testReport.Add(errorMessage);
                Console.WriteLine(errorMessage);
                throw new Exception(e.Message);
            }
        }

        public void Report(Action<List<string>> handler)
        {
            handler.Invoke(_testReport);
        }

        private string ComposeCustomMessage(Expression<Action<T>> expression, string customMessage)
        {
            var methodCallExp = (MethodCallExpression)expression.Body;
            var arguments = methodCallExp.Arguments;
            object[] parameterValues = new object[arguments.Count];
            for (int index = 0; index < arguments.Count; index++)
            {
                var argument = arguments[index];
                var exp = ResolveMemberExpression(argument);
                parameterValues[index] = (exp == null ? argument.ToString() : GetValue(exp).ToString());
            }
            return string.Format(customMessage, parameterValues);
        }

        private string ComposeMessage(Expression<Action<T>> expression, string prefix, bool ignoreParameter)
        {
            var methodCallExp = (MethodCallExpression)expression.Body;
            var arguments = methodCallExp.Arguments;
            string methodName = methodCallExp.Method.Name;

            string message = GetFormattedString(methodName).ToLower();
            if (!message.StartsWith(prefix))
                message = prefix + message;

            if (arguments.Count == 1 && !ignoreParameter)
            {
                var exp = ResolveMemberExpression(arguments[0]);
                string value = exp == null ? arguments[0].ToString() : GetValue(exp).ToString();

                bool boolVal;
                if (bool.TryParse(value, out boolVal))
                {
                    if (!boolVal)
                    {
                        message = message.Replace(" is ", " is not ");
                    }
                }
                else
                {
                    message += " " + value;
                }

            }
            return message;
        }

        #region Helpers
        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string GetFormattedString(string s)
        {
            var r = new Regex(@"
                    (?<=[A-Z])(?=[A-Z][a-z]) |
                     (?<=[^A-Z])(?=[A-Z]) |
                     (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Replace(s, " ");
        }

        public static MemberExpression ResolveMemberExpression(Expression expression)
        {
            if(expression is MemberExpression)
                return (MemberExpression)expression;
            return null;
        }

        private static object GetValue(MemberExpression exp)
        {
            if (exp.Expression is ConstantExpression)
            {
                return (((ConstantExpression)exp.Expression).Value)
                        .GetType()
                        .GetField(exp.Member.Name)
                        .GetValue(((ConstantExpression)exp.Expression).Value);
            }
            if (exp.Expression is MemberExpression)
            {
                return GetValue((MemberExpression)exp.Expression);
            }
            throw new NotImplementedException();
        }
        #endregion //Helpers

    }

    public static class TestClauseExtensions
    {
        public static TestClause<T> Given<T>(this T t, Expression<Action<T>> expression)
            where T : class, new()
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            return new TestClause<T>(t, method.Name, expression);
        }
    }
}