# LetsTalk back-end
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![MySQL](https://img.shields.io/badge/mysql-%2300f.svg?style=for-the-badge&logo=mysql&logoColor=white)
![Apache Kafka](https://img.shields.io/badge/Apache%20Kafka-000?style=for-the-badge&logo=apachekafka)
![Nginx](https://img.shields.io/badge/nginx-%23009639.svg?style=for-the-badge&logo=nginx&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![Debian](https://img.shields.io/badge/Debian-D70A53?style=for-the-badge&logo=debian&logoColor=white)

![scheme](demo.gif)

[Live demo](https://chat.epetukhov.cyou/)
## Description
The idea behind this project is to demonstrate that 
* I'm be able to create modern ASP.NET Core applications
* I understand the main principles of microservice architecture and event-driven development
* I know basic approaches of work with Apache Kafka
* I can deploy .NET applications on Linux
## Architecture
![scheme](scheme-compressed.svg)

The back-end implements microservice architecture. There are a few microservices, such as
* **Authentication microservice** generates and validates JWT tokens
* **Notification microservice** sends notifications to the Angular application via SignalR
* **Link preview microservice** processes links inside messages and generates a preview

The API and the Notification microservice communicate with the Authentication microservice via GRPC.

The API, the LinkPreview, and the Notification microservice communicate with each other via Apache Kafka.