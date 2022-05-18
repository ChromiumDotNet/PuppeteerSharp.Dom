# PuppeteerSharp.Dom

[![Nuget](https://img.shields.io/nuget/v/PuppeteerSharp.Dom?style=for-the-badge)](https://www.nuget.org/packages/PuppeteerSharp.Dom/)
[![Nuget](https://img.shields.io/nuget/dt/PuppeteerSharp.Dom?style=for-the-badge)](https://www.nuget.org/packages/PuppeteerSharp.Dom/)
[![GitHub Sponsors](https://img.shields.io/github/sponsors/amaitland?style=for-the-badge)](https://github.com/sponsors/amaitland/)

PuppeteerSharp.Dom is an extension of [puppeteer-sharp by Darío Kondratiuk](https://github.com/hardkoded/puppeteer-sharp) that provides strongly typed DOM extensions/helpers.
- Built on top of Puppeteer Sharp

# Prerequisites

 * .Net Standard 2.0
 * [PuppeteerSharp](https://www.nuget.org/packages/PuppeteerSharp/) 7.0 or greater

# Questions and Support

* Ask a question on [Discussions](https://github.com/ChromiumDotNet/PuppeteerSharp.Dom/discussions).
* File bug reports on [Issues](https://github.com/ChromiumDotNet/PuppeteerSharp.Dom/issues).

Need priority support? [Sponsor @amaitland via GitHub Sponsors](https://github.com/sponsors/amaitland/) (Montly and One time sponsorships available).

# Usage

## DOM Access

Read/write to the DOM
<!-- snippet: QuerySelector -->
<a id='snippet-queryselector'></a>
```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.

// Add using PuppeteerSharp.Dom to access QuerySelectorAsync<T> and QuerySelectorAllAsync<T> extension methods.
// Get element by Id
// https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelector
var element = await Page.QuerySelectorAsync<HtmlElement>("#myElementId");

//Strongly typed element types
//Only a subset of element types have been added so far, use HtmlElement as a generic type for all others
var htmlDivElement = await Page.QuerySelectorAsync<HtmlDivElement>("#myDivElementId");
var htmlSpanElement = await Page.QuerySelectorAsync<HtmlSpanElement>("#mySpanElementId");
var htmlSelectElement = await Page.QuerySelectorAsync<HtmlSelectElement>("#mySelectElementId");
var htmlInputElement = await Page.QuerySelectorAsync<HtmlInputElement>("#myInputElementId");
var htmlFormElement = await Page.QuerySelectorAsync<HtmlFormElement>("#myFormElementId");
var htmlAnchorElement = await Page.QuerySelectorAsync<HtmlAnchorElement>("#myAnchorElementId");
var htmlImageElement = await Page.QuerySelectorAsync<HtmlImageElement>("#myImageElementId");
var htmlTextAreaElement = await Page.QuerySelectorAsync<HtmlImageElement>("#myTextAreaElementId");
var htmlButtonElement = await Page.QuerySelectorAsync<HtmlButtonElement>("#myButtonElementId");
var htmlParagraphElement = await Page.QuerySelectorAsync<HtmlParagraphElement>("#myParagraphElementId");
var htmlTableElement = await Page.QuerySelectorAsync<HtmlTableElement>("#myTableElementId");

// Get a custom attribute value
var customAttribute = await element.GetAttributeAsync<string>("data-customAttribute");

//Set innerText property for the element
await element.SetInnerTextAsync("Welcome!");

//Get innerText property for the element
var innerText = await element.GetInnerTextAsync();
//Can use this method to get any property that isn't currently mapped
innerText = await element.GetInnerTextAsync();

//Get all child elements
var childElements = await element.QuerySelectorAllAsync("div");

//Change CSS style background colour
await element.EvaluateFunctionAsync("e => e.style.backgroundColor = 'yellow'");

//Type text in an input field
await element.TypeAsync("Welcome to my Website!");

//Click The element
await element.ClickAsync();

// Simple way of chaining method calls together when you don't need a handle to the HtmlElement
var htmlButtonElementInnerText = await Page.QuerySelectorAsync<HtmlButtonElement>("#myButtonElementId")
    .AndThen(x => x.GetInnerTextAsync());

//Event Handler
//Expose a function to javascript, functions persist across navigations
//So only need to do this once
await Page.ExposeFunctionAsync("jsAlertButtonClick", () =>
{
    _ = Page.EvaluateExpressionAsync("window.alert('Hello! You invoked window.alert()');");
});

var jsAlertButton = await Page.QuerySelectorAsync<HtmlButtonElement>("#jsAlertButton");

//Write up the click event listner to call our exposed function
_ = jsAlertButton.AddEventListenerAsync("click", "jsAlertButtonClick");

//Get a collection of HtmlElements
var divElements = await Page.QuerySelectorAllAsync<HtmlDivElement>("div");

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
        var newDiv = await Page.CreateHtmlElementAsync<HtmlDivElement>("div");
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
        var newDiv = await Page.CreateHtmlElementAsync<HtmlDivElement>("div");
        await newDiv.SetInnerTextAsync("New Div Added!");
        await cell.AppendChildAsync(newDiv);
    }
}
```
<sup><a href='/PuppeteerSharp.Dom.Tests/QuerySelectorTests/PageContextQuerySelectorTests.cs#L19-L130' title='Snippet source file'>snippet source</a> | <a href='#snippet-queryselector' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
