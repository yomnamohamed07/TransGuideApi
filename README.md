# 🚆 TransGuide System

TransGuide is an intelligent transportation guidance system that combines AI-based sign language recognition, real-time communication, route optimization, and user management services to provide an accessible and efficient transportation experience.

---

## 📌 Overview

The system is built using a modular backend architecture with ASP.NET Core and supports:

- Real-time sign language recognition
- AI-powered gesture prediction
- Smart route planning and optimization
- Voice-to-text conversion
- JWT Authentication and Role-Based Access Control (RBAC)
- User trip history tracking
- High-performance geolocation using Redis

---

## 🏗️ Architecture

- Frontend: React
- Backend: ASP.NET Core Web API
- Real-time: SignalR
- Database: SQL Server
- Cache/Geo: Redis
- AI Services: External AI Model (MediaPipe + custom API)

---

## 🤖 AI Integration

### AiService
- Receives SessionId + Landmarks
- Converts data to JSON
- Sends HTTP request to AI model
- Returns prediction (label, character, confidence)

### SessionWatcher
- Background service
- Runs every 2 seconds
- Ends inactive sessions automatically

### SignHub (SignalR)
- Real-time communication hub
- Sends and receives landmarks
- Returns AI predictions instantly

### SignSessionService
- Creates and manages sessions
- Tracks activity (heartbeat)
- Auto ends inactive sessions
- Broadcasts session status

### VoiceServices
- Converts speech to text
- Uses Hugging Face API
- Returns transcription result

---

## 👤 Authentication & Authorization

### AuthService
- User registration
- Login with JWT
- Google OAuth login

### AuthorizationService (RBAC)
- Create / update / delete roles
- Assign roles to users
- Manage permissions

### JWT
- Claims: UserId, Email, Roles
- Configurable expiration

---

## 🔐 Password Reset (OTP)

1. Generate 6-digit OTP  
2. Send via email  
3. Validate OTP (10 minutes expiry)  
4. Reset password securely  

---

## 📧 Email Service

- SMTP using MailKit
- HTML emails supported
- Used for OTP & notifications
- Async sending

---

## 🗺️ Location & Routing System

### LocationService
- Finds nearest stations
- Fuzzy search for station names
- Arabic text normalization
- Returns best route options

### Fuzzy Matching
- Uses FuzzySharp
- TokenSetRatio & TokenSortRatio
- Threshold: 75%

### Arabic Normalization
- أ → ا
- إ → ا
- آ → ا
- ة → ه
- ى → ي
- ئ → ي
- ؤ → و

---

## 📊 Route Optimization

Graph-based system:

- Nodes = Stations
- Edges = Connections

### Algorithm
Modified Dijkstra:

Priority = (Transfers × 1000) + Distance

### Features:
- Max 2 transfers
- Loop prevention
- Direct route filtering
- Ranked results

### Distance
Uses Haversine Formula for accurate Earth distance.

---

## 🧭 GeoLocation (Redis)

- GEOADD for storing stations
- GEORADIUS for search
- 5km radius search

Benefits:
- Fast performance
- Low DB load
- Scalable search

---

## 📂 Core Services

### StationService
- Create / update / delete (soft delete)
- Search + pagination

### RouteService
- Route management
- Filtering + relationships

### FeedbackService
- Ratings (1–5)
- Validation checks
- Analytics support

---

## 📊 Dashboard

Uses:
- StationService
- RouteService
- FeedbackService

Provides:
- Real-time analytics
- System statistics

### UI Counters:
- Total users
- Total trips
- Total feedbacks

---

## 🧠 Features Summary

- AI sign language recognition
- Real-time SignalR communication
- Smart route optimization
- Arabic fuzzy search support
- Redis geospatial indexing
- JWT authentication + RBAC
- OTP password reset
- Voice-to-text system
- Trip history tracking

---

## 🔄 Workflow

1. User creates session  
2. SignalR connection starts  
3. Landmarks streamed to backend  
4. AI returns prediction  
5. Session updated in real-time  
6. Inactive sessions auto-ended  
7. Routes calculated and stored  
8. Voice input processed separately  

---

## 🚀 Tech Stack

- ASP.NET Core
- SignalR
- SQL Server
- Redis
- JWT
- MailKit
- FuzzySharp
- MediaPipe
- Hugging Face API
