using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TinyBDD
{
    public class TestClause<T>
    {
        private readonly T _value;

        #region Constructor
        public TestClause(T value, string methodName, Expression<Action<T>> item)
        {
            Console.WriteLine("Scenario: " + UppercaseFirst(GetFormattedString(methodName).ToLower()) + "\n");
            _value = value;
            ExecuteStep(item);
        }
        #endregion

        #region Steps
        public TestClause<T> And(Expression<Action<T>> item)
        {
            ExecuteStep(item, "And");
            return this;
        }

        public TestClause<T> When(Expression<Action<T>> item)
        {
            ExecuteStep(item);
            return this;
        }

        public TestClause<T> Then(Expression<Action<T>> item)
        {
            ExecuteStep(item);
            return this;
        }
        #endregion

        private void ExecuteStep(Expression<Action<T>> expression, string prefix = "")
        {
            var message = ComposeMessage(expression, prefix);
            try
            {
                expression.Compile().Invoke(_value);
                Console.WriteLine(UppercaseFirst(message));
            }
            catch (Exception e)
            {
                Console.WriteLine(UppercaseFirst(message) + " failed: " + e.Message);
                throw new Exception(e.Message);
            }

        }

        private string ComposeMessage(Expression<Action<T>> expression, string prefix)
        {
            var methodCallExp = (MethodCallExpression)expression.Body;
            var parameteres = methodCallExp.Method.GetParameters();
            var arguments = methodCallExp.Arguments;
            string message = "";
            string methodName = methodCallExp.Method.Name;


            if (!string.IsNullOrEmpty(prefix))
                message = prefix + " ";

            var formattedString = GetFormattedString(methodName).ToLower();

            return message + ReplaceParametersWithValues(formattedString, parameteres, arguments);
        }

        private string ReplaceParametersWithValues(string formattedString, ParameterInfo[] parameteres, ReadOnlyCollection<Expression> arguments)
        {
            for (int i = 0; i < parameteres.Length; i++)
            {
                var t = parameteres[i];
                if (formattedString.Contains(t.Name))
                {
                    formattedString = formattedString.Replace(t.Name, arguments[i].ToString());
                }
            }
            return formattedString;
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
        #endregion //Helpers

    }
}