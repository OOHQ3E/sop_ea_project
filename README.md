## HU: Szolgáltatás Orientált Programozás előadás beadandó (NodeJS + PHP RestAPI + WPF kliens + OpenAPI) - EKKE IK PTI Bsc. - NK-OOHQ3E Bagoly Gábor - 2021/22 őszi félév
### Kis setup segítség:

  - [NodeJS-t telepíteni a hivatalos oldalról](https://nodejs.org/en/download/)
  - [XAMPP-ot telepíteni a hivatalos oldalról](https://www.apachefriends.org/index.html)
  - restAPI mappát a /xampp/htdocs-on belülre kell helyezni
  - MySQL-be beimportálni az adatbázist:
      - A reservations.sql fájlt bele kell helyezni a xampp mappájába
      - MySQL-ben létre kell hozni az adatbázist: ```create database reservations;```   
      - MySQL-ben az alábbi kód az importáláshoz: ```mysql -u root -p -h localhost reservations<reservations.sql```
  - Mielőtt futtatjuk a programot:
      - XAMPP-on az Apache-ot és a MySQL-t el kell indítani
      - restAPI mappát megnyitni powershell-ben és beírni ezt az utasítást ```node ./restapi.js```

## ENG: Service Oriented Preogramming lecture project (NodeJS + PHP RestAPI + WPF client + OpenAPI) - EKCU IF CS Bsc. - NK-OOHQ3E Gábor Bagoly - 2021/22 autumn semester
### Little setup help:

  - [Installing NodeJS from the offical site](https://nodejs.org/en/download/)
  - [Installing XAMPP from the offical site](https://www.apachefriends.org/index.html)
  - The folder "restAPI" has to be moved in /xampp/htdocs/
  - Importing the database in MySQL:
      - The file "reservations.sql" has to be moved in /xampp/
      - Creating the needed database in MySQL: ```create database reservations;```   
      - Importing the data in the database: ```mysql -u root -p -h localhost reservations<reservations.sql```
  - Before starting the program:
      - You have to start Apache and MySQL in XAMPP
      - Open the restAPI folder in powershell and enter the command ```node ./restapi.js```
