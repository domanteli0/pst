using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

public static class Extensions
{
    public static void Down(this IWebDriver driver)
    {
        if (Environment.GetEnvironmentVariable("DONT_QUIT") is null) { driver.Quit(); }
    }

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

    public static void MoveTo(this IWebDriver driver, IWebElement elem) =>
        driver.MoveBy(y: elem.Location.Y);

    public static void MoveBy(this IWebDriver driver, int y) =>
        new Actions(driver)
            .ScrollByAmount(0, y)
            .Perform();


    public static void Until(this WebDriverWait wait, Action<IWebDriver> action)
    {
        wait.Until<Util.Unit>(drv =>
        {
            action(drv);

            return new Util.Unit { };
        });
    }
}