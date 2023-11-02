# LetsTalk Chat App @ back-end
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![MySQL](https://img.shields.io/badge/mysql-%2300f.svg?style=for-the-badge&logo=mysql&logoColor=white)
![Apache Kafka](https://img.shields.io/badge/Apache%20Kafka-000?style=for-the-badge&logo=apachekafka)
![Nginx](https://img.shields.io/badge/nginx-%23009639.svg?style=for-the-badge&logo=nginx&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![Debian](https://img.shields.io/badge/Debian-D70A53?style=for-the-badge&logo=debian&logoColor=white)

🔔 Please, also see the [front-end repository](https://github.com/evgenii-petukhov/LetsTalk.Angular.App) 🙏

![scheme](demo.gif)

🔴 [Live demo](https://chat.epetukhov.cyou/)
## Description
This is an instant messaging service with authentication via social media, such as Facebook. It allows users to send text messages, images, and share links. 

This project is a showcase of my technical skills and talent for potential IT recruiters, employers, customers, etc. It demonstrates that
* I can
  * create a single-page web application with Angular
  * implement reactive state management with NgRx store in Angular applications
* I understand
  * basic principles of the OpenAPI Specification
  * microservice communication patterns and protocols, such as GRPC
  * main principles of microservice architecture, event-driven development, and experienced in Apache Kafka
  * Domain-driven design data consistency aspects (please, read [my article about DDD](https://www.linkedin.com/pulse/how-i-practiced-ddd-principles-ignoring-them-evgenii-petukhov/) on LinkedIn)
## Architecture
![scheme](scheme-compressed.svg)

The front-end is an Angular single-page application which uses NgRx for reactive state management.

The back-end implements microservice architecture. The table below describes each of them.

| Microservice name | Protocol | Description |
| ----------- | ----------- | ----------- |
| Chat API | REST | Responsible for sending messages and account management |
| Authentication | [GRPC](https://github.com/grpc/grpc) | Generates and validates JSON Web Tokens |
| Notification | [Apache Kafka](https://github.com/apache/kafka) &#124; [SignalR](https://github.com/SignalR/SignalR) | Sends out notifications about new messages |
| Link preview | [Apache Kafka](https://github.com/apache/kafka) | Decorates messages with a website's name and a picture preview, if a message contains links |
| File storage | [GRPC Web](https://github.com/grpc/grpc-web) | Saves avatars and images uplodaed by users on the file system and serves them when requested |
| Image processing | [Apache Kafka](https://github.com/apache/kafka) | Generates image previews, uses [SkiaSharp](https://github.com/mono/SkiaSharp) |
## Deployment
The back-end can be deployed on Linux. For this you need to make the following steps
* Clone the repository
* [Install dotnet](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu)
* Install and [configure nginx](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-7.0&tabs=linux-ubuntu) as reverse proxy
* Set up SSL certificates. I prefer using Certbot + Let's Encrypt.
* Register microservices as Linux daemons