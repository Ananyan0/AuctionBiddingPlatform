# 🏦 Auction Bidding Platform — Backend Microservices Solution

This project implements the backend of an **online auction system** that allows users to create auction items, place competitive bids, and automatically determine winners when auctions close. The solution follows **Clean/Onion Architecture**, uses **Entity Framework Core**, **JWT authentication**, and is deployed using **Docker + Docker Compose**. Inter-service communication is event-driven using **RabbitMQ**.

---

## 🎯 Key Features

- User registration and login (JWT-based authentication)
- Secure bidding — only authenticated users can place bids
- Custom rule enforcement: **bid must be higher** than current highest bid
- View and manage auction items
- View your personal bid history
- Auction closing triggers **automatic winner determination**
- RabbitMQ used to publish auction close events
- Winner service receives events and identifies the winner
- Fully containerized deployment using Docker Compose

---

## 🧱 System Architecture

The solution uses **Clean / Onion Architecture** ensuring:

- Business logic is independent of frameworks
- Infrastructure concerns (DB, messaging) are pluggable
- Controllers only coordinate operations and do not contain business logic

