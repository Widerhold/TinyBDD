using NUnit.Framework;

namespace TinyBDD.Examples.Atm
{
    [Story("Atm",
        As = "an Account Holder",
        IWant = "to withdraw cash from an ATM",
        SoThat = "I can get money when the bank is closed")]
    public class AtmTests
    {
        private Card _card;
        private Atm _atm;

        [Test]
        public void AccountHasSufficientFund()
        {
            this.Given(s => s.TheAccountBalanceIs(100))
            .And("the Card is valid")
            .And(s => s.TheMachineContains(100))
            .When(s => s.TheAccountHolderRequests(20))
            .Then(s => s.TheAtmShouldDispense(20))
            .And(s => s.TheAccountBalanceShouldBe(80))
            .And(s => s.CardIsRetained(false));
        }

        [Test]
        public void CardHasBeenDisabled()
        {
            this.Given(s => s.TheCardIsDisabled())
                .And(s => s.TheMachineContains(100))
                .When(s => s.TheAccountHolderRequests(20))
                .Then(s => s.CardIsRetained(true))
                .And(s => s.TheAtmShouldSayTheCardHasBeenRetained());
        }

        [Test]
        public void AccountHasInsufficientFund()
        {
            this.Given(s => s.TheAccountBalanceIs(10))
                .And("the Card is valid")
                .And(s => s.TheMachineContains(100))
                .When(s => s.TheAccountHolderRequests(20))
                .Then(s => s.TheAtmShouldDispense(0), "the atm should not dispense any money")
                .And(s => s.TheAtmShouldSayInsufficentFunds());
        }

        #region Given-Methods
        public void TheAccountBalanceIs(int balance)
        {
            _card = new Card(true, balance);
        }

        public void TheCardIsDisabled()
        {
            _card = new Card(false, 100);
        }

        public void TheMachineContains(int atmBalance)
        {
            _atm = new Atm(atmBalance);
        }
        #endregion

        #region When-Methods
        public void TheAccountHolderRequests(int moneyRequest)
        {
            _atm.RequestMoney(_card, moneyRequest);
        }
        #endregion

        #region Then-Methods
        public void TheAtmShouldDispense(int dispensedMoney)
        {
            Assert.AreEqual(_atm.DispenseValue, dispensedMoney);
        }

        public void TheAccountBalanceShouldBe(int balance)
        {
            Assert.AreEqual(_card.AccountBalance, balance);
        }

        public void CardIsRetained(bool cardIsRetained)
        {
            Assert.AreEqual(_atm.CardIsRetained, cardIsRetained);
        }

        void TheAtmShouldSayTheCardHasBeenRetained()
        {
            Assert.AreEqual(_atm.Message, DisplayMessage.CardIsRetained);
        }

        void TheAtmShouldSayInsufficentFunds()
        {
            Assert.AreEqual(_atm.Message, DisplayMessage.InsufficientFunds);
        }
        #endregion
    }
}
