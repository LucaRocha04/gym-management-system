-- Datos iniciales para SQLite
PRAGMA foreign_keys = ON;

-- Planes ejemplo
INSERT INTO planes (nombre_plan, duracion_dias, precio, descripcion) VALUES
('Mensual', 30, 2000.00, 'Plan mensual'),
('Trimestral', 90, 5500.00, 'Plan trimestral'),
('Anual', 365, 20000.00, 'Plan anual');

-- Socios ejemplo
INSERT INTO socios (nombre, apellido, dni, telefono, mail, direccion, fecha_ingreso, id_plan, estado) VALUES
('Laura','Gomez','12345678','111-222-333','laura@mail.com','Calle 1', date('now'), 1, 'Activo'),
('Carlos','Lopez','87654321','222-333-444','carlos@mail.com','Calle 2', date('now'), 2, 'Activo');

-- Usuarios iniciales
INSERT INTO usuarios (nombre_usuario, mail, password, rol) VALUES
('admin', 'admin@gimnasio.local', 'admin123', 'Administrador'),
('recep', 'recepcionista@gimnasio.local', 'recep123', 'Recepcionista'),
('prof', 'profesor@gimnasio.local', 'prof123', 'Profesor');

-- Clase de ejemplo (Zumba mañana)
INSERT INTO clases (nombre, descripcion, fecha, hora_inicio, hora_fin, cupo, id_profesor, estado)
VALUES ('Zumba', 'Clase de Zumba para todos los niveles', date('now', '+1 day'), '18:00:00', '19:00:00', 20, 
        (SELECT id_usuario FROM usuarios WHERE nombre_usuario = 'prof' LIMIT 1), 'Activa');