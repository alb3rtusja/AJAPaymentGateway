# AJA Payment Gateway

Self-portfolio project: a mini payment gateway built with ASP.NET Core (.NET 8),
focused on reliability, webhook delivery, and real-world payment patterns.

---

## ✨ Features

- Create payment & track status
- Idempotent payment creation (by OrderId)
- Webhook delivery with retry mechanism
- Webhook Outbox Pattern (reliable delivery)
- Admin Dashboard
  - Payments monitoring
  - Webhook logs
  - Webhook outbox + manual retry
- Unit tests (xUnit + Moq)

---

## 🧠 Architecture Highlights

- Domain-first service design
- Outbox pattern for webhook reliability
- Retry limit & failure isolation
- BackgroundService for webhook processing
- Clean separation:
  Controller → Application Service → Domain

---

## 🛠 Tech Stack

- ASP.NET Core (.NET 8)
- Entity Framework Core
- xUnit + Moq
- Bootstrap (Admin UI)

---

## 🎯 Why This Project?

This project was built as a **self-portfolio** to demonstrate
experience with real payment gateway problems:

- Network failures
- Duplicate requests (idempotency)
- Webhook retry & observability
- Admin monitoring & manual retry

---

## ⚠️ Disclaimer

This project is for learning & portfolio purposes only.
Not intended for production use.

