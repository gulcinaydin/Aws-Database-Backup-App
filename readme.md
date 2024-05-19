# AWS Database Backup App

Yukar�daki proje ile, **AWS EC2** hizmeti �zerinde y�kl� olan mariaDB veritaban�ndaki **employees** veritaban� phpMyAdmin ile y�netilebilmektedir. Employees veritaban� **S3** hizmetine yedeklenebilmekte ve bilgisayara indirilebilmektedir.  

 - EC2 hizmetinde y�kl� olan ubuntu server'da bir sql backup'� olu�turulur.
 - Bu sql backup'� ziplenerek boyutunun k���lt�lmesi sa�lan�r.
 - Ziplenmi� dosya S3'e y�klenir.
 - Y�klenmi� dosya bilgisayara indirilebilir.
 - T�m bunlar�n yap�labildi�i projemiz de AWS �zerinde publish edildi.

(Projenin �al��t�r�labilmesi i�in, AccessKey ve AccessSecret'�n appsettings'e eklenmesi gerekmektedir.)
 
### Projenin canl� haline a�a��daki linkten eri�ebilirsiniz.

[Proje linki](http://awsdatabasebackup-gulcinaydin-prod.eba-qydpzsue.eu-north-1.elasticbeanstalk.com/)

![Proje](./doc/screenshots/screenshot1.png)

### EC2 Ubuntu Server �zerinde olu�turulup zip haline getirilmi� sql backup'�

![EC2](./doc/screenshots/screenshot3.png)

### S3'e y�klenmi� zip dosyas�

![S3](./doc/screenshots/screenshot2.png)

### PhpMyAdmin Employees �rnek Veritaban�

![PhpMyAdmin](./doc/screenshots/screenshot4.png)
