# 🏨 HotelBookingSystem

An AI-powered hotel booking system with React.js frontend, ASP.NET Core backend, and Ocelot API Gateway. Features include AI hotel search by city, dates, people; hotel listing with rooms and discounted prices; comments and rating system using MongoDB; booking reservations with automatic capacity decrement; email notifications when room capacity falls below 20%; and a microservices architecture managed via Ocelot gateway.

Built with React.js (frontend), ASP.NET Core Web API (backend), MySQL for hotels/rooms/bookings/users, MongoDB for comments, Ocelot for API Gateway, and SMTP for notification emails.

## Setup Instructions

Clone the repository:

git clone https://github.com/yourusername/HotelBookingSystem.git
cd HotelBookingSystem

Configure database connections in appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=hotelbooking;user=root;password=yourpassword"
},
"MongoDBSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "HotelBookingDB",
  "CommentsCollectionName": "Comments"
}

Apply EF Core migrations:

dotnet ef database update

Run the backend:

dotnet run

It runs on https://localhost:5177

Run the React frontend:

cd Frontend
npm install
npm start

It runs on http://localhost:3000

Configure and run the API Gateway (Ocelot). Example ocelot.json route:

{
  "DownstreamPathTemplate": "/api/v{version}/hotel",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    { "Host": "localhost", "Port": 5177 }
  ],
  "UpstreamPathTemplate": "/api/v{version}/hotel",
  "UpstreamHttpMethod": [ "GET", "POST", "OPTIONS" ]
}

Run gateway on https://localhost:5000

Notification Email Setup: Configure EmailService.cs with your SMTP credentials:

var smtpClient = new SmtpClient("smtp.gmail.com")
{
    Port = 587,
    Credentials = new NetworkCredential("youremail@gmail.com", "app_password"),
    EnableSsl = true,
};

Use Gmail App Password if 2FA is enabled.

Key Endpoints:

/api/v1/ai/ask (POST) – AI hotel search
/api/v1/hotel (GET/POST) – Hotels CRUD
/api/v1/booking (GET/POST) – Bookings CRUD
/api/v1/comments/hotel/{id} (GET) – Get hotel comments
/api/v1/notification/send (POST) – Trigger email notification if capacity < 20%

Project Structure:

HotelBookingSystem/
Controllers/
Models/
Services/
Repositories/
Data/
Migrations/
Frontend/
src/
package.json
ocelot.json
README.md

Maintainer: Gülce Doğruöz – Full Stack Development

SE4458 Final Project, 2025 Spring – Built with ❤️ and caffeine.
