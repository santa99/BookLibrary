
## Zmeny ku dnu 28.5.2024
- Pribudla View aplikacia vytvorena v Blazor. 
- Tato aplikacia ponuka zakladne CRUD operacia s knihami ADD/REMOVE/EDIT
- Rozsirenie o poziciavanie knih na samostatnej karte Borrow / Return + Filtracia podla typu (Free, Borrowed, All)
- Vytvorene strankovanie kvoli obmedzeniu viditelnej casti obsahu.
poziciavanie a vratenie knihy.
- Pribudol CHANGELOG.md
- Pribudol Swagger s vyplnenou dokumentaciu
- Pribudla authentifikacia s Cookies schemou.
- Pribudli Logger (Serilog) ktory loguju vsetko + specificky logger na komunikaciu smerom do sluzby do suboru C:/Logs/Http/intraffic.txt
- Opravene status kody z REST APIcka

# O projekte:

Aplikaciu som vypracoval pri plnom vedomi a svedomi.
- Sustredil som sa na oddelenie vrstiev avsak View(prezentacia) je sucastou API projektu. To by slo este rozdelit.
- Contracts obsahuju vsetky modely ktore pouziva Api aj DataAccess. Zaroven su tu obsiahnute dva DAO kontrakty(data access object) a dva kontrakty pre repozitare.
- Implementacia DAOs (`BookLibraryDaoImpl`, `ReadersInfoDaoImpl`) a Repositories (`BookLibraryRepository`, `ReadersInfoRepository`) je v DataAccess konkretne.
- Nastavenia pre DataAccess su v BookLibraryDataSourceConfig citane z appsettings.json a obsahuju cestu k XML suboru, cestu na XSD a flag ci validaovat vstupnu databazu na zaciatku.
- Nastavenia pre prihlasenie pouzivatela je v UserIdentityConfiguration citane z appsettings.json.
- LibraryController pouziva custom filter CustomAuthorizeFilter na autorizaciu (TODO: Prerobil by som na Authorize a schemu)
- Error handling pripraveny ako globalny ErrorHandlerMiddleware. Vsetky chyby sa mapuju na `ErrorCodeModel` smerom von.
- Ako request modely v  `LibraryController` som pouzil recordy + validacie + jednu custom validaciu na datum z buducnosti DateFromFutureValidationAttribute
- Vytvoril som `IBorrowBookCommand` ktory evokuje ze sa jedna o zapisy do DB. Na opacnej strane chybaju Query pre ostatne retrieve operacie z DB (priame volanie v controlleroch).

## Controllers

### LibraryController 

Vytvorene APIcko pre pristup ku kniznici. V skutocnosti Som pripravil vsak `HomeController` ako REST API ktore vola pomocou HTTP callov LibraryController endpointy.

- List knih : https://localhost:7227/
    - List podla typu:    
      - Vsetky: https://localhost:7227/api/book/select/-1
      - Volne: https://localhost:7227/api/book/select/0
      - Pozicane: https://localhost:7227/api/book/select/1
- Odstranenie knihy: https://localhost:7227/api/book/remove/1
- Edit knihy : https://localhost:7227/api/book/edit/1?title=Svet%20je%20nahovno&author=M.B
- Pridanie knihy: https://localhost:7227/api/book/add?title=Harry%20Potter%20A%20Dari%20Smrti%20&author=Rowlingova
- Pozicanie knihy: https://localhost:7227/api/book/borrow/5/1
- Vratenie knihy: https://localhost:7227/api/book/return/5

### AccountController
- Login: - https://localhost:7227/account/login?returnUrl=%2F
- Nastavenia pre prihlasenie pouzivatela je v UserIdentityConfiguration citane z appsettings.json.
- Zapis do cookies a oveenie cez `CustomAuthorizeFilter` pri operaciach nad `LibraryController`

### ReadersController
- Doplneny mnou ako dummy DB citatelov s menom priezviskom a idckom charakterizujucim identifikator citatela.
- Pouzity pri poziciavani knihy.

## Views
- Vsetky views implementovane do istej miery a niekde aj zobrazenie chyb. Prezentacia by sa dala urobit milion krat lepsie.
- Implementacia pomocou Razor pages + vyhrabane starsie css styly.

## Tests
- Testy urobit - pre nazoronost urobene testy na pokrytie BookLibraryRepositoryTests.
- Testy ktore by som doplnil
    - API integracne testy aby sme overili funkcionalitu az po uroven DB
    - validacie
    - konkurentne zapisovanie - trebalo by - chyba
	- zla databaza - trebalo by - chyba
	- zla cesta k databaze 
	
## Validacie

- Validacie vstupov -
   - validácia povinných polí (názov knihy a autor) - OK 
   - validácia na maximálnu dĺžku názvu knihy na 15 znakov. - OK 
   - validácia dátumu výpožičky, vypožičaná kniha musí mať vyplnený dátum výpožičky + aby nebolo možné zadať dátum z budúcnosti - OK 
- validacia schemy pri prvom pouziti s flagom- OK


## ToDo:
- viac testov
- authorizacia lepsie zvladnuta cez Authorize a claimy
- Logovanie => asi by som pridal serilog ale musel by som pozriet mapovanie pre aplikaciu. Logovanie je vsak v ErrorHandlerMiddleware.
- strankovanie zapracovat (je to pripraven len sa to nevola iba v testoch)
- uvazovat o business logike 
