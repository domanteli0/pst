
using OpenQA.Selenium;

public class CreatedUserFixture : IDisposable
{
    private Random rng = new Random();
    private readonly string password;
    private readonly string email;
    private IWebDriver driver = Util.up();

    public CreatedUserFixture()
    {
        (email, password) = createUser();
    }

    private (String, String) createUser()
    {
        var password = "password";
        var email = $"{rng.NextInt64()}@gmail.com";

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

    public void Dispose()
    {
        if (Environment.GetEnvironmentVariable("DONT_QUIT") is null) { driver.Quit(); }
    }

    public string Password => password;

    public string Email => email;
}