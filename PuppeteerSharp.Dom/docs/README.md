## About

PuppeteerSharp.Dom is an extension of [puppeteer-sharp by Darío Kondratiuk](https://github.com/hardkoded/puppeteer-sharp) that provides strongly typed DOM extensions/helpers.
Easily access the DOM using a strongly typed API.

## Key Features

✔️ PuppeteerSharp.Dom is a library with extension methods for strongly typed DOM access when using Puppeteer Sharp.
✔️ It provides a convenient way to write readable/robust/refactorable DOM access code.
✔️ Supports .NET Standard 2.0

## How to Use

```cs
// Add using PuppeteerSharp.Dom to access QuerySelectorAsync<T> and QuerySelectorAllAsync<T> extension methods.
// Get element by Id
// https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelector
var element = await page.QuerySelectorAsync<HtmlElement>("#myElementId");

//Set innerText property for the element
await element.SetInnerTextAsync("Welcome!");

//Get innerText property for the element
var innerText = await element.GetInnerTextAsync();

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
