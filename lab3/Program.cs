using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
// using static OpenQA.Selenium.Support.UI.ExpectedConditions;

using System.Diagnostics;
using OpenQA.Selenium.Interactions;
using System.Security.Principal;
using System.Numerics;

// Užduotis 2:
// Galima naudoti implicit waits
//
// 1. Atsidaryti https://demoqa.com/
// 2. Pasirinkti "Elements" kortele,
// 3. Pasirinkti meniu punkta "Web Tables"
// 4. Prideti pakankamai elementų, kad atsirastų antras puslapis puslapiavime
// 5. Pasirinkti antra puslapi paspaudus "Next"
// 6. Ištrinti elementa antrajame puslapyje
// 7. Įsitikinti, kad automatiškai puslapiavimas perkeliamas į pirmąjį puslapį ir kad puslapiu skaičius

var driver = new ChromeDriver();
driver.Manage().Window.Maximize();

var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

wait.PollingInterval = TimeSpan.FromMilliseconds(2000);
wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
// wait.Until<object>((webDriver) => {
//     return "";
// });

var fillForm = () => {
    var addBtn = driver.FindButton(withLabel: "Add");
    driver.MoveTo(addBtn);
    addBtn.Click();

    wait.Until(d => d.FindElement(By.CssSelector(".modal-title")));

    driver.FindElement(By.CssSelector("#firstName")).SendKeys("Vardenis");
    driver.FindElement(By.CssSelector("#lastName")).SendKeys("Pavardenis");
    driver.FindElement(By.CssSelector("#userEmail")).SendKeys("vardenis@pavardenis.lt");
    driver.FindElement(By.CssSelector("#age")).SendKeys("30");
    driver.FindElement(By.CssSelector("#salary")).SendKeys("2000");
    driver.FindElement(By.CssSelector("#department")).SendKeys("H.R.");
    driver.FindElement(By.XPath("//button[text() = \"Submit\"]")).Click();
};

try {
    // Užduotis 1:
    // REIKIA NAUDOTI EXPLICIT WAIT

    // 1. Atsidaryti https://demoqa.com/
    driver.Url = ("https://demoqa.com/");

    // Dismiss cookie window
    driver.FindElement(By.CssSelector("button.fc-cta-consent")).Click();

    // 2. Pasirinkti "Widgets" kortelę
    driver.FindElement(By.XPath("//div[contains(@class, \"top-card\")]/node()[. = \"Elements\"]")).Click();

    // 3. Pasirinkti meniu punkta "Web Tables"
    var progressBar = driver.FindButton(withLabel: "Web Tables", withTagName: "span");
    driver.MoveTo(progressBar);
    progressBar.Click();

    // 4. Prideti pakankamai elementų, kad atsirastų antras puslapis puslapiavime

    Utils
        .While(() => driver.FindElement(By.CssSelector("span.-totalPages")).Text.Equals("1"))
        .Do(() => fillForm());

    // 5. Pasirinkti antra puslapi paspaudus "Next"
    var nextButton = driver.FindButton(withLabel: "Next");
    driver.MoveTo(nextButton);
    nextButton.Click();
        
    var beforeCountStr = driver.FindElement(By.CssSelector("span.-totalPages")).Text;
    var beforeCount = Int32.Parse(beforeCountStr);

    // 6. Ištrinti elementa antrajame puslapyje
    driver.FindElement(By.XPath("//span[@title = \"Delete\"]")).Click();
    
    var afterCountStr = driver.FindElement(By.CssSelector("span.-totalPages")).Text;
    var afterCount = Int32.Parse(afterCountStr);

    // 7. Įsitikinti, kad automatiškai puslapiavimas perkeliamas į pirmąjį puslapį ir kad puslapiu skaičius sumažėjo lygiai 1

    Debug.Assert(afterCount == 1);
    Debug.Assert( afterCount - beforeCount == 1);


} finally {
    Thread.Sleep(2000);
    if (Environment.GetEnvironmentVariable("DONT_QUIT") is null) { driver.Quit(); }
}

public static class MyExtentions {
    public static IWebElement FindButton(this IWebDriver driver, string withLabel, string withTagName = "button") =>
        driver.FindElement(By.XPath($"//{withTagName}[text() = \"{withLabel}\"]"));
    
    public static void MoveTo(this IWebDriver driver, IWebElement elem) {
        int deltaY_ = elem.Location.Y;
        new Actions(driver)
            .ScrollByAmount(0, deltaY_)
            .Perform();
    }
}

public static class Utils {

    public static WhileImpl While(Func<bool> pred) => new WhileImpl(pred);

    public record WhileImpl(Func<bool> pred) {
        public void Do(Action action) {
            while(pred()) {
                action();
            }
        }
    }
}