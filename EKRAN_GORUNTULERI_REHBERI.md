# EKRAN GÖRÜNTÜLERİ REHBERİ

Bu dosya, proje raporuna eklenecek ekran görüntülerinin listesini ve nasıl alınacağını içermektedir.

## Alınması Gereken Ekran Görüntüleri

### 1. Ana Sayfa (Home Page)
**Dosya Adı:** `screenshot-01-homepage.png`
- Tarayıcıda ana sayfayı açın
- Tam sayfa ekran görüntüsü alın
- Hero section, özellik kartları ve navigasyon menüsü görünür olmalı

### 2. Giriş Sayfası (Login)
**Dosya Adı:** `screenshot-02-login.png`
- `/Identity/Account/Login` sayfasını açın
- "Remember Me" checkbox'ı görünür olmalı
- Form alanları (Email, Password) görünür olmalı

### 3. Spor Salonları Listesi
**Dosya Adı:** `screenshot-03-gyms.png`
- `/Gyms` sayfasını açın
- Admin olarak giriş yapın (butonlar görünür olmalı)
- En az 2-3 salon kaydı olmalı

### 4. Hizmetler Listesi
**Dosya Adı:** `screenshot-04-services.png`
- `/GymServices` sayfasını açın
- Hizmet kartları görünür olmalı
- Fiyat, süre bilgileri görünür olmalı

### 5. Antrenörler Listesi
**Dosya Adı:** `screenshot-05-trainers.png`
- `/Trainers` sayfasını açın
- Tablo görünümü görünür olmalı
- Antrenör adları, uzmanlık alanları görünür olmalı

### 6. Randevu Oluşturma
**Dosya Adı:** `screenshot-06-create-appointment.png`
- Üye olarak giriş yapın
- `/Appointments/Create` sayfasını açın
- Form alanları (Service, Trainer, Start DateTime) görünür olmalı

### 7. Randevularım
**Dosya Adı:** `screenshot-07-my-appointments.png`
- Üye olarak giriş yapın
- `/Appointments/MyAppointments` sayfasını açın
- En az 1 randevu kaydı olmalı
- Randevu durumu (Pending, Approved) görünür olmalı

### 8. Admin Paneli - Bekleyen Randevular
**Dosya Adı:** `screenshot-08-admin-pending.png`
- Admin olarak giriş yapın
- `/Admin/Appointments/Pending` sayfasını açın
- En az 1 bekleyen randevu olmalı
- "Approve" ve "Reject" butonları görünür olmalı

### 9. AI Öneri Formu
**Dosya Adı:** `screenshot-09-ai-workout.png`
- Giriş yapın
- `/AIWorkout` sayfasını açın
- Form alanları (Age, Height, Weight, Gender, Goal, Activity Level) görünür olmalı

### 10. AI Öneri Sonuçları
**Dosya Adı:** `screenshot-10-ai-results.png`
- AI formunu doldurup gönderin
- Sonuç sayfasını gösterin
- Egzersiz planı ve beslenme önerileri görünür olmalı

### 11. REST API Test
**Dosya Adı:** `screenshot-11-api-test.png`
- Postman veya tarayıcıda şu endpoint'i açın:
  `https://localhost:xxxx/api/TrainersApi`
- JSON response görünür olmalı
- Veya tarayıcı developer tools'da Network tab'ı gösterin

### 12. Veritabanı Şeması
**Dosya Adı:** `screenshot-12-database-schema.png`
- SQL Server Management Studio'yu açın
- Database → FitnessCenter → Tables
- Tüm tabloları gösterin
- Veya Visual Studio'da SQL Server Object Explorer'dan tabloları gösterin

## Ekran Görüntüsü Alma Yöntemleri

### Windows:
- **Tam Ekran:** `Windows + Print Screen`
- **Aktif Pencere:** `Alt + Print Screen`
- **Snipping Tool:** `Windows + Shift + S`

### Tarayıcı:
- **Chrome DevTools:** `F12` → Elements/Network tab
- **Tam Sayfa:** Browser extension kullanın (örn: Full Page Screen Capture)

## Ekran Görüntülerini Rapor'a Ekleme

1. Tüm ekran görüntülerini `screenshots/` klasörüne kaydedin
2. Word belgesine eklerken:
   - Her ekran görüntüsünün altına açıklama ekleyin
   - Görüntüleri uygun boyutlarda yerleştirin
   - Sayfa düzenini bozmayacak şekilde ayarlayın

## Notlar

- Ekran görüntüleri net ve okunabilir olmalı
- Gereksiz bilgiler (kişisel veriler) gizlenmeli
- Her ekran görüntüsü ilgili bölümde referans verilmelidir



