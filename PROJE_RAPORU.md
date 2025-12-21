# FITNESS CENTER MANAGEMENT SYSTEM
## Proje Raporu

**Öğrenci Numarası:** B221210580  
**Öğrenci Adı:** WALID ALSHAWI  
**GitHub Repository:** https://github.com/waleed12587/FitnessCenterManagement  
**Tarih:** Aralık 2025

---

## 1. PROJE TANITIMI

### 1.1 Proje Hakkında

Fitness Center Management System, spor salonlarının hizmetlerini, antrenörlerini ve randevularını yönetmek için geliştirilmiş kapsamlı bir web uygulamasıdır. Bu proje, ASP.NET Core MVC 9.0 teknolojisi kullanılarak geliştirilmiştir ve Web Programlama dersi kapsamında hazırlanmıştır.

### 1.2 Proje Amacı

Projenin temel amacı, spor salonu yönetimini dijitalleştirmek ve şu işlevleri sağlamaktır:

- Spor salonlarının bilgilerini yönetme
- Antrenör bilgileri ve uzmanlık alanlarını takip etme
- Salon hizmetlerini (fitness, yoga, pilates vb.) tanımlama
- Üyelerin antrenörlerle randevu alabilmesi
- Yöneticilerin randevuları onaylama/reddetme
- AI destekli kişiselleştirilmiş egzersiz ve beslenme planları sunma

### 1.3 Teknoloji Stack

**Backend:**
- ASP.NET Core MVC 9.0
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity (Kimlik Doğrulama)

**Frontend:**
- Bootstrap 5
- HTML5, CSS3
- JavaScript, jQuery
- Bootstrap Icons

**AI Entegrasyonu:**
- OpenAI API (Opsiyonel)
- Fallback mekanizması

**Diğer:**
- RESTful API
- LINQ sorguları
- Client ve Server-side validation

### 1.4 Proje Özellikleri

#### Temel Özellikler:
- ✅ Spor Salonu Yönetimi (CRUD işlemleri)
- ✅ Antrenör Yönetimi (CRUD işlemleri)
- ✅ Hizmet Yönetimi (CRUD işlemleri)
- ✅ Randevu Sistemi
- ✅ Randevu Onay/Red Mekanizması
- ✅ Çakışma Kontrolü (Aynı antrenör için çakışan randevuları engelleme)

#### Teknik Özellikler:
- ✅ REST API Endpoints (LINQ filtreleme ile)
- ✅ AI Destekli Egzersiz ve Beslenme Önerileri
- ✅ Kullanıcı Kimlik Doğrulama (Register/Login)
- ✅ Rol Tabanlı Yetkilendirme (Admin, Member)
- ✅ Veri Doğrulama (Client ve Server-side)
- ✅ Modern ve Responsive UI (Bootstrap 5)

#### API Endpoints:
- `GET /api/TrainersApi` - Tüm antrenörleri listeleme (filtreleme ile)
- `GET /api/TrainersApi/available` - Belirli bir hizmet için müsait antrenörleri getirme
- `GET /api/AppointmentsApi/mine` - Kullanıcının randevularını getirme
- `GET /api/AppointmentsApi/trainer/{id}` - Antrenörün randevularını getirme

---

## 2. VERİTABANI MODELİ

### 2.1 Veritabanı Yapısı

Proje, SQL Server veritabanı kullanmaktadır ve Entity Framework Core ORM aracılığıyla yönetilmektedir. Veritabanı şeması aşağıdaki tablolardan oluşmaktadır:

### 2.2 Tablolar ve İlişkiler

#### 2.2.1 ApplicationUser (Kullanıcılar)
ASP.NET Core Identity sisteminden türetilmiş kullanıcı tablosudur.

**Özellikler:**
- `Id` (string, PK) - Kullanıcı kimliği
- `UserName` (string) - Kullanıcı adı
- `Email` (string) - E-posta adresi
- `FullName` (string, nullable) - Tam ad
- `HeightCm` (int, nullable) - Boy (cm)
- `WeightKg` (float, nullable) - Kilo (kg)
- `BodyType` (string, nullable) - Vücut tipi

**İlişkiler:**
- Bir kullanıcının birden fazla randevusu olabilir (1-N: `Appointments`)

#### 2.2.2 Gym (Spor Salonları)
Spor salonu bilgilerini tutan tablodur.

**Özellikler:**
- `Id` (int, PK) - Salon kimliği
- `Name` (string) - Salon adı
- `Address` (string, nullable) - Adres
- `OpeningTime` (TimeSpan) - Açılış saati
- `ClosingTime` (TimeSpan) - Kapanış saati

**İlişkiler:**
- Bir salonun birden fazla hizmeti olabilir (1-N: `GymServices`)
- Bir salonun birden fazla antrenörü olabilir (1-N: `Trainers`)

