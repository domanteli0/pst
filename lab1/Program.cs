using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

var driver = new ChromeDriver();

try {
// TODO: Maximize window

// 1. Atsidaryti tinklalapį https://demowebshop.tricentis.com/
driver.Url = "https://demowebshop.tricentis.com/";

// 2. Spausti 'Gift Cards' kairiajame meniu.
driver.FindElement(By.XPath("//a[@href=\"/gift-cards\"]/..")).Click();

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

// driver.FindElement(By.CssSelector("#product_attribute_71_10_16")).SendKeys("80");
// driver.FindElement(By.CssSelector("#product_attribute_71_11_17_50")).Click();

// var overview_ = driver.FindElement(By.XPath("//div[@class=\"overview\"]"));
// overview_.FindElement(By.CssSelector("div.add-to-cart-panel > input")).Click();
// overview_.FindElement(By.CssSelector("div.add-to-cart-panel > input")).SendKeys(Keys.Backspace);
// overview_.FindElement(By.CssSelector("div.add-to-cart-panel > input")).SendKeys("26");

// Thread.Sleep(200);
// driver.FindElement(By.CssSelector("#add-to-cart-button-71")).Click();

// Thread.Sleep(200);
// driver.FindElement(By.CssSelector("#add-to-wishlist-button-71")).Click();

// Thread.Sleep(1000);
// driver.FindElement(By.CssSelector("a[href=\"/wishlist\"]")).Click();

// driver.FindElements(By.Name("addtocart")).ToList().ForEach(
//     elem => {
//         Thread.Sleep(500);
//         elem.Click();
//     }
// );

// driver.FindElement(By.XPath("//input[@value = 'Add to cart']")).Click();

// Debug.Assert(
//     driver.FindElement(By.CssSelector("span.order-total > strong")).Text == "1002600.00"
// );

} finally {
    Thread.Sleep(2000);
    // driver.Quit();
}