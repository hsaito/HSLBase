# HSLBase

Hideki's Songlist Data Handler

## Purpose

This is the new upcoming codebase for [Hideki's Songlist](http://songlist.hclippr.com/). 

## Using HSLProcessr

The HSLProcessor is a tool to process song list data.

Note: If you are using a binary distribution, use, `dotnet` followed by DLL name, instead of `dotnet run`. For example, use `dotnet HSLProcessor.dll list` in place of `dotnet run list`.

### Importing Data
Create a CSV file, like this:

<pre>
Title,Artist,Source
My Song,My Artist,Source
</pre>

Save this file, and use `importcsv` command to import.

<pre>
dotnet run importcsv inputfile.csv
</pre>

The first line of the file is ignored by the parser.

### Listing the entries
Once you have imported the data, you can confirm the contents of the data base using `list` command. Use the command as follows.

<pre>
dotnet run list
</pre>

### Search the entries
It is now possible to search contents of the database, using `search` command.

<pre>
dotnet run search text
</pre>

### Delete the entry
You can remove entry by passing GUID of item into `deleteitem` command.

<pre>
dotnet run deleteitem 09f8312f-51e1-4425-b739-4374b2d656d5
</pre>

GUID for item can be obtained through `list` or `search` command.

### XML export
You can export contents of database into `exportxml` command. Exported file can be imported using `importxml` command. XML file is useful as it contains the whole data structure including internal identifier, which can be imported without loss of data.

<pre>
dotnet run exportxml export.xml
dotnet run importxml import.xml
</pre>

### Generating static pages
You can generate static web site contains whole data structure of the site. Each page is interactive; you will be able to click on title, artist, source information to jump to the specific page.

You can use `generatehtml` command to do this.

<pre>
dotnet run generatehtml output
</pre>

Argument passed `output` portion to specify where you want output your page to.