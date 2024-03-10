using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

using System.Diagnostics;
using OpenQA.Selenium.Interactions;

public class Part3 : IDisposable
{
    IWebDriver driver;
    public Part3() => driver = Util.up();
    void IDisposable.Dispose() => driver.Down();

    // Užduotis 1:
    // NOTE: REIKIA NAUDOTI EXPLICIT WAIT
    //
    // 1. Atsidaryti https://demoqa.com/
    // 2. Pasirinkti "Widgets" kortelę
    // 3. Pasirinkti meniu punkta "Progress Bar"
    // 4. Spausti mygtuka "Start"
    // 5. Sulaukti, kol bus 100% in paspausti "Reset"
    // 6. Isitikinti, kad progreso eilutė tuscia (0%).
    [Fact]
    public void Test1()
    {

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

        wait.PollingInterval = TimeSpan.FromMilliseconds(2000);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

        // 1. Atsidaryti https://demoqa.com/
        driver.Url = ("https://demoqa.com/");

        // Dismiss cookie window, if present
        driver.FindElements(By.CssSelector("button.fc-cta-consent")).Take(1).ToList().ForEach((consent) => consent.Click());

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
        Assert.Equal(
            expected: "0%",
            actual: driver
                .FindElement(By.CssSelector("div[role=\"progressbar\"][aria-valuenow=\"0\"]"))
                .Text
        );
    }

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
    [Fact]
    public void Test2()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

        wait.PollingInterval = TimeSpan.FromMilliseconds(2000);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

        var fillForm = () =>
        {
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

        // Užduotis 1:
        // REIKIA NAUDOTI EXPLICIT WAIT

        // 1. Atsidaryti https://demoqa.com/
        driver.Url = ("https://demoqa.com/");

        // Dismiss cookie window, if present
        driver.FindElements(By.CssSelector("button.fc-cta-consent")).Take(1).ToList().ForEach((consent) => consent.Click());

        // 2. Pasirinkti "Widgets" kortelę
        var elem = driver.FindElement(By.XPath("//div[contains(@class, \"top-card\")]/node()[. = \"Elements\"]"));
        driver.MoveTo(elem);
        elem.Click();

        // 3. Pasirinkti meniu punkta "Web Tables"
        var progressBar = driver.FindButton(withLabel: "Web Tables", withTagName: "span");
        driver.MoveTo(progressBar);
        progressBar.Click();

        // 4. Prideti pakankamai elementų, kad atsirastų antras puslapis puslapiavime

        Util.While(() => driver.FindElement(By.CssSelector("span.-totalPages")).Text.Equals("1"))
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

        Assert.Equal(expected: 1, actual: afterCount);
        Assert.Equal(expected: 1, actual: afterCount - beforeCount);

    }
}