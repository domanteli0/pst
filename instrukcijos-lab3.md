(1) Demo:

2. Chrome settings -> Search 'Proxy' -> Http://localhost 8888. Rinkčiaus Firefox nes individualiai proxy galima nustatyti,
   bet Demoblaze saitas turi popupų, kuriuos naršyklė (neteisingai?) interpretuoja kaip Cross-Origin Request Blocked

3. right click Test Plan -> add Non-Test Elements -> HTTP(S) Test Script Recorder

4. right click Test Plan -> add Thread Group

5. right click Thread Group -> Logic Controllers -> Recording Controllers

6. HTTP(S) Test Script Recorder -> Start

7. Atsidaryti Chrome, atsidaryti https://www.demoblaze.com/, Stop recorder. Pirmu recorderio paleidimu JMeter sugeneruoja sertifikatą.

8. Chrome settings -> Search 'Certificates' -> Import -> apache-jmeter/bin/ApacheJMeterTemporaryRootCA.crt -> restart browser.
   Jei neveikia, rasti sertifikatą bin folderyje ir double click ir praeiti wizardą.

9. Parodyti, kad Recorderis pirmu paleidimu įrašė daug requestų.

10. Pridėti Listenerį ir pademonstruoti, kad paleidus skriptą įrašytieji requestai veikia.

11. Ištrinti senus įrašus ir startuoti recorderį iš naujo https://www.demoblaze.com/ -> Sign up (turi buti unikalus) ->
    Log in -> Add item -> Cart -> Pay. Prieš atliekant kiekvieną veiksmą:

- Thread Group -> Add Logic Controler -> Recording Controllers,
- HTTP(S) Test Script Recorder -> Target Controller - pridėti sukurtą controlerį.

12. Paleisti skriptą ir parodyti View Results Tree, kad neveikia kai kurie requestai, reikia sutvarkyti Sign up unikalumą ir perduoti jį į Log in

- Parodyti Sign Up/signup requestą, kuris siunčia {"username":"VUPTuser6689","password":"ttttaaaa8746"}, jo response bus {"errorMessage":"This user already exist."}. Paakcentuoti, kad response code 200, nors skriptas veikia ne pagal scenarijų, todėl reikalingi assertionai
- Parodyti Log in/login requestą, kuris siunčia {"username":"VUPTuser6689","password":"ttttaaaa8746"}, jo response bus "Auth_token: VlVQVHVzZXI0MzIxMDAxNjYyMTMw", token generuojamas kiekviena kartą, bet mums reikia pataisyti skriptą, kad token būtų paveldimas tarp requestų (turime atlikti koreliaciją)
- Paaiškinti, kad mes čia ieškosim adatos šieno kupetoje nes yra daug elementų kuriuos įrašė skriptas ir kai kurie elementai nėra mums aktualūs.
- Pirmiausiai, skriptas įrašo visiškai su skriptu nesusijusį srautą, pvz naršyklė parsisiunčia atnaujinimus.
- Tuo pačiu parsiunčiami resursai, kaip .img, kuriuos naršyklė cache'ina, ar norim tokius resursus palikti
  priklauso nuo testo tikslo ir dizaino, ar norim, kad būtų imituojamas naujas vartotojas, kuris pirmą kartą prisijungia ir kuriam yra siunčiami visi resursai.
