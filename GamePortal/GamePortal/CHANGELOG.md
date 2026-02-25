# Changelog

## [1.0.0] - 2026-02-15
### Added
- User registration (test version)
- Game purchase system
- Game cards layout
- Online games section( one test game)

## [1.1.0] - 2026-02-17
### Added
- Role "Admin"
- Automatic admin creation at startup
- Access restriction for Edit/Delete pages
- Serilog logging for user registration
- Log level override for Microsoft and System

### Changed
- Improved authorization architecture

## [v1.1.1] - 2026-02-19

### Changed
- EF Core logging level has been lowered to Error
- Added logging of game purchases

### Added
- "Purchased" button instead of repurchase
- Visual indicator of purchased game

## [v1.2.0] - 2026-02-22

### Added
- Shopping Cart system (CartItems entity)
- Checkout process with transaction
- Price fixation at purchase moment (PriceAtPurchase)
- Cart page with total calculation
- Protection against duplicate cart entries
- Active/Inactive game handling
- Three full demo games with images and descriptions

### Changed
- Purchase logic redesigned (Cart → Checkout → Purchases)
- Details page redesigned (cover image, screenshots layout)
- Games Index now hides inactive games for non-admin users

## [v1.3.0] - 2026-02-23

### Added
- Minimal REST API for Games (GET, POST, PUT, DELETE)
- DTO models (CreateGameDto, UpdateGameDto) for API data validation
- Role-based authorization for API endpoints (AdminOnly policy)
- Swagger UI integration for interactive API testing
- Password visibility toggle on Login and Register pages
- Russian localization for Identity validation errors
- News system (Create / Index / Details)
- 9 demo news records for testing
- Randomized display of 6 published news on /News page

### Changed
- Replaced default ILogger with Serilog for structured logging
- Implemented role-based log separation (UserLog / AdminLog)
- Improved login error handling (generic message for security)
- Enhanced authentication flow stability

### Security
- API endpoints protected via authorization policies
- Game modification (POST/PUT/DELETE) restricted to Admin role
- Improved validation responses via ValidationProblemDetails