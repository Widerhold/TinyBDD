using NUnit.Framework;

namespace TinyBDD.Examples.FizzBuzz
{
    [TestFixture]
    public class FizzBuzzTests
    {
        private FizzBuzz _fizzBuzz;
        private int _number;

        [Test]
        public void NumberDivisibleBy3WillPrintFizz([Values(3,6,9,12)] int number)
        {
            this.Given(s => s.WeUseFizzBuzz())
                .When(s => s.TheNumberIs(number))
                .And("it is a multiple of 3")
                .Then(s => s.ItShouldReturnExpectedAnswer("Fizz"));
        }

        [Test]
        public void NumberDivisibleBy5WillPrintBuzz([Values(5, 10, 20, 25)] int number)
        {
            this.Given(s => s.WeUseFizzBuzz())
                .When(s => s.TheNumberIs(number), "the number is {0}")
                .And("it is a multiple of 5")
                .Then(s => s.ItShouldReturnExpectedAnswer("Buzz"));
        }

        [Test]
        public void NumberDivisibleBy3And5WillPrintFizzBuzz([Values(15)] int number)
        {
            this.Given(s => s.WeUseFizzBuzz())
                .When(s => s.TheNumberIs(number))
                .And("it is a multiple of 3 and 5")
                .Then(s => s.ItShouldReturnExpectedAnswer("FizzBuzz"));
        }

        [Test]
        public void NumberThatIsNotDivisibaleBy3And5WillPrintTheNumber([Values(1,2,4,7)] int number)
        {
            this.Given(s => s.WeUseFizzBuzz())
                .When(s => s.TheNumberIs(number))
                .Then(s => s.ItShouldReturnExpectedAnswer(number.ToString()));
        }

        private void TheNumberIs(int number)
        {
            _number = number;
        }

        private void ItShouldReturnExpectedAnswer(string expectedAnswer)
        {
            string answer = _fizzBuzz.DoFizzBuzz(_number);
            Assert.AreEqual(expectedAnswer, answer);
        }

        private void WeUseFizzBuzz()
        {
            _fizzBuzz = new FizzBuzz();
        }
    }
}