#### 2.2.3 GymService (Salon Hizmetleri)
Spor salonunun sunduğu hizmetleri tanımlayan tablodur.

**Özellikler:**
- `Id` (int, PK) - Hizmet kimliği
- `Name` (string) - Hizmet adı (örn: Fitness, Yoga, Pilates)
- `DurationMinutes` (int) - Süre (dakika cinsinden, 10-300 arası)
- `Price` (decimal, Precision: 18,2) - Fiyat
- `GymId` (int, FK) - Bağlı olduğu salon

**İlişkiler:**
- Bir hizmet bir salona aittir (N-1: `Gym`)
- Bir hizmet birden fazla antrenör tarafından verilebilir (N-N: `TrainerService` aracılığıyla)
- Bir hizmete birden fazla randevu alınabilir (1-N: `Appointments`)

#### 2.2.4 Trainer (Antrenörler)
Antrenör bilgilerini tutan tablodur.

**Özellikler:**
- `Id` (int, PK) - Antrenör kimliği
- `FullName` (string) - Tam ad
- `Specialty` (string, nullable) - Uzmanlık alanı
- `Bio` (string, nullable) - Biyografi
- `GymId` (int, FK, nullable) - Çalıştığı salon

**İlişkiler:**
- Bir antrenör bir salona bağlı olabilir (N-1: `Gym`)
- Bir antrenör birden fazla hizmet verebilir (N-N: `TrainerService` aracılığıyla)
- Bir antrenörün müsaitlik saatleri vardır (1-N: `TrainerAvailability`)
- Bir antrenörün birden fazla randevusu olabilir (1-N: `Appointments`)

#### 2.2.5 TrainerService (Antrenör-Hizmet İlişkisi)
Antrenörlerin hangi hizmetleri verebileceğini belirten ara tablodur (Many-to-Many ilişki).

**Özellikler:**
- `TrainerId` (int, FK, Composite PK) - Antrenör kimliği
- `GymServiceId` (int, FK, Composite PK) - Hizmet kimliği

**İlişkiler:**
- Bir antrenör-hizmet kaydı bir antrenöre aittir (N-1: `Trainer`)
- Bir antrenör-hizmet kaydı bir hizmete aittir (N-1: `GymService`)

#### 2.2.6 TrainerAvailability (Antrenör Müsaitlik Saatleri)
Antrenörlerin hangi günlerde ve saatlerde müsait olduğunu belirten tablodur.

**Özellikler:**
- `Id` (int, PK) - Müsaitlik kaydı kimliği
- `TrainerId` (int, FK) - Antrenör kimliği
- `DayOfWeek` (DayOfWeek enum) - Haftanın günü (Pazartesi, Salı, vb.)
- `StartTime` (TimeSpan) - Başlangıç saati
- `EndTime` (TimeSpan) - Bitiş saati

**İlişkiler:**
- Bir müsaitlik kaydı bir antrenöre aittir (N-1: `Trainer`)

#### 2.2.7 Appointment (Randevular)
Üyelerin antrenörlerle aldığı randevuları tutan tablodur.

**Özellikler:**
- `Id` (int, PK) - Randevu kimliği
- `MemberId` (string, FK) - Üye kimliği (ApplicationUser)
- `TrainerId` (int, FK) - Antrenör kimliği
- `GymServiceId` (int, FK) - Hizmet kimliği
- `StartDateTime` (DateTime) - Randevu başlangıç tarihi ve saati
- `EndDateTime` (DateTime) - Randevu bitiş tarihi ve saati
- `Price` (decimal, Precision: 18,2) - Fiyat
- `Status` (AppointmentStatus enum) - Randevu durumu (Pending, Approved, Rejected, Cancelled)

**İlişkiler:**
- Bir randevu bir üyeye aittir (N-1: `ApplicationUser` - Member)
- Bir randevu bir antrenöre aittir (N-1: `Trainer`)
- Bir randevu bir hizmete aittir (N-1: `GymService`)

### 2.3 Enum Tipleri

#### AppointmentStatus (Randevu Durumu)
- `Pending` - Beklemede
- `Approved` - Onaylandı
- `Rejected` - Reddedildi
- `Cancelled` - İptal edildi

#### PaymentStatus (Ödeme Durumu)
- `Pending` - Ödeme bekleniyor
- `Paid` - Ödendi
- `Failed` - Ödeme başarısız
- `Refunded` - İade edildi

### 2.4 Veritabanı İlişki Diyagramı

```
ApplicationUser (1) ────< (N) Appointment
                              │
                              ├───> (N) Trainer
                              │
                              └───> (N) GymService

Gym (1) ────< (N) GymService
    │
    └───< (N) Trainer

Trainer (1) ────< (N) TrainerService ────> (N) GymService
    │
    └───< (N) TrainerAvailability
    │
    └───< (N) Appointment
```

### 2.5 Veritabanı Kısıtlamaları

- **Fiyat Hassasiyeti:** `GymService.Price` ve `Appointment.Price` alanları `decimal(18,2)` formatında saklanır.
- **Composite Key:** `TrainerService` tablosunda `(TrainerId, GymServiceId)` composite primary key olarak kullanılır.
- **Foreign Key İlişkileri:** Tüm foreign key ilişkileri Entity Framework Core tarafından otomatik olarak yönetilir.
- **Cascade Delete:** İlişkili kayıtlar silindiğinde bağımlı kayıtlar da silinir (varsayılan davranış).

---

## 3. EKRAN GÖRÜNTÜLERİ

> **NOT:** Aşağıda belirtilen ekran görüntüleri projeye dahil edilmelidir. Her ekran görüntüsü için açıklama verilmiştir.

### 3.1 Ana Sayfa (Home Page)
**Dosya:** `screenshot-01-homepage.png`

Ana sayfa, kullanıcıları karşılayan ilk ekrandır. Bootstrap 5 ile tasarlanmış modern bir arayüze sahiptir. Sayfada şu bölümler bulunur:
- Hero section (Başlık ve kısa açıklama)
- Özellik kartları (Gyms, Trainers, Appointment System)
- AI özellik tanıtımı
- Navigasyon menüsü

### 3.2 Giriş Sayfası (Login Page)
**Dosya:** `screenshot-02-login.png`

Kullanıcı giriş sayfası, "Remember Me" özelliği ile birlikte gösterilmelidir. Form validasyon mesajları görüntülenebilir.

### 3.3 Spor Salonları Listesi (Gyms Index)
**Dosya:** `screenshot-03-gyms.png`

Spor salonlarının listelendiği sayfa. Admin kullanıcıları için "Add New Gym", "Edit", "Delete" butonları görünür. Normal kullanıcılar sadece "Details" butonunu görebilir.

### 3.4 Hizmetler Listesi (Services Index)
**Dosya:** `screenshot-04-services.png`

Salon hizmetlerinin (Fitness, Yoga, Pilates vb.) listelendiği sayfa. Her hizmet için fiyat, süre ve bağlı olduğu salon bilgisi gösterilir.

### 3.5 Antrenörler Listesi (Trainers Index)
**Dosya:** `screenshot-05-trainers.png`

Antrenörlerin listelendiği tablo görünümü. Antrenör adı, uzmanlık alanı, biyografi ve bağlı olduğu salon bilgileri gösterilir.

### 3.6 Randevu Oluşturma (Create Appointment)
**Dosya:** `screenshot-06-create-appointment.png`

Üyelerin randevu oluşturduğu form sayfası. Form alanları:
- Hizmet seçimi (dropdown)
- Antrenör seçimi (dropdown)
- Başlangıç tarihi ve saati (datetime picker)

### 3.7 Randevularım (My Appointments)
**Dosya:** `screenshot-07-my-appointments.png`

Kullanıcının kendi randevularını görüntülediği sayfa. Tabloda şu bilgiler gösterilir:
- Tarih/Saat
- Antrenör adı
- Hizmet adı
- Fiyat
- Randevu durumu (Pending, Approved, Rejected)

### 3.8 Admin Paneli - Bekleyen Randevular (Admin Panel - Pending Appointments)
**Dosya:** `screenshot-08-admin-pending.png`

Yöneticilerin bekleyen randevuları görüntüleyip onayladığı/reddettiği sayfa. Her randevu için "Approve" ve "Reject" butonları bulunur.

### 3.9 AI Öneri Sayfası (AI Workout Recommendation)
**Dosya:** `screenshot-09-ai-workout.png`

Kullanıcıların kişisel bilgilerini girerek AI destekli egzersiz ve beslenme planı aldığı form sayfası. Form alanları:
- Yaş
- Boy (cm)
- Kilo (kg)
- Cinsiyet
- Hedef (Weight Loss, Muscle Building, vb.)
- Aktivite seviyesi
- Sağlık durumu (opsiyonel)

### 3.10 AI Öneri Sonuçları (AI Workout Results)
**Dosya:** `screenshot-10-ai-results.png`

AI tarafından oluşturulan kişiselleştirilmiş egzersiz ve beslenme planının gösterildiği sayfa. Plan şu bölümleri içerir:
- Haftalık egzersiz programı
- Beslenme önerileri
- İlerleme takibi önerileri
- Motivasyon ipuçları

### 3.11 REST API Test (Postman veya Browser)
**Dosya:** `screenshot-11-api-test.png`

