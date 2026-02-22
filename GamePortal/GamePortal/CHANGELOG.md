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
- Кнопка "Куплено" вместо повторной покупки
- Визуальный индикатор купленной игры

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