using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace lab4;

// Užduotis:
// Sukurti du automatinius testus, uždavinio sąlygos:
// - [X] Prieš vykdant testus automatiniu būdu turi būti sukurtas naujas vartotojas.
// - [X] Abu testai turi prisijungti tuo pačiu sukurtuoju vartotoju.
// - [X] Testai ir naudotojo kūrimas turi būti atliekami atskirose webdriverio sesijose.
// - [ ] Pasinaudoti Unit testų anotacijomis iškviečiant ir uždarant webdriverio sesijas.
// - [ ] Paleisti testus per Jenkins jobą su cron scheduleriu.
// - [ ] Sutvarkyt checkout (4)

public class CreatedUserFixture
{
    private Random rng = new Random();
    private readonly string password;
    private readonly string email;

    public CreatedUserFixture() {
        (email, password) = createUser();
    }

    public (String, String) createUser()
    {
        IWebDriver driver = new ChromeDriver();
        var password = "password";
        var email = $"{rng.NextInt64()}@gmail.com";

        try
        {
            // Naudotojo kūrimo eiga:
            // 1. Atsidaryti tinklalapį https://demowebshop.tricentis.com/
            driver.Manage().Window.Maximize();
            driver.Url = "https://demowebshop.tricentis.com/";

            // 2. Spausti 'Log in'
            driver.FindButton(withLabel: "Log in", withTagName: "a").Click();

            // 3. Spausti 'Register' skiltyje 'New Customer'
            driver.FindButton(withTagName: "input", withAttribute: ("value", "Register")).Click();

            // 4. Užpildyti registracijos formos laukus
            driver.FindButton(withTagName: "input", withAttribute: ("id", "gender-male")).Click();
            driver.FindButton(withTagName: "input", withAttribute: ("id", "FirstName")).SendKeys("Vardenis");
            driver.FindButton(withTagName: "input", withAttribute: ("id", "LastName")).SendKeys("Pavardenis");
            driver.FindButton(withTagName: "input", withAttribute: ("id", "Email")).SendKeys(email);
            driver.FindButton(withTagName: "input", withAttribute: ("id", "Password")).SendKeys(password);
            driver.FindButton(withTagName: "input", withAttribute: ("id", "ConfirmPassword")).SendKeys(password);

            // 5. Spausti 'Register'
            driver.FindButton(withTagName: "input", withAttribute: ("id", "register-button")).Click();

            // 6. Spausti 'Continue'
            driver.FindButton(withTagName: "input", withAttribute: ("value", "Continue")).Click();

            return (email, password);
        }
        finally
        {
            driver.Quit();
        }
    }

    public string Password => password;

    public string Email => email;
}

public class UnitTest1 : IClassFixture<CreatedUserFixture>
{
    private readonly CreatedUserFixture _userFixture;

    public UnitTest1(CreatedUserFixture userFixture) {
        _userFixture = userFixture;
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

    private static void fillInAddress(IWebDriver driver) {
        driver
            .FindButton(withTagName: "select", withName: "BillingNewAddress.CountryId")
            .SelectByText("Lithuania");

        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.City").SendKeys("Vilnius");
        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.Address1").SendKeys("Naugarduko g., 24");
        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.ZipPostalCode").SendKeys("LT-03225");
        driver.FindButton(withTagName: "input", withName: "BillingNewAddress.PhoneNumber").SendKeys("+370 672 75623");
    }

    private static void selectExistingAddress(IWebDriver driver) {
        driver
            .FindButton(withTagName: "select", withName: "billing_address_id")
            .SelectByText("Naugarduko g., 24", partialMatch: true);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test1(List<string> data, Action<IWebDriver> addressFiller)
    {
        var (driver, wait) = up();

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
        WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(60));
        _wait.IgnoreExceptionTypes(typeof(ElementNotInteractableException));
        _wait.Until(drv => drv
            .FindButton(
                withTagName: "input",
                withValue: "Continue",
                withAttribute: ("class", "button-1 payment-method-next-step-button")
            ).Click()
        );

        // 10. 'Payment Information' spausti 'Continue'
        _wait.Until(drv => drv
            .FindButton(
                withTagName: "input",
                withValue: "Continue",
                withAttribute: ("class", "button-1 payment-info-next-step-button")
            ).Click()
        );

        // 11. 'Confirm Order' spausti 'Confirm'
        _wait.Until(drv => drv.FindButton(withTagName: "input", withValue: "Confirm").Click());

        // 12. Įsitikinti, kad užsakymas sėkmingai užskaitytas.
        wait.Until(drv => drv.FindButton(withTagName: "div", withAttribute: ("class", "title")).Displayed);
        Assert.Equal(
            expected: "Your order has been successfully processed!",
            actual: driver.FindButton(withTagName: "div", withAttribute: ("class", "title")).Text
        );

        if (Environment.GetEnvironmentVariable("DONT_QUIT") is null) { driver.Quit(); }
    }

    private (IWebDriver, WebDriverWait) up()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(60));

        driver.Manage().Window.Maximize();

        wait.PollingInterval = TimeSpan.FromMilliseconds(2000);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return (driver, wait);
    }

}

public static class Util
{
    public static By FindButtonLocator(
        string withTagName = "button",
        string? withLabel = null,
        string? withValue = null,
        string? withId = null,
        string? withName = null,
        (string, string)? withAttribute = null
    ) =>
        By.XPath(
                $"//{withTagName}{((withLabel is null) ? "" : $"[contains(.,'{withLabel}')]")}{((withId is null) ? "" : $"[./@id = \"{withId}\"]")}{((withValue is null) ? "" : $"[./@value = \"{withValue}\"]")}{((withName is null) ? "" : $"[./@name = \"{withName}\"]")}{((withAttribute is null) ? "" : $"[./@{withAttribute.Value.Item1} = \"{withAttribute.Value.Item2}\"]")}");

    public record Unit { }

    public static WhileImpl While(Func<bool> pred) => new WhileImpl(pred);

    public record WhileImpl(Func<bool> pred)
    {
        public void Do(Action action) { while (pred()) { action(); } }
    }
}

public static class MyExtentions
{

    public static IWebElement SelectByText(
        this IWebElement self,
        string text,
        bool partialMatch = false
    )
    {
        var materialDropDown = new SelectElement(self);
        materialDropDown.SelectByText(text, partialMatch);
        return self;
    }
    public static IWebElement FindButton(
        this ISearchContext driver,
        string withTagName = "button",
        string? withLabel = null,
        string? withValue = null,
        string? withId = null,
        string? withName = null,
        (string, string)? withAttribute = null
    ) =>
        driver.FindElement(Util.FindButtonLocator(withTagName, withLabel, withValue, withId, withName, withAttribute));

    public static void MoveTo(this IWebDriver driver, IWebElement elem)
    {
        int deltaY_ = elem.Location.Y;
        new Actions(driver)
            .ScrollByAmount(0, deltaY_)
            .Perform();
    }

    public static void Until(this WebDriverWait wait, Action<IWebDriver> action)
    {
        wait.Until<Util.Unit>(drv => {
            action(drv);

            return new Util.Unit {};
        });
    }
}