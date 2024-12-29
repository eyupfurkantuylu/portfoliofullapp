# Portfolio Full Stack Uygulama

Bu proje, kişisel portfolyo web uygulamasının backend ve frontend bileşenlerini içermektedir.

## Backend (API) Projesi

Backend projesi, modern ve güvenli bir API sunmak için aşağıdaki teknolojileri kullanmaktadır:

### Kullanılan Teknolojiler

- **Framework:** .NET 8
- **Programlama Dili:** C#
- **ORM ve Database Access:**
  - Entity Framework Core (Database şeması oluşturma için)
  - Dapper (Veritabanı sorguları için)
- **Güvenlik:**
  - IdentityServer4
  - JWT Token Authentication
- **Mimari:**
  - Repository Pattern
  - DTO (Data Transfer Objects)
  - Clean Architecture

## Frontend Projesi

Frontend projesi, kullanıcı dostu ve modern bir arayüz sunmak için Magic UI'ın Portfolio Template'ini kullanmaktadır.

### Kullanılan Teknolojiler

- Magic UI Portfolio Template
- React.js
- TypeScript
- Tailwind CSS
- Axios (API iletişimi için)

## Proje Yapısı

```
portfoliofullapp/
├── backendapp/         # .NET API projesi
│   ├── src/           # Kaynak kodlar
│   ├── tests/         # Test projeleri
│   └── docs/          # API dokümantasyonu
│
└── frontendapp/       # React frontend projesi
    ├── src/          # Kaynak kodlar
    ├── public/       # Statik dosyalar
    └── components/   # React bileşenleri
```

## Başlangıç

### Backend Gereksinimleri

- .NET 8 SDK
- SQL Server
- Riderr

### Frontend Gereksinimleri

- Node.js
- npm veya yarn
- Cursor (önerilen)
