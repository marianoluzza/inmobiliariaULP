# Base de datos

El sistema usa SQL Server LocalDB, que se instala junto con Visual Studio. Para verificar que lo tenés, corré `sqllocaldb info`: tiene que aparecer `MSSQLLocalDB`.

Hay dos formas de tener la base con los datos de ejemplo. Alcanza con una.

## Opción 1: crearla con el script (recomendada)

`seed.sql` crea la base `BDInmobiliaria` con todas las tablas y los datos de ejemplo.

1. Abrí una terminal en la carpeta `Inmobiliaria .Net Core` y corré:

   ```
   sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i Data\seed.sql
   ```

   Al terminar dice `Base BDInmobiliaria creada.`

2. En `appsettings.json`, cambiá `DefaultConnection` por:

   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BDInmobiliaria;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   ```

   Es la misma cadena que ya figura como `LocalDB`.

Si la base ya existe, el script no hace nada y lo avisa. Para empezar de cero, borrala y volvé a correrlo:

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -Q "DROP DATABASE BDInmobiliaria"
```

## Opción 2: usar los archivos del zip

`Data.7z` trae los archivos de la base ya armados (`BDInmobiliaria.mdf` y su `.ldf`).

1. Descomprimilo en esta misma carpeta (`Data`).
2. En `appsettings.json`, apuntá `DefaultConnection` al `.mdf` con su ruta completa. Por ejemplo:

   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=C:\\Git\\inmobiliariaULP\\Inmobiliaria .Net Core\\Data\\BDInmobiliaria.mdf;TrustServerCertificate=True"
   ```

   Cambiá `C:\\Git\\inmobiliariaULP` por la carpeta donde clonaste el repo. En JSON, cada `\` se escribe doble.

## Usuarios para probar

Todos los usuarios y propietarios de ejemplo tienen la clave `123`.

- Administrador: `mluzza@ulp.edu.ar`
- Empleado: `a@a.com`
- Propietario (para la API): `lucia.ferreyra@mail.com`

Las claves de ejemplo están guardadas con el formato de hash anterior. La primera vez que alguien inicia sesión, la app la vuelve a guardar con el formato actual (ver `Services/ServicioClaves.cs`). Por eso la entrada `Salt` de `appsettings.json` tiene que quedar, al menos mientras existan claves viejas.

## Regenerar el script

Si la base cambia (tablas o datos) y querés actualizar `seed.sql`:

1. En SQL Server Management Studio, hacé clic derecho en la base y elegí **Tareas → Generar scripts**.
2. Elegí todas las tablas.
3. En **Avanzado**, poné **Tipos de datos para incluir en el script** en *Esquema y datos*.
4. Reemplazá en `seed.sql` todo lo que está debajo de `USE [BDInmobiliaria];` por lo generado. Dejá el encabezado, que es el que crea la base y frena si ya existe.