REST API endpoint'lerinin test edildiği ekran görüntüsü. Örnek:
- `/api/TrainersApi` endpoint'inin JSON response'u
- `/api/TrainersApi/available?serviceId=1&date=2025-12-20` endpoint'inin sonucu

### 3.12 Veritabanı Şeması (Database Schema)
**Dosya:** `screenshot-12-database-schema.png`

SQL Server Management Studio veya Visual Studio'dan alınmış veritabanı tablolarının ve ilişkilerinin gösterildiği ekran görüntüsü.

---

## 4. PROJE YAPISI

### 4.1 Klasör Yapısı

```
FitnessCenter.Web/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   │   └── AppointmentsController.cs
│   │   └── Views/
│   │       └── Appointments/
│   │           └── Pending.cshtml
│   └── Identity/
│       └── Pages/
│           └── Account/
│               └── Login.cshtml
├── Controllers/
│   ├── Api/
│   │   ├── AppointmentsApiController.cs
│   │   └── TrainersApiController.cs
│   ├── AIWorkoutController.cs
│   ├── AppointmentsController.cs
│   ├── GymsController.cs
│   ├── GymServicesController.cs
│   ├── PaymentController.cs
│   └── TrainersController.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Models/
│   ├── ApplicationUser.cs
│   ├── Appointment.cs
│   ├── AppointmentStatus.cs
│   ├── Gym.cs
│   ├── GymService.cs
│   ├── PaymentStatus.cs
│   ├── Trainer.cs
│   ├── TrainerAvailability.cs
│   └── TrainerService.cs
├── ViewModels/
│   ├── AIWorkoutRequestVM.cs
│   └── AppointmentCreateVM.cs
├── Views/
│   ├── AIWorkout/
│   ├── Appointments/
│   ├── Gyms/
│   ├── GymServices/
│   ├── Home/
│   ├── Payment/
│   ├── Trainers/
│   └── Shared/
└── wwwroot/
    └── uploads/
```

### 4.2 Önemli Dosyalar

- **Program.cs:** Uygulama yapılandırması, servis kayıtları, middleware yapılandırması
- **ApplicationDbContext.cs:** Entity Framework Core DbContext sınıfı
- **appsettings.json:** Veritabanı bağlantı string'i ve yapılandırma ayarları

---

## 5. KURULUM VE ÇALIŞTIRMA

### 5.1 Gereksinimler

- .NET 9.0 SDK
- SQL Server (LocalDB veya SQL Server Express)
- Visual Studio 2022 veya Visual Studio Code

### 5.2 Kurulum Adımları

1. **Repository'yi klonlayın:**
   ```bash
   git clone https://github.com/waleed12587/FitnessCenterManagement.git
   ```

2. **Proje klasörüne gidin:**
   ```bash
   cd FitnessCenterManagement/FitnessCenter.Web
   ```

3. **Veritabanı bağlantı string'ini güncelleyin:**
   `appsettings.json` dosyasında `ConnectionStrings:DefaultConnection` değerini kendi SQL Server bağlantınıza göre düzenleyin.

4. **Migration'ları uygulayın:**
   ```powershell
   Update-Database
   ```
   (Visual Studio Package Manager Console'dan)

5. **Uygulamayı çalıştırın:**
   ```bash
   dotnet run
   ```

### 5.3 Varsayılan Admin Hesabı

- **Email:** ogrencinumarasi@sakarya.edu.tr
- **Şifre:** sau
- **Rol:** Admin

---

## 6. GELİŞTİRME SÜRECİ

### 6.1 Commit Geçmişi

Proje, 15 Aralık 2025 tarihinden itibaren düzenli commit'lerle geliştirilmiştir. GitHub repository'sinde 18+ commit bulunmaktadır.

### 6.2 Versiyon Kontrolü

Proje, Git versiyon kontrol sistemi kullanılarak yönetilmektedir. Tüm değişiklikler GitHub'da saklanmaktadır.

---

## 7. SONUÇ

Fitness Center Management System, modern web teknolojileri kullanılarak geliştirilmiş, kapsamlı bir spor salonu yönetim sistemidir. Proje, ASP.NET Core MVC, Entity Framework Core, ve Bootstrap 5 gibi güncel teknolojilerle geliştirilmiş olup, RESTful API, AI entegrasyonu, ve rol tabanlı yetkilendirme gibi gelişmiş özellikler içermektedir.

Proje, Web Programlama dersi kapsamında başarıyla tamamlanmış ve GitHub'da yayınlanmıştır.

---

## 8. REFERANSLAR

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.0)
- [GitHub Repository](https://github.com/waleed12587/FitnessCenterManagement)

---

**Hazırlayan:** WALID ALSHAWI  
**Öğrenci No:** B221210580  
**Tarih:** Aralık 2025



