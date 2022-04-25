# Matches-REST-API-With-Efficient-CsvParser

## Description
This project was made with C#, .NET, and PostgreSQL for the database. It consists of a CSV parser to parse football matches data from the CSV files, and a REST API. The parser is able to read tens of thousands of rows, and write them to the DB in a couple of seconds. The API uses JWT for authentication and authorization for the two different roles of users: Customer and Developer. The token for each role can be retrieved by making a `GET` request to the respective route (explained below). The secret and token are hardcoded for the purposes of the specific project since there is no support for actually having users, so every time a request is made to get a token, either for a developer or a customer, the token will always be the same. 

A developer token will grant access to make calls to all the routes of the API, while a customer token will only grant access to the customer routes.

There is also a caching layer, in order to cache some modifiers that come from an external API. The purpose of the modifiers is to signify which football divisions are active, in order to filter the responses from the API and show only the football matches that belong to the active divisions. 

Unit testing was performed with Xunit and Moq.

## Installation

1. `git clone git@github.com:gtamvakas/Matches-REST-API-With-Efficient-CsvParser.git`

2. `cd Matches-REST-API-With-Efficient-CsvParser && cd src` then run the `script.bat` if on Windows or `sudo ./script.sh` if on Linux, in order to setup docker. Be sure to make the script executable first, if on Linux, by running `sudo chmod +x script.sh`

3. Once the script has successfully finished, `cd matchesapi` and run the application using `dotnet run`.

4. Visit `localhost:<port>/swagger` to view the documentation.

5. First make a `GET` request to `/dev/token` to get a developer token so you can access all the routes freely. Add the token to swagger/postman/curl.

6. Make a `GET` request to `/dev/parse` before anything else, so all the data from the CSV files are parsed and the DB is populated.

## License
[MIT](https://choosealicense.com/licenses/mit/)
