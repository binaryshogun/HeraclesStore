# HeraclesStore
Cross-platform .NET microservices and container based web application that runs on Linux, Windows and macOS. This application is based on microservice architecture.

### Architecture overview
The Heracles Store application is cross-platform at the server and client side.
The architecture proposes a microservice oriented architecture implementation with multiple autonomous microservices 
(each one owning its own data/database) and implementing DDD/CQRS patterns approaches within each microservice using HTTP 
as the communication protocol between the client apps and the microservices and supports asynchronous communication for data 
updates propagation across multiple services based on Integration Events and an Event Bus (RabbitMQ).

![](img/solution_architecture.png)
