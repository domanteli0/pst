using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Reflection;
using System.Diagnostics;

var driver = new ChromeDriver();

try {
// TODO: Maximize window
driver.Url = "https://demowebshop.tricentis.com/";
driver.FindElement(By.XPath("/html/body/div[4]/div[1]/div[4]/div[1]/div[1]/div[2]/ul/li[7]/a")).Click();

var over99 = driver.FindElements(
    By.XPath("//div[@class=\"item-box\"][./descendant::span[@class=\"price actual-price\"] > 99]//div[@class=\"picture\"]")
);

over99.First().Click();

Thread.Sleep(1000);
var overview = driver.FindElement(By.XPath("//div[@class=\"overview\"]"));

overview.FindElement(By.XPath("//input[@class=\"recipient-name\"]")).SendKeys("Guillaume");
overview.FindElement(By.XPath("//input[@class=\"sender-name\"]")).SendKeys("Antoinne");

overview.FindElement(By.CssSelector("div.add-to-cart-panel > input")).Click();
overview.FindElement(By.CssSelector("div.add-to-cart-panel > input")).SendKeys(Keys.Backspace);
overview.FindElement(By.CssSelector("div.add-to-cart-panel > input")).SendKeys("5000");
driver.FindElement(By.CssSelector("#add-to-cart-button-4")).Click();

Thread.Sleep(200);
driver.FindElement(By.CssSelector("#add-to-wishlist-button-4")).Click();

Thread.Sleep(1000);
driver.FindElement(By.XPath("//a[@href=\"/jewelry\"]/..")).Click();

Thread.Sleep(1000);
driver.FindElement(By.XPath("//a[@href=\"/create-it-yourself-jewelry\"]")).Click();

Thread.Sleep(1000);
var select = driver.FindElement(
    By.CssSelector("#product_attribute_71_9_15")
);

var materialDropDown = new SelectElement(select);
materialDropDown.SelectByText("Silver", partialMatch: true);

driver.FindElement(By.CssSelector("#product_attribute_71_10_16")).SendKeys("80");
driver.FindElement(By.CssSelector("#product_attribute_71_11_17_50")).Click();

var overview_ = driver.FindElement(By.XPath("//div[@class=\"overview\"]"));
overview_.FindElement(By.CssSelector("div.add-to-cart-panel > input")).Click();
overview_.FindElement(By.CssSelector("div.add-to-cart-panel > input")).SendKeys(Keys.Backspace);
overview_.FindElement(By.CssSelector("div.add-to-cart-panel > input")).SendKeys("26");

Thread.Sleep(200);
driver.FindElement(By.CssSelector("#add-to-cart-button-71")).Click();

Thread.Sleep(200);
driver.FindElement(By.CssSelector("#add-to-wishlist-button-71")).Click();

Thread.Sleep(1000);
driver.FindElement(By.CssSelector("a[href=\"/wishlist\"]")).Click();

driver.FindElements(By.CssSelector("input[name=\"addtocart\"]")).ToList().ForEach(
    elem => {
        Thread.Sleep(500);
        elem.Click();
    }
);

driver.FindElement(By.CssSelector("body > div.master-wrapper-page > div.master-wrapper-content > div.master-wrapper-main > div > div > div.page-body > div.wishlist-content > form > div > div > input.button-2.wishlist-add-to-cart-button")).Click();

Debug.Assert(
 driver.FindElement(By.CssSelector("body > div.master-wrapper-page > div.master-wrapper-content > div.master-wrapper-main > div > div > div.page-body > div > form > div.cart-footer > div.totals > div.total-info > table > tbody > tr:nth-child(1) > td.cart-total-right > span > span")).Text == "1002600.00"
);

} finally {
    Thread.Sleep(2000);
    // driver.Quit();
}