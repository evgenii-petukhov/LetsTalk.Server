# 💬LetsTalk Chat App💬 @ back-end
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
This project is a showcase of my technical skills and talent for potential IT recruiters, employers, customers, etc. It demonstrates that I am able to
* create a single-page web application with Angular
* implement reactive state management with NgRx store in Angular applications
* understand the basic principles of the OpenAPI Specification
* understand microservice communication patterns and protocols, such as GRPC
* use event streaming platforms and brokers, such as Apache Kafka
## Architecture
![scheme](scheme-compressed.svg)

The back-end implements microservice architecture. There are a few microservices, such as
* **Authentication microservice** generates and validates JWT tokens
* **Notification microservice** sends notifications to the Angular application via SignalR
* **Link preview microservice** processes links inside messages and generates a preview
* **File storage microservice** saves and retrieves files on / from the file system

The API and the Notification microservice communicate with the Authentication microservice via GRPC.

The API, the LinkPreview, and the Notification microservice communicate with each other via Apache Kafka.

## Deployment
This is an instant messaging service with authentication via social media, such as Facebook. The service allows users to send text messages and share links. 

The back-end can be deployed on Linux. For this you need to make the following steps
* Clone the repository
* [Install dotnet](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu)
* Install and [configure nginx](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-7.0&tabs=linux-ubuntu) as reverse proxy
* Set up SSL certificates. I prefer using Certbot + Let's Encrypt.
* Register microservices as Linux daemons