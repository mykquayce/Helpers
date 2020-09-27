- downloaded xml from ```https://old.reddit.com/r/worldnews/.rss```
- converted to xsd using ```https://www.freeformatter.com/xsd-generator.html``` and Salami Slice.
- edit out the fields we don't care about
- convert to .cs with:

```powershell
& 'C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\xsd.exe' .\worldnews.xsd  /classes /fields /namespace:Helpers.Reddit.Models /out:.
```
