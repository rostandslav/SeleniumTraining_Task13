using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;


namespace AddAndDeleteProductsTestProject
{
    [TestClass]
    public class UnitTest1
    {
        private IWebDriver driver;
        private WebDriverWait wait;


        [TestInitialize]
        public void Init()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }


        private bool CheckIfAtLeastOneElementExists(IWebDriver driver, By by)
        {
            return driver.FindElements(by).Count > 0;
        }


        [TestMethod]
        public void AddAndDeleteProductsTest()
        {
            // открываем главную страницу
            driver.Url = "http://litecart/";

            for (int prodCount = 0; prodCount < 3; prodCount++)
            {
                // выбираем первый товар в Most Popular
                driver.FindElement(By.CssSelector("#box-most-popular li a.link")).Click();

                By cartItemsCountBy = By.CssSelector("#header #cart .quantity");

                // количество товаров в корзине до добавления
                int beforeAddItemsCount = Convert.ToInt32(driver.FindElement(cartItemsCountBy).Text);
                
                // если есть обязательные селекты - выбираем первый элемент из списка
                var requiredSelects = driver.FindElements(By.CssSelector("form[name='buy_now_form'] select[required='required']"));
                for (int i = 0; i < requiredSelects.Count; i++)
                {
                    requiredSelects[i].SendKeys(Keys.ArrowDown);
                }

                // количество добавляемых товаров
                int addedItemsCount = Convert.ToInt32(driver.FindElement(By.CssSelector("form[name='buy_now_form'] input[name='quantity']")).GetAttribute("value"));
                
                // ожидаемое количество товаров в корзине после добавления
                int afterAddItemsCount = beforeAddItemsCount + addedItemsCount;

                // добавляем товар [Add to Cart]
                driver.FindElement(By.CssSelector("button[name='add_cart_product']")).Click();

                wait.Until(ExpectedConditions.TextToBePresentInElement(driver.FindElement(cartItemsCountBy), afterAddItemsCount.ToString()));

                driver.Navigate().Back();
            }


            driver.FindElement(By.CssSelector("a[href = 'http://litecart/en/checkout']")).Click();

            while (CheckIfAtLeastOneElementExists(driver, By.CssSelector("#box-checkout-cart")))
            {
                string cartFormSelector = "#box-checkout-cart form[name='cart_form']";
                driver.FindElement(By.CssSelector(cartFormSelector + " input[name='quantity']")).Click();
                string currentProduct = driver.FindElement(By.CssSelector(cartFormSelector + " div a strong")).Text;
                driver.FindElement(By.CssSelector(cartFormSelector + " button[name='remove_cart_item']")).Click();
                wait.Until(ExpectedConditions.StalenessOf(driver.FindElement(By.XPath("//*[@id='order_confirmation-wrapper']/table/tbody/tr/td[contains(text(), '" + currentProduct + "')]"))));
            }
        }


        [TestCleanup]
        public void Finish()
        {
            driver.Quit();
            //driver = null;
        }
    }
}
