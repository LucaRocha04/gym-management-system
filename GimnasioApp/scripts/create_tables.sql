-- SQLite Schema para GimnasioApp
PRAGMA foreign_keys = ON;

CREATE TABLE usuarios (
  id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
  nombre_usuario TEXT NOT NULL,
  mail TEXT UNIQUE,
  password TEXT NOT NULL,
  rol TEXT NOT NULL CHECK (rol IN ('Administrador','Recepcionista','Profesor'))
);

CREATE TABLE planes (
  id_plan INTEGER PRIMARY KEY AUTOINCREMENT,
  nombre_plan TEXT NOT NULL,
  duracion_dias INTEGER NOT NULL,
  precio REAL NOT NULL,
  descripcion TEXT
);

CREATE TABLE socios (
  id_socio INTEGER PRIMARY KEY AUTOINCREMENT,
  nombre TEXT NOT NULL,
  apellido TEXT NOT NULL,
  dni TEXT NOT NULL UNIQUE,
  telefono TEXT,
  mail TEXT,
  direccion TEXT,
  fecha_ingreso DATE,
  id_plan INTEGER,
  estado TEXT DEFAULT 'Activo' CHECK (estado IN ('Activo','Inactivo')),
  FOREIGN KEY (id_plan) REFERENCES planes(id_plan) ON DELETE SET NULL
);

CREATE TABLE pagos (
  id_pago INTEGER PRIMARY KEY AUTOINCREMENT,
  id_socio INTEGER NOT NULL,
  fecha_pago DATE NOT NULL,
  monto REAL NOT NULL CHECK (monto > 0),
  metodo TEXT NOT NULL CHECK (metodo IN ('Efectivo','Tarjeta','Transferencia')),
  observaciones TEXT,
  FOREIGN KEY (id_socio) REFERENCES socios(id_socio) ON DELETE CASCADE
);

CREATE TABLE asistencias (
  id_asistencia INTEGER PRIMARY KEY AUTOINCREMENT,
  id_socio INTEGER NOT NULL,
  fecha DATE NOT NULL,
  hora_entrada TIME NOT NULL,
  hora_salida TIME,
  observaciones TEXT,
  FOREIGN KEY (id_socio) REFERENCES socios(id_socio) ON DELETE CASCADE
);

-- Clases y sus inscripciones
CREATE TABLE clases (
  id_clase INTEGER PRIMARY KEY AUTOINCREMENT,
  nombre TEXT NOT NULL,
  descripcion TEXT,
  fecha DATE NOT NULL,
  hora_inicio TIME NOT NULL,
  hora_fin TIME NOT NULL,
  cupo INTEGER NOT NULL CHECK (cupo > 0),
  id_profesor INTEGER NOT NULL,
  estado TEXT NOT NULL DEFAULT 'Activa' CHECK (estado IN ('Activa','Cancelada')),
  FOREIGN KEY (id_profesor) REFERENCES usuarios(id_usuario) ON DELETE CASCADE
);

CREATE TABLE clase_inscripciones (
  id_inscripcion INTEGER PRIMARY KEY AUTOINCREMENT,
  id_clase INTEGER NOT NULL,
  id_socio INTEGER NOT NULL,
  fecha_inscripcion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_clase) REFERENCES clases(id_clase) ON DELETE CASCADE,
  FOREIGN KEY (id_socio) REFERENCES socios(id_socio) ON DELETE CASCADE,
  UNIQUE(id_clase, id_socio)
);