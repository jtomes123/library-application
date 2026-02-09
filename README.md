# Library Application

A library management system built with ASP.NET Core and Blazor that allows users to browse books, borrow and return book copies.

## Table of Contents

- [Overview](#overview)
- [API Documentation](#api-documentation)
  - [Books Endpoints](#books-endpoints)
  - [Users Endpoints](#users-endpoints)
- [Data Models](#data-models)
- [Authentication](#authentication)

## Overview

This application provides a complete library management system with:
- Book catalog management
- Book copy tracking
- User borrowing and returning functionality
- Search capabilities
- Admin role for adding new books

## API Documentation

All API endpoints are prefixed with `/api` and are currently configured to allow anonymous access.

### Books Endpoints

#### Get All Books

```http
GET /api/books
```

Retrieves a list of all books in the library.

**Response:**
- **Status:** `200 OK`
- **Body:** Array of `BookDto` objects

**Example Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "The Great Gatsby",
    "author": "F. Scott Fitzgerald",
    "year": 1925,
    "isbn": "9780743273565",
    "availableCopies": 3
  }
]
```

---

#### Get Book Copies by Book ID

```http
GET /api/books/{bookId}/copies
```

Retrieves all copies of a specific book, including their availability status and current borrower.

**Path Parameters:**
- `bookId` (Guid, required): The unique identifier of the book

**Response:**
- **Status:** `200 OK`
- **Body:** Array of `BookCopyDto` objects

**Example Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "The Great Gatsby",
    "author": "F. Scott Fitzgerald",
    "year": 1925,
    "isbn": "9780743273565",
    "borrower": "John Doe",
    "isAvailable": false
  },
  {
    "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "name": "The Great Gatsby",
    "author": "F. Scott Fitzgerald",
    "year": 1925,
    "isbn": "9780743273565",
    "borrower": null,
    "isAvailable": true
  }
]
```

---

### Users Endpoints

#### Get User's Borrowed Books

```http
GET /api/users/{userId}/books
```

Retrieves all book copies currently borrowed by a specific user.

**Path Parameters:**
- `userId` (Guid, required): The unique identifier of the user

**Response:**
- **Status:** `200 OK`
- **Body:** Array of `BookCopyDto` objects

**Example Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "The Great Gatsby",
    "author": "F. Scott Fitzgerald",
    "year": 1925,
    "isbn": "9780743273565",
    "borrower": "John Doe",
    "isAvailable": false
  }
]
```

---

#### Borrow Book Copy

```http
POST /api/users/{userId}/borrow/byCopyId/{copyId}
```

Allows a user to borrow a specific book copy. The book copy must be available.

**Path Parameters:**
- `userId` (Guid, required): The unique identifier of the user
- `copyId` (Guid, required): The unique identifier of the book copy

**Response:**
- **Status:** `200 OK` - Book copy successfully borrowed
- **Status:** `400 Bad Request` - User not found or failed to borrow book copy

**Error Response Example:**
```json
"User not found"
```
or
```json
"Failed to borrow book copy"
```

---

#### Return Book Copy

```http
POST /api/users/{userId}/return/byCopyId/{copyId}
```

Allows a user to return a borrowed book copy.

**Path Parameters:**
- `userId` (Guid, required): The unique identifier of the user
- `copyId` (Guid, required): The unique identifier of the book copy

**Response:**
- **Status:** `200 OK` - Book copy successfully returned
- **Status:** `400 Bad Request` - User not found or failed to return book copy

**Error Response Example:**
```json
"User not found"
```
or
```json
"Failed to return book copy"
```

---

## Data Models

### BookDto

Represents a book in the library catalog.

```csharp
{
  "id": "Guid",           // Unique identifier for the book
  "name": "string",       // Title of the book (max 128 characters)
  "author": "string",     // Author name (max 128 characters)
  "year": "integer",      // Publication year (range: -10000 to 10000)
  "isbn": "string",       // ISBN-13 (exactly 13 characters)
  "availableCopies": "integer"  // Number of available copies for borrowing
}
```

---

### BookCopyDto

Represents a physical copy of a book.

```csharp
{
  "id": "Guid",           // Unique identifier for the book copy
  "name": "string",       // Title of the book
  "author": "string",     // Author name
  "year": "integer",      // Publication year
  "isbn": "string",       // ISBN-13
  "borrower": "string?",  // Name of current borrower (null if available)
  "isAvailable": "boolean"  // Availability status
}
```

---

### UserDto

Represents a library user.

```csharp
{
  "id": "Guid",           // Unique identifier for the user
  "name": "string",       // User's full name
  "email": "string"       // User's email address
}
```

---

### HistoryEntryDto

Represents a history entry for tracking borrowing and returning actions.

```csharp
{
  "id": "Guid",           // Unique identifier for the history entry
  "timeStamp": "DateTime",  // When the action occurred
  "action": "HistoryAction",  // Type of action (enum)
  "user": "Guid"          // User ID who performed the action
}
```

---

## Authentication

The API currently has antiforgery protection disabled and allows anonymous access for all endpoints. 

The web interface includes role-based authorization:
- **Admin Role**: Can add new books to the library catalog
- **Regular Users**: Can browse books, borrow and return book copies

---

## Features

### Book Management
- Browse all books in the catalog
- Search books by name, author, or ISBN
- View book copies and their availability
- Admin users can add new books with validation:
  - Required fields: Name, Author, Year, ISBN
  - ISBN must be exactly 13 characters
  - Year must be between -10000 and 10000

### User Operations
- View borrowed books
- Borrow available book copies
- Return borrowed book copies
- Track borrowing history

---

## Technical Stack

- **Framework:** ASP.NET Core with Blazor
- **Database:** SQLite (Entity Framework Core)
- **UI Components:** Fluent UI Blazor Components
- **Authentication:** ASP.NET Core Identity with role-based authorization

---

## Getting Started

### Prerequisites
- .NET 10.0 SDK or later

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd LibraryApplication
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Access the application in your browser (default URL will be shown in the console)

---

## Database

The application uses SQLite with the following database file:
- `library.db` - Main database file

The database includes tables for:
- Books
- Book Copies
- Users
- History Entries

---

