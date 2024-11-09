# PuppeteerSharp.Dom

[![Nuget](https://img.shields.io/nuget/v/PuppeteerSharp.Dom?style=for-the-badge)](https://www.nuget.org/packages/PuppeteerSharp.Dom/)
[![Nuget](https://img.shields.io/nuget/dt/PuppeteerSharp.Dom?style=for-the-badge)](https://www.nuget.org/packages/PuppeteerSharp.Dom/)
[![GitHub Sponsors](https://img.shields.io/github/sponsors/amaitland?style=for-the-badge)](https://github.com/sponsors/amaitland/)

PuppeteerSharp.Dom is an extension of [puppeteer-sharp by Darío Kondratiuk](https://github.com/hardkoded/puppeteer-sharp) that provides strongly typed DOM extensions/helpers.
- Built on top of Puppeteer Sharp

# Prerequisites

 * .Net Standard 2.0
 * [PuppeteerSharp](https://www.nuget.org/packages/PuppeteerSharp/) 19.0 or greater

# Questions and Support

* Ask a question on [Discussions](https://github.com/ChromiumDotNet/PuppeteerSharp.Dom/discussions).
* File bug reports on [Issues](https://github.com/ChromiumDotNet/PuppeteerSharp.Dom/issues).

Need priority support? [Sponsor @amaitland via GitHub Sponsors](https://github.com/sponsors/amaitland/) (Montly and One time sponsorships available).

# Usage

## DOM Access

Read/write to the DOM
<!-- snippet: QuerySelector -->
<a id='snippet-QuerySelector'></a>
```cs
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.

// Add using PuppeteerSharp.Dom to access QuerySelectorAsync<T> and QuerySelectorAllAsync<T> extension methods.
// Get element by Id
// https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelector
var element = await page.QuerySelectorAsync<HtmlElement>("#myElementId");

//Strongly typed element types (this is only a subset of the types mapped)
var htmlDivElement = await page.QuerySelectorAsync<HtmlDivElement>("#myDivElementId");
var htmlSpanElement = await page.QuerySelectorAsync<HtmlSpanElement>("#mySpanElementId");
var htmlSelectElement = await page.QuerySelectorAsync<HtmlSelectElement>("#mySelectElementId");
var htmlInputElement = await page.QuerySelectorAsync<HtmlInputElement>("#myInputElementId");
var htmlFormElement = await page.QuerySelectorAsync<HtmlFormElement>("#myFormElementId");
var htmlAnchorElement = await page.QuerySelectorAsync<HtmlAnchorElement>("#myAnchorElementId");
var htmlImageElement = await page.QuerySelectorAsync<HtmlImageElement>("#myImageElementId");
var htmlTextAreaElement = await page.QuerySelectorAsync<HtmlImageElement>("#myTextAreaElementId");
var htmlButtonElement = await page.QuerySelectorAsync<HtmlButtonElement>("#myButtonElementId");
var htmlParagraphElement = await page.QuerySelectorAsync<HtmlParagraphElement>("#myParagraphElementId");
var htmlTableElement = await page.QuerySelectorAsync<HtmlTableElement>("#myTableElementId");

// Get a custom attribute value
var customAttribute = await element.GetAttributeAsync("data-customAttribute");

//Set innerText property for the element
await element.SetInnerTextAsync("Welcome!");

//Get innerText property for the element
var innerText = await element.GetInnerTextAsync();

//Get all child elements
var childElements = await element.QuerySelectorAllAsync("div");

//Change CSS style background colour
await element.EvaluateFunctionAsync("e => e.style.backgroundColor = 'yellow'");

//Type text in an input field
await element.TypeAsync("Welcome to my Website!");

//Click The element
await element.ClickAsync();

// Simple way of chaining method calls together when you don't need a handle to the HtmlElement
var htmlButtonElementInnerText = await page.QuerySelectorAsync<HtmlButtonElement>("#myButtonElementId")
    .AndThen(x => x.GetInnerTextAsync());

//Event Handler
//Expose a function to javascript, functions persist across navigations
//So only need to do this once
await page.ExposeFunctionAsync("jsAlertButtonClick", () =>
{
    _ = page.EvaluateExpressionAsync("window.alert('Hello! You invoked window.alert()');");
});

var jsAlertButton = await page.QuerySelectorAsync<HtmlButtonElement>("#jsAlertButton");

//Write up the click event listner to call our exposed function
_ = jsAlertButton.AddEventListenerAsync("click", "jsAlertButtonClick");

//Get a collection of HtmlElements
var divElements = await page.QuerySelectorAllAsync<HtmlDivElement>("div");

foreach (var div in divElements)
{
    // Get a reference to the CSSStyleDeclaration
    var style = await div.GetStyleAsync();

    //Set the border to 1px solid red
    await style.SetPropertyAsync("border", "1px solid red", important: true);

    await div.SetAttributeAsync("data-customAttribute", "123");
    await div.SetInnerTextAsync("Updated Div innerText");
}

//Using standard array
var tableRows = await htmlTableElement.GetRowsAsync().ToArrayAsync();

foreach (var row in tableRows)
{
    var cells = await row.GetCellsAsync().ToArrayAsync();
    foreach (var cell in cells)
    {
        var newDiv = await page.CreateHtmlElementAsync<HtmlDivElement>("div");
        await newDiv.SetInnerTextAsync("New Div Added!");
        await cell.AppendChildAsync(newDiv);
    }
}

//Get a reference to the HtmlCollection and use async enumerable
//Requires Net Core 3.1 or higher
var tableRowsHtmlCollection = await htmlTableElement.GetRowsAsync();

await foreach (var row in tableRowsHtmlCollection)
{
    var cells = await row.GetCellsAsync();
    await foreach (var cell in cells)
    {
        var newDiv = await page.CreateHtmlElementAsync<HtmlDivElement>("div");
        await newDiv.SetInnerTextAsync("New Div Added!");
        await cell.AppendChildAsync(newDiv);
    }
}
```
<sup><a href='/PuppeteerSharp.Dom.Tests/QuerySelectorTests/PageContextQuerySelectorTests.cs#L19-L127' title='Snippet source file'>snippet source</a> | <a href='#snippet-QuerySelector' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
