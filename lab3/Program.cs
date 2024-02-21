using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
// using static OpenQA.Selenium.Support.UI.ExpectedConditions;

using System.Diagnostics;
using OpenQA.Selenium.Interactions;
using System.Security.Principal;

// Užduotis 1:
// NOTE: REIKIA NAUDOTI EXPLICIT WAIT
//
// 1. Atsidaryti https://demoqa.com/
// 2. Pasirinkti "Widgets" kortelę
// 3. Pasirinkti meniu punkta "Progress Bar"
// 4. Spausti mygtuka "Start"
// 5. Sulaukti, kol bus 100% in paspausti "Reset"
// 6. Isitikinti, kad progreso eilutė tuscia (0%).

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

try {
    // Užduotis 1:
    // REIKIA NAUDOTI EXPLICIT WAIT

    // 1. Atsidaryti https://demoqa.com/
    driver.Url = ("https://demoqa.com/");

    // Dismiss cookie window
    driver.FindElement(By.CssSelector("button.fc-cta-consent")).Click();

    // 2. Pasirinkti "Widgets" kortelę
    driver.FindElement(By.XPath("//div[contains(@class, \"top-card\")]/node()[. = \"Widgets\"]")).Click();

    // 3. Pasirinkti meniu punkta "Progress Bar"
    var progressBar = driver.FindElement(By.XPath("//span[text() = \"Progress Bar\"]/.."));
    // new Actions(driver)
    //     .ScrollToElement(progressBar)
    //     .Perform();
    int deltaY = progressBar.Location.Y;
    new Actions(driver)
        .ScrollByAmount(0, deltaY)
        .Perform();
    progressBar.Click();

    // 4. Spausti mygtuka "Start"
    driver.FindElement(By.XPath("//button[text() = \"Start\"]")).Click();

    // 5. Sulaukti, kol bus 100% in paspausti "Reset"
    wait.Until(wd => 
        wd.FindElement(By.CssSelector("div[role=\"progressbar\"][aria-valuenow=\"100\"]"))
    );
    driver.FindElement(By.CssSelector("#resetButton")).Click();

    // 6. Isitikinti, kad progreso eilutė tuscia (0%).
    Debug.Assert(
        driver
            .FindElement(By.CssSelector("div[role=\"progressbar\"][aria-valuenow=\"0\"]"))
            .Text == "0%"
    );

} finally {
    Thread.Sleep(2000);
    if (Environment.GetEnvironmentVariable("DONT_QUIT") is null) { driver.Quit(); }
}

var uzduotis1 = () => {

};

public static class MyExtentions {
    public static IWebElement Button(this IWebDriver driver, string withLabel, string withType = "button") =>
        driver.FindElement(By.XPath($"//{withType}[text() = \"{withLabel}\"]/.."));
}