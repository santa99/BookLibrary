

# O projekte:

Aplikaciu som vypracoval ako som najlepsie vedel.
- Sustredil som sa na oddelenie vrstiev avsak View(prezentacia) je sucastou API projektu. To by slo este rozdelit.
- Contracts obsahuju vsetky modely ktore pouziva Api aj DataAccess. Zaroven su tu obsiahnute dva DAO kontrakty(data access object) a dva kontrakty pre repozitare.
- Implementacia DAOs (BookLibraryDaoImpl, ReadersInfoDaoImpl) a Repositories (BookLibraryRepository, ReadersInfoRepository) je v DataAccess konkretne.
- Nastavenia pre DataAccess su v BookLibraryDataSourceConfig citane z appsettings.json a obsahuju cestu k XML suboru, cestu na XSD a flag ci validaovat vstupnu databazu na zaciatku.
- Nastavenia pre prihlasenie pouzivatela je v UserIdentityConfiguration citane z appsettings.json.
- LibraryController pouziva custom filter CustomAuthorizeFilter na autorizaciu (TODO: Prerobil by som na Authorize a schemu)
- Error handling pripraveny ako globalny ErrorHandlerMiddleware
- Ako request modely v  LibraryController som pouzil recordy + validacie + jednu custom validaciu na datum z buducnosti DateFromFutureValidationAttribute


## LibraryController 
- List knih : https://localhost:7227/
    - List podla typu:    
         Vsetky: https://localhost:7227/api/book/select/-1
		 Volne: https://localhost:7227/api/book/select/0
		 Pozicane: https://localhost:7227/api/book/select/1
- Odstranenie knihy: https://localhost:7227/api/book/remove/1
- Edit knihy : https://localhost:7227/api/book/edit/1?title=Svet%20je%20nahovno&author=M.B
- Pridanie knihy: https://localhost:7227/api/book/add?title=Harry%20Potter%20A%20Dari%20Smrti%20&author=Rowlingova
- Pozicanie knihy: https://localhost:7227/api/book/borrow/5/1
- Vratenie knihy: https://localhost:7227/api/book/return/5


- Login: - https://localhost:7227/account/login?returnUrl=%2F - ok
- Vsetky views - OK do istej miery iba happy paths a niekde aj zobrazenie chyb
- Testy urobit - zatial iba jeden test kvoli nedostatku casu.
    - konkurentne zapisovanie - trebalo by - chyba
	- zla databaza - trebalo by - chyba
	- zla cesta k databaze 
	

- Validacie vstupov -
   validácia povinných polí (názov knihy a autor) - OK 
   validácia na maximálnu dĺžku názvu knihy na 15 znakov. - OK 
   validácia dátumu výpožičky, vypožičaná kniha musí mať vyplnený dátum výpožičky + aby nebolo možné zadať dátum z budúcnosti - OK 
- validacia schemy pri prvom pouziti s flagom- OK


## todo:
- testy
- authorizacia lepsie zvladnuta cez Authorize
- Logovanie co s tym?
- strankovanie zapracovat (je to pripraven len sa to nevola)
