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
- The architecture of the project is not perfect. A better approach might be to use the CQRS pattern with Mediator and use an onion or layered architecture instead of a monolithic one.

## License

This project is licensed under the [Non-Commercial License](https://opensource.org/licenses/NC-BY-4.0).
You are free to use, copy, modify, and distribute the software for non-commercial purposes only.
If you wish to use this software for commercial purposes, please contact the project owner for permission

# 💫 About Me:
I'm a .net developer who mainly works on the backend, but likes to get dirty in the frontend sometimes 🙃.<br>I write mostly in C#, but the C family of languages is like an unwanted sibling to me 🧒🏻, I'll call on them as needed.<br><br>I also play around in docker and use 🐍 language (python) as the opportunity arises. <br><br>I learn and teach what I can 🎓.


# 💻 Tech Stack:
![C](https://img.shields.io/badge/c-%2300599C.svg?style=flat&logo=c&logoColor=white) ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=flat&logo=c-sharp&logoColor=white) ![C++](https://img.shields.io/badge/c++-%2300599C.svg?style=flat&logo=c%2B%2B&logoColor=white) ![HTML5](https://img.shields.io/badge/html5-%23E34F26.svg?style=flat&logo=html5&logoColor=white) ![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=flat&logo=javascript&logoColor=%23F7DF1E) ![Python](https://img.shields.io/badge/python-3670A0?style=flat&logo=python&logoColor=ffdd54) ![TypeScript](https://img.shields.io/badge/typescript-%23007ACC.svg?style=flat&logo=typescript&logoColor=white) ![Azure](https://img.shields.io/badge/azure-%230072C6.svg?style=flat&logo=azure-devops&logoColor=white) ![Heroku](https://img.shields.io/badge/heroku-%23430098.svg?style=flat&logo=heroku&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=flat&logo=.net&logoColor=white) ![Angular](https://img.shields.io/badge/angular-%23DD0031.svg?style=flat&logo=angular&logoColor=white) ![Bootstrap](https://img.shields.io/badge/bootstrap-%23563D7C.svg?style=flat&logo=bootstrap&logoColor=white) ![Express.js](https://img.shields.io/badge/express.js-%23404d59.svg?style=flat&logo=express&logoColor=%2361DAFB) ![JWT](https://img.shields.io/badge/JWT-black?style=flat&logo=JSON%20web%20tokens) ![NPM](https://img.shields.io/badge/NPM-%23000000.svg?style=flat&logo=npm&logoColor=white) ![UNITY](https://img.shields.io/badge/Unity-%2320232a.svg?style=flat&logo=unity&logoColor=white) ![MongoDB](https://img.shields.io/badge/MongoDB-%234ea94b.svg?style=flat&logo=mongodb&logoColor=white) ![MySQL](https://img.shields.io/badge/mysql-%2300f.svg?style=flat&logo=mysql&logoColor=white) ![Canva](https://img.shields.io/badge/Canva-%2300C4CC.svg?style=flat&logo=Canva&logoColor=white) ![LINUX](https://img.shields.io/badge/Linux-FCC624?style=flat&logo=linux&logoColor=black) ![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=flat&logo=docker&logoColor=white) ![Jira](https://img.shields.io/badge/jira-%230A0FFF.svg?style=flat&logo=jira&logoColor=white) ![Postman](https://img.shields.io/badge/Postman-FF6C37?style=flat&logo=postman&logoColor=white)
# 📊 GitHub Stats:
![](https://github-readme-stats.vercel.app/api?username=Namlesz&theme=gotham&hide_border=true&include_all_commits=true&count_private=true)<br/>
![](https://github-readme-streak-stats.herokuapp.com/?user=Namlesz&theme=gotham&hide_border=true)<br/>
![](https://github-readme-stats.vercel.app/api/top-langs/?username=Namlesz&theme=gotham&hide_border=true&include_all_commits=true&count_private=true&layout=compact)

### ✍️ Random Dev Quote
![](https://quotes-github-readme.vercel.app/api?type=horizontal&theme=radical)

### 😂 Random Dev Meme
<img src="https://rm.up.railway.app/" width="512px"/>

---
[![](https://visitcount.itsvg.in/api?id=Namlesz&icon=5&color=6)](https://visitcount.itsvg.in)

<!-- Proudly created with GPRM ( https://gprm.itsvg.in ) -->
