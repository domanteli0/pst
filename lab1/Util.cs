using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public static class Util
{
    public static IWebDriver up()
    {
        ChromeOptions options = new ChromeOptions();
        if (Environment.GetEnvironmentVariable("GUI") is null) options.AddArguments("--headless");
        options.AddArguments("--no-sandbox");
        options.AddArguments("--disable-dev-shm-usage");

        IWebDriver driver = new ChromeDriver(options);

        driver.Manage().Window.Maximize();
        return driver;
    }

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
