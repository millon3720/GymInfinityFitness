CREATE DATABASE GymInfinityFitness;
GO
USE GymInfinityFitness;
GO

-- Usuarios generales
CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY(1000,1),
    Cedula NVARCHAR(20) UNIQUE NOT NULL,
    NombreCompleto NVARCHAR(100) NOT NULL,
    FechaNacimiento DATE,
    Correo NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20),
    Direccion NVARCHAR(200),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Rol NVARCHAR(20) NOT NULL CHECK (Rol IN ('Cliente', 'Entrenador')),
    Estado BIT DEFAULT 1
);
GO
CREATE TABLE UsuariosLogin (
    IdUsuarioLogin INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Contrasena NVARCHAR(256) NOT NULL,
    Usuario NVARCHAR(100) NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO
CREATE TABLE Expedientes (
    IdExpediente INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    PesoKg DECIMAL(5,2),
    AlturaCm INT,
    IMC DECIMAL(5,2),
    PorcentajeGrasa DECIMAL(5,2),
    PorcentajeMuscular DECIMAL(5,2),
    MedidaPecho DECIMAL(5,2),
    MedidaEspalda DECIMAL(5,2),
    MedidaCintura DECIMAL(5,2),
    MedidaCadera DECIMAL(5,2),
    BicepDerecho DECIMAL(5,2),
    BicepIzquierdo DECIMAL(5,2),
    PiernaDerecha DECIMAL(5,2),
    PiernaIzquierda DECIMAL(5,2),
    PantorrillaDerecha DECIMAL(5,2),
    PantorrillaIzquierda DECIMAL(5,2),
    Observaciones NVARCHAR(1000),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO

CREATE TABLE Lesiones (
    IdLesion INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Descripcion NVARCHAR(300),
    FechaDiagnostico DATE,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO
CREATE TABLE Asistencias (
    IdAsistencia INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    FechaIngreso DATETIME DEFAULT GETDATE(),
    FechaSalida DATETIME NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO
CREATE TABLE Rutinas (
    IdRutina INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100),
    Descripcion NVARCHAR(300),
    FechaCreacion DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Ejercicios (
    IdEjercicio INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100),
    Descripcion NVARCHAR(300),
    VideoURL NVARCHAR(300)
);
GO

CREATE TABLE RutinaEjercicios (
    IdRutinaEjercicio INT PRIMARY KEY IDENTITY(1,1),
    IdRutina INT,
    IdEjercicio INT,
    Series INT,
    Repeticiones INT,
    DescansoSegundos INT,
    DiaSemana NVARCHAR(20),
    FOREIGN KEY (IdRutina) REFERENCES Rutinas(IdRutina) ON DELETE CASCADE,
    FOREIGN KEY (IdEjercicio) REFERENCES Ejercicios(IdEjercicio) ON DELETE CASCADE
);
GO

CREATE TABLE ClienteRutina (
    IdClienteRutina INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    IdRutina INT,
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    Observaciones NVARCHAR(500),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE,
    FOREIGN KEY (IdRutina) REFERENCES Rutinas(IdRutina) ON DELETE CASCADE
);
GO

CREATE TABLE PlanesNutricionales (
    IdPlan INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Descripcion NVARCHAR(500),
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO
CREATE TABLE AlimentosPlanNutricional (
    Id INT PRIMARY KEY IDENTITY(1,1) ,
	IdPlan int,
    DiaSemana NVARCHAR(20),        
    Comida NVARCHAR(20),           
    HoraEstimada TIME,             
    Alimento NVARCHAR(200),        
    Porciones NVARCHAR(50),       
    Comentarios NVARCHAR(500)     
	FOREIGN KEY (IdPlan) REFERENCES PlanesNutricionales(IdPlan) ON DELETE CASCADE

);
go

CREATE TABLE ProductosServicios (
    IdProductoServicio INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100),
    Descripcion NVARCHAR(300),
    Tipo NVARCHAR(50),
    Precio DECIMAL(10,2)
);
GO

CREATE TABLE Inventario (
    IdInventario INT PRIMARY KEY IDENTITY(1,1),
    IdProductoServicio INT,
    CantidadDisponible INT,
    PuntoDeReorden INT,
    FOREIGN KEY (IdProductoServicio) REFERENCES ProductosServicios(IdProductoServicio) ON DELETE CASCADE
);
GO

CREATE TABLE Facturas (
    IdFactura INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    Fecha DATETIME DEFAULT GETDATE(),
    Total DECIMAL(10,2),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO

CREATE TABLE DetalleFactura (
    IdDetalleFactura INT PRIMARY KEY IDENTITY(1,1),
    IdFactura INT,
    IdProductoServicio INT,
    Cantidad INT,
    Subtotal Decimal(10,2),
    FOREIGN KEY (IdFactura) REFERENCES Facturas(IdFactura) ON DELETE CASCADE,
    FOREIGN KEY (IdProductoServicio) REFERENCES ProductosServicios(IdProductoServicio) ON DELETE CASCADE
);
GO

CREATE TABLE Eventos (
    IdEvento INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100),
    Descripcion NVARCHAR(300),
    FechaEvento DATETIME,
	Imagen VARBINARY(MAX)
);
GO

CREATE TABLE ClienteEvento (
    IdClienteEvento INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT,
    IdEvento INT,
    FechaInscripcion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE,
    FOREIGN KEY (IdEvento) REFERENCES Eventos(IdEvento) ON DELETE CASCADE
);
GO
CREATE TABLE Mensualidades (
    IdMensualidad INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL    
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);