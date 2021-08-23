Cineworld has ten XML documents downloadable from: [www.cineworld.co.uk/syndication](https://www.cineworld.co.uk/syndication/)

* [all-performances.xml](https://www.cineworld.co.uk/syndication/all-performances.xml)
* [all-performances_ie.xml](https://www.cineworld.co.uk/syndication/all-performances_ie.xml)
* [cinemas.xml](https://www.cineworld.co.uk/syndication/cinemas.xml)
* [cinemas_ie.xml](https://www.cineworld.co.uk/syndication/cinemas_ie.xml)
* [film_times.xml](https://www.cineworld.co.uk/syndication/film_times.xml)
* [film_times_ie.xml](https://www.cineworld.co.uk/syndication/film_times_ie.xml)
* [listings.xml](https://www.cineworld.co.uk/syndication/listings.xml)
* [listings_ie.xml](https://www.cineworld.co.uk/syndication/listings_ie.xml)
* [weekly_film_times.xml](https://www.cineworld.co.uk/syndication/weekly_film_times.xml)
* [weekly_film_times_ie.xml](https://www.cineworld.co.uk/syndication/weekly_film_times_ie.xml)

I use `all-performances.xml` for cinemas and films (because it has film length), and `listings.xml` for shows (because of the ISO-8601 formatted times)

XML is downloaded with:
```powershell
curl --remote-name https://www.cineworld.co.uk/syndication/all-performances.xml
curl --remote-name https://www.cineworld.co.uk/syndication/listings.xml
```
They are then edited down to a more manageable size, then converted to XSD with: [www.freeformatter.com/xsd-generator.html](https://www.freeformatter.com/xsd-generator.html)

...and turned into C# with the official tool:
```powershell
& ${env:ProgramFiles(x86)}'\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\xsd.exe' .\all-performances.xsd /classes /fields /namespace:Helpers.Cineworld.Models.Generated.AllPerformances /out:.
& ${env:ProgramFiles(x86)}'\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\xsd.exe' .\listings.xsd /classes /fields /namespace:Helpers.Cineworld.Models.Generated.Listings /out:.
```
