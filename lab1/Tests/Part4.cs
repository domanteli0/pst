using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V119.ServiceWorker;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace lab4;

// Užduotis:
// Sukurti du automatinius testus, uždavinio sąlygos:
// - [X] Prieš vykdant testus automatiniu būdu turi būti sukurtas naujas vartotojas.
// - [X] Abu testai turi prisijungti tuo pačiu sukurtuoju vartotoju.
// - [X] Testai ir naudotojo kūrimas turi būti atliekami atskirose webdriverio sesijose.
// - [+-] Pasinaudoti Unit testų anotacijomis iškviečiant ir uždarant webdriverio sesijas.
// - [ ] Paleisti testus per Jenkins jobą su cron scheduleriu.

public class Part4 : IClassFixture<CreatedUserFixture>
{
    private readonly CreatedUserFixture _userFixture;

    public Part4(CreatedUserFixture userFixture)
    {
        _userFixture = userFixture;
    }

    private static void fillInAddress(IWebDriver driver)
    {
        driver
            .FindButton(withTagName: "select", withName: "BillingNewAddress.CountryId")
            .SelectByText("Lithuania");

        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.City").SendKeys("Vilnius");
        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.Address1").SendKeys("Naugarduko g., 24");
        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.ZipPostalCode").SendKeys("LT-03225");
        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.PhoneNumber").SendKeys("+370 672 75623");
    }

    private static void selectExistingAddress(IWebDriver driver)
    {
        driver
            .FindButton(withTagName: "select", withName: "billing_address_id")
            .SelectByText("Naugarduko g., 24", partialMatch: true);
    }

    // Disables: complaining about not invoking `fillInAddress` and `selectExistingAddress`
#pragma warning disable CS8974
    // data1.txt: data2.txt:
    // 3rd Album 3rd Album
    // 3rd Album Music 2
    // 3rd Album
    public static IEnumerable<object[]> Data =>
        new List<object[]> {
            new object[] { new List<string> { "3rd Album", "3rd Album", "3rd Album" }, fillInAddress},
            new object[] { new List<string> { "3rd Album", "Music 2" }, selectExistingAddress},
        };
#pragma warning restore CS8974

    [Theory]
    [MemberData(nameof(Data))]
    public void Test1(List<string> data, Action<IWebDriver> addressFiller)
    {
        var driver = Util.up();

        // 1. Atsidaryti tinklalapį https://demowebshop.tricentis.com/
        driver.Url = "https://demowebshop.tricentis.com/";

        // 2. Spausti 'Log in'
        driver.FindButton(withLabel: "Log in", withTagName: "a").Click();

        // 3. Užpildyti 'Email:', 'Password:' ir spausti 'Log in'
        driver.FindButton(withTagName: "input", withId: "Email").SendKeys(_userFixture.Email);
        driver.FindButton(withTagName: "input", withId: "Password").SendKeys(_userFixture.Password);
        driver.FindButton(withTagName: "input", withValue: "Log in").Click();

        // 4. Šoniniame meniu pasirinkti 'Digital downloads'
        driver.FindButton(withTagName: "a", withLabel: "Digital downloads").Click();

        // 5. Pridėti į krepšelį prekes nuskaitant tekstinį failą (pirmam testui skaityti iš data1.txt, antram testui skaityti iš data2.txt)
        data.ForEach(data =>
        {
            var pollState = () => driver
                .FindButton(withTagName: "a", withLabel: "Shopping cart")
                .FindButton(withTagName: "span", withAttribute: ("class", "cart-qty"))
                .Text;

            var currentState = pollState();

            Util.While(() => pollState().Equals(currentState))
                .Do(() => driver
                    .FindButton(withTagName: "div", withAttribute: ("class", "details"))
                    .FindButton(withTagName: "input", withValue: "Add to cart")
                    .Click()
                );
        }
        );

        // 6. Atsidaryti 'Shopping cart'
        driver.FindButton(withTagName: "a", withLabel: "Shopping cart").Click();

        // 7. Paspausti 'I agree' varnelę ir mygtuką 'Checkout'
        driver.FindButton(withTagName: "input", withId: "termsofservice").Click();
        driver.FindButton(withLabel: "Checkout").Click();

        // 8. 'Billing Address' pasirinkti jau esantį adresą arba supildyti naujo adreso laukus, spausti 'Continue'
        addressFiller(driver);
        driver.FindButton(withTagName: "input", withValue: "Continue").Click();

        // 9. 'Payment Method' spausti 'Continue'
        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));
        wait.IgnoreExceptionTypes(typeof(ElementNotInteractableException));
        wait.Until(drv => drv
            .FindButton(
                withTagName: "input",
                withValue: "Continue",
                withAttribute: ("class", "button-1 payment-method-next-step-button")
            ).Click()
        );

        // 10. 'Payment Information' spausti 'Continue'
        wait.Until(drv => drv
            .FindButton(
                withTagName: "input",
                withValue: "Continue",
                withAttribute: ("class", "button-1 payment-info-next-step-button")
            ).Click()
        );

        // 11. 'Confirm Order' spausti 'Confirm'
        wait.Until(drv => drv.FindButton(withTagName: "input", withValue: "Confirm").Click());

        // 12. Įsitikinti, kad užsakymas sėkmingai užskaitytas.
        wait = new(driver, TimeSpan.FromSeconds(60))
        {
            PollingInterval = TimeSpan.FromSeconds(1)
        };
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.Until(drv => drv.FindButton(withTagName: "div", withAttribute: ("class", "title")).Displayed);
        Assert.Equal(
            expected: "Your order has been successfully processed!",
            actual: driver.FindButton(withTagName: "div", withAttribute: ("class", "title")).Text
        );

    }

}
