# News Api

Web API written in .net technology and using MongoDb Atlas database to store data.
It allows users to register their own editorial office and manage its journalists. Journalists have access to create and
manage the editorial board's articles.
To use the API, a token is required, which you get only after logging in and previously after registration.

## Table of Contents

- [Live demo](#live-demo)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Known Issues](#known-issues)
- [License](#license)

## Live Demo

You can try out a live demo of the project at [news-app-live-demo](https://news-app-api.azurewebsites.net).

## Getting Started

To run the project you need .net 7, which can be downloaded from the following link

[Download .NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

The application sends an email to the user with a confirmation link, so before launching our application, we need to set
the following environment variables to be visible in the application:

`EMAIL_PASSWORD=[password_to_email]`          
`EMAIL_ACCOUNT=[address_email]`

In addition, configure the connection of the MongoDb database to the application in the appsettings.json file:

````
"NewsDatabase": {
    "ConnectionString": "[Database Connection String]",
...
}
````

## Usage

To run the project using .NET CLI use command you need to be in the API project directory and run the following command:

`dotnet run`

To run test set terminal in the Tests project directory and run the following command:

`dotnet test`

## Known Issues

- Better to change location for connection string to .net secret manager.
- Some tests are not working properly.
- Architecture of the project is not perfect. Better approach maybe be use CQRS pattern with MediatR and use onion
  architecture or layer architecture instead of monolit.

## License

This project is licensed under the [Non-Commercial License](https://opensource.org/licenses/NC-BY-4.0).
You are free to use, copy, modify, and distribute the software for non-commercial purposes only.
If you wish to use this software for commercial purposes, please contact the project owner for permission