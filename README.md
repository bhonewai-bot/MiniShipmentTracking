# 📦 MiniShipment

**Type:** Web API – Shipment Tracking System  
**Backend:** ASP.NET Core Web API  
**Database:** MSSQL

---

## 📌 Overview

MiniShipment is a lightweight shipment tracking API designed to model real-world delivery flow with clear status transitions and tracking history.

The system focuses on:
- Creating shipments with a unique tracking number
- Updating shipment status step by step (no skipping)
- Recording tracking events for every status change
- Allowing public users to track shipments without authentication
- Restricting operational actions to Staff and Admin roles

---

## 🔄 Shipment Flow

Each shipment follows a strict status progression:

```
Created → PickedUp → InTransit → OutForDelivery → Delivered
```


- Status transitions are validated using a state machine
- Invalid jumps (e.g. Created → Delivered) are rejected
- Every valid transition creates a tracking event

---

## 🔐 Authentication & Authorization

- Authentication is handled via custom middleware
- UserId and Role are injected into HttpContext.Items
- No JWT is used (simplified session-based approach)
- 
### Public Access
- Shipment tracking by TrackingNo is public
- No authentication required for tracking

---

## 👤 User Roles

### Staff

- Create shipments
- Update shipment status (step-by-step)
- Add tracking events during delivery flow

### Admin

- All Staff permissions
- Correct tracking events (human error handling)

---

## 🛠️ Tech Notes

- Built with ASP.NET Core Web API
- Entity Framework Core for data access
- Clean separation between controllers, services, and domain logic
- Designed as an MVP with real-world logistics constraints in mind