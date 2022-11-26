# Wyszukiwanie książek przy pomocy tagów

## Cel projektu
Bardzo często zdarza się, że przeczytamy jakąś książkę, ale zapominamy jej tytuł. Pamiętamy jednak jej elementy takie jak kluczowe sceny lub charakterystyczne postacie. Nasza aplikacja oferuje możliwość dodawania nowych książek. Na ich bazie aplikacja wykorzystuje Cognitive Services do przeanalizowania okładek książek i wygenerowania odpowiednich tagów opisujących obrazki. Następnie te tagi będą wykorzystywane do wyszukiwania książek o pasujących tagach.  

## Opis projektu 
Projekt składa się z czterech głównych komponentów: frontend, backend, cognitive search oraz baza danych. Warstwa frontendowa została napisana przy pomocy bliblioteki react 17.0.1 w języku javascript. Aby upiększyć szatę graficzną użyliśmy obszernej biblioteki Material UI wraz z gotowymi komponentami i wieloma przydatnymi funkcjonalnościami. Do fetchowania danych zostało użyte narzęcie React Query, które pomaga obsługiwać request’y wysyłane do serwera. Warstwa backendowa została napisana w języku C# w wersji 10.0. Wystawia ona REST API z dwoma endpointami, które służą do pobierania i dodawania książek. Głównym zadaniem serwera jest połączenie z usługą Cognitive Search, która dostarcza nam samą funkcjonalność biznesową jaką jest analiza okładek książek. Do przechowywania danych używamy usługi SQL Azure. 

## Opis działania 
Pierwszą rzeczą jaką musimy zrobić wchodząc do aplikacji jest dodanie książek wraz z ich okładami. Po wykonaniu tej czynności możemy przystąpić do przeszukiwania książek za pomocą tagów. Jak tylko wpiszemy kluczowe słowo i klikniemy enter, to wyszuka nam wszystkie książki z danym tagiem. Gdy dodamy więcej niż jeden tag, to wyszuka nam wszystkie okładki, które zawierają przynajmniej jeden tag, który wpisaliśmy. 

## Opis serwisów

__SQL Azure__ – Wykorzystany do przechowywania szczegółów książek (okładka, tytuł, autor) oraz tagów (nazwa tagu). 

__Cognitive Services__ - Służy do analizy okładek książek i rozbijania ich na tagi określające co znajduje się na danej okładce. 

__Azure Functions__ – Wykorzystywany do obsługi żądań z frontendu oraz interakcji z bazą danych. 

## Authors

- [Fereniec Michał](https://github.com/Michal2390)
- [Kasprzak Arkadiusz](https://github.com/Kasprzak-Arkadiusz)
- [Milewski Adrian](https://github.com/milewsa3)


## Architektura
![architecture](/Readme/Architecture.png)

## Demo

